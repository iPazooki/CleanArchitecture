using DomainValidationError = DomainValidation.Error;

namespace CleanArchitecture.Api.Extensions;

/// <summary>
/// Extension methods for converting Result types to ProblemDetails responses.
/// </summary>
internal static class ResultExtensions
{
    private const string BadRequestType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1";
    private const string NotFoundType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5";
    private const string ConflictType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10";
    private const string ValidationType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.21";
    private const string ServerErrorType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1";

    /// <summary>
    /// Converts a Result to an IResult with appropriate ProblemDetails response.
    /// </summary>
    public static IResult ToProblemDetails(this Result result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok();
        }

        return CreateProblemDetails(result.Errors);
    }

    /// <summary>
    /// Converts a Result&lt;T&gt; to an IResult with appropriate response or ProblemDetails.
    /// </summary>
    public static IResult ToProblemDetails<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(result);
        }

        return CreateProblemDetails(result.Errors);
    }

    /// <summary>
    /// Converts a Result&lt;T&gt; to an IResult with Created response or ProblemDetails.
    /// </summary>
    public static IResult ToCreatedResponse<T>(this Result<T> result, Func<T, Uri> uriFactory)
    {
        ArgumentNullException.ThrowIfNull(uriFactory);

        if (result.IsSuccess)
        {
            Uri uri = uriFactory(result.Value!);
            return Results.Created(uri.ToString(), result);
        }

        return CreateProblemDetails(result.Errors);
    }

    /// <summary>
    /// Converts a Result to NoContent or ProblemDetails.
    /// </summary>
    public static IResult ToNoContentResponse(this Result result)
    {
        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return CreateProblemDetails(result.Errors);
    }

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

        ProblemDetailsMetadata metadata = GetProblemDetailsMetadata(errorArray);

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

    private static ProblemDetailsMetadata GetProblemDetailsMetadata(DomainValidationError[] errors)
    {
        if (Array.Exists(errors, IsNotFoundError))
        {
            return new(StatusCodes.Status404NotFound, "Not Found", NotFoundType);
        }

        if (Array.Exists(errors, IsConflictError))
        {
            return new(StatusCodes.Status409Conflict, "Conflict", ConflictType);
        }

        if (Array.TrueForAll(errors, IsValidationError))
        {
            return new(StatusCodes.Status422UnprocessableEntity, "Validation Error", ValidationType);
        }

        return new(StatusCodes.Status400BadRequest, "Bad Request", BadRequestType);
    }

    private static bool IsValidationError(DomainValidationError error) =>
        !string.IsNullOrWhiteSpace(error.Code) &&
        !IsNotFoundError(error) &&
        !IsConflictError(error);

    private static bool IsNotFoundError(DomainValidationError error) =>
        error.Code?.EndsWith(".NotFound", StringComparison.OrdinalIgnoreCase) is true;

    private static bool IsConflictError(DomainValidationError error) =>
        error.Code?.Contains("Conflict", StringComparison.OrdinalIgnoreCase) is true;

    private readonly record struct ProblemDetailsMetadata(int StatusCode, string Title, string Type);
}
