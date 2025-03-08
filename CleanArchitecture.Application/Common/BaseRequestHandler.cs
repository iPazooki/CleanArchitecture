namespace CleanArchitecture.Application.Common;

public abstract class BaseRequestHandler<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) 
    : IRequestHandler<TRequest, Result<TResponse>> where TRequest : IRequest<Result<TResponse>>
{
    public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(request, cancellationToken)));

        List<ValidationFailure> failures = validationResults
            .Where(r => r.Errors.Count != 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count == 0)
        {
            return await HandleRequest(request, cancellationToken);
        }

        Error[] errors = failures.Select(f => new Error(f.ErrorMessage)).ToArray();
        
        return Result<TResponse>.Failure(errors);

    }

    protected abstract Task<Result<TResponse>> HandleRequest(TRequest request, CancellationToken cancellationToken);
}

public abstract class BaseRequestHandler<TRequest>(IEnumerable<IValidator<TRequest>> validators)
    : IRequestHandler<TRequest, Result> where TRequest : IRequest<Result>
{
    public async Task<Result> Handle(TRequest request, CancellationToken cancellationToken)
    {
        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(request, cancellationToken)));

        List<ValidationFailure> failures = validationResults
            .Where(r => r.Errors.Count != 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count == 0)
        {
            return await HandleRequest(request, cancellationToken);
        }

        Error[] errors = failures.Select(f => new Error(f.ErrorMessage)).ToArray();
        
        return Result.Failure(errors);
    }

    protected abstract Task<Result> HandleRequest(TRequest request, CancellationToken cancellationToken);
}