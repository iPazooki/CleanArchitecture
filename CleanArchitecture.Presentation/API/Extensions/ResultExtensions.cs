using CleanArchitecture.Domain.Common;
using DomainValidationError = DomainValidation.Error;

namespace CleanArchitecture.Api.Extensions;

/// <summary>
/// Translates a <see cref="Result"/> into an HTTP response.
/// </summary>
/// <remarks>
/// Success responses carry the payload itself, never the <see cref="Result"/> envelope: the
/// envelope is an Application-layer concept, and serializing it would also publish the
/// <see cref="DomainValidationError"/> caller metadata (source file, line, member) to clients.
/// </remarks>
internal static class ResultExtensions
{
    private const string NotFoundType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5";
    private const string ConflictType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10";
    private const string ValidationType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.21";
    private const string ServerErrorType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1";

    /// <summary>
    /// Converts a <see cref="Result{T}"/> to a 200 carrying its value, or a ProblemDetails response.
    /// </summary>
    public static IResult ToProblemDetails<T>(this Result<T> result) =>
        result.IsSuccess
            ? Results.Ok(result.Value)
            : CreateProblemDetails(result.Errors);

    /// <summary>
    /// Converts a <see cref="Result{T}"/> to a 201 carrying its value, or a ProblemDetails response.
    /// </summary>
    public static IResult ToCreatedResponse<T>(this Result<T> result, Func<T, Uri> uriFactory)
    {
        ArgumentNullException.ThrowIfNull(uriFactory);

        if (!result.IsSuccess)
        {
            return CreateProblemDetails(result.Errors);
        }

        Uri uri = uriFactory(result.Value!);

        return Results.Created(uri, result.Value);
    }

    /// <summary>
    /// Converts a <see cref="Result"/> to a 204, or a ProblemDetails response.
    /// </summary>
    public static IResult ToNoContentResponse(this Result result) =>
        result.IsSuccess
            ? Results.NoContent()
            : CreateProblemDetails(result.Errors);

    private static IResult CreateProblemDetails(IEnumerable<DomainValidationError> errors)
    {
        DomainValidationError[] errorArray = errors.ToArray();

        if (errorArray.Length == 0)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Server Error",
                type: ServerErrorType,
                detail: "An unknown error occurred.");
        }

        ProblemDetailsMetadata metadata = Classify(errorArray);

        ProblemDetails problemDetails = new()
        {
            Status = metadata.StatusCode,
            Title = metadata.Title,
            Type = metadata.Type,
            Extensions =
            {
                ["errors"] = errorArray.Select(e => new
                {
                    code = e.Code ?? "Error",
                    message = e.Message
                }).ToArray()
            }
        };

        return Results.Problem(problemDetails);
    }

    /// <summary>
    /// Picks the response for the most significant error in the set.
    /// </summary>
    /// <remarks>
    /// The classification travels on the error itself (<see cref="DomainError.Type"/>). Anything
    /// that is not a <see cref="DomainError"/> carries no classification and is treated as an
    /// unexpected server-side failure rather than being reported to the caller as their mistake.
    /// </remarks>
    private static ProblemDetailsMetadata Classify(DomainValidationError[] errors)
    {
        if (Array.Exists(errors, e => TypeOf(e) is ErrorType.NotFound))
        {
            return new(StatusCodes.Status404NotFound, "Not Found", NotFoundType);
        }

        if (Array.Exists(errors, e => TypeOf(e) is ErrorType.Conflict))
        {
            return new(StatusCodes.Status409Conflict, "Conflict", ConflictType);
        }

        if (Array.TrueForAll(errors, e => TypeOf(e) is ErrorType.Validation))
        {
            return new(StatusCodes.Status422UnprocessableEntity, "Validation Error", ValidationType);
        }

        return new(StatusCodes.Status500InternalServerError, "Server Error", ServerErrorType);
    }

    private static ErrorType TypeOf(DomainValidationError error) =>
        error is DomainError domainError ? domainError.Type : ErrorType.Failure;

    private readonly record struct ProblemDetailsMetadata(int StatusCode, string Title, string Type);
}
