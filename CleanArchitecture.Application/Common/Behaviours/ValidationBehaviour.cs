namespace CleanArchitecture.Application.Common.Behaviours;

/// <summary>
/// Represents a validation behavior in the pipeline that validates the request before passing it to the next handler.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
internal sealed class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    /// <summary>
    /// Handles the request by validating it and then passing it to the next handler if validation succeeds.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="next">The delegate representing the next handler in the pipeline.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response from the next handler.</returns>
    /// <exception cref="ApplicationValidationException">Thrown when validation fails.</exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        ValidationContext<TRequest> context = new(request);

        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(v =>
                v.ValidateAsync(context, cancellationToken)));

        List<ValidationFailure> failures = validationResults
            .Where(r => r.Errors.Count != 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count != 0)
        {
            throw new ApplicationValidationException(failures);
        }

        return await next();
    }
}