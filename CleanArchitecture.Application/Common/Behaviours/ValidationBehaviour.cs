using System.Linq.Expressions;
using System.Reflection;

namespace CleanArchitecture.Application.Common.Behaviours;

/// <summary>
/// A pipeline behavior that runs all FluentValidation validators registered for a request
/// before the handler executes. When any validator fails, the pipeline short-circuits and
/// returns a failed <see cref="Result"/> instead of invoking the handler.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type, which must be a <see cref="Result"/> or <see cref="Result{TValue}"/>.</typeparam>
internal sealed class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    // Both Result and Result<T> declare a public static Failure(Error[]) factory that returns
    // their own type. Resolve it once per closed generic and cache the compiled delegate.
    private static readonly Func<Error[], TResponse> CreateFailure = BuildFailureFactory();

    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        ValidationResult[] results = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(message, cancellationToken))).ConfigureAwait(false);

        // Carry a DomainError, not a bare Error: it preserves ErrorType.Validation, which is what
        // lets the Presentation layer answer 422 instead of falling through to a generic 400.
        Error[] errors = results
            .Where(result => !result.IsValid)
            .SelectMany(result => result.Errors)
            .Select(failure => (Error)DomainError.Validation(
                string.IsNullOrEmpty(failure.PropertyName) ? "Validation" : failure.PropertyName,
                failure.ErrorMessage))
            .ToArray();

        return errors.Length == 0
            ? await next(message, cancellationToken).ConfigureAwait(false)
            : CreateFailure(errors);
    }

    private static Func<Error[], TResponse> BuildFailureFactory()
    {
        MethodInfo factory = typeof(TResponse).GetMethod(
            nameof(Result.Failure),
            BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,
            binder: null,
            types: [typeof(Error[])],
            modifiers: null)
            ?? throw new InvalidOperationException(
                $"'{typeof(TResponse)}' must expose a public static {nameof(Result.Failure)}({nameof(Error)}[]) factory.");

        ParameterExpression errors = Expression.Parameter(typeof(Error[]), "errors");
        return Expression.Lambda<Func<Error[], TResponse>>(
            Expression.Call(factory, errors), errors).Compile();
    }
}
