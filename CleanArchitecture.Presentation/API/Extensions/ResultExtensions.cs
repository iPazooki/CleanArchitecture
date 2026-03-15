using Microsoft.AspNetCore.Mvc;
using CleanArchitecture.Domain.Common;
using DomainValidationError = DomainValidation.Error;

namespace CleanArchitecture.Api.Extensions;

/// <summary>
/// Extension methods for converting Result types to ProblemDetails responses.
/// </summary>
internal static class ResultExtensions
{
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
    public static IResult ToCreatedResponse<T>(this Result<T> result, Uri uri)
    {
        if (result.IsSuccess)
        {
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
                detail: "An unknown error occurred.");
        }

        // Convert DomainValidation.Error to our format
        // DomainValidation.Error has Code and Message properties

        // For now, treat all as validation errors (422)
        // You can enhance this to parse error codes if needed
        int statusCode = StatusCodes.Status422UnprocessableEntity;
        string title = "Validation Error";

        // Create ProblemDetails with structured errors
        ProblemDetails problemDetails = new()
        {
            Status = statusCode,
            Title = title,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Extensions =
            {
                ["errors"] = errorArray.Select(e => new
                {
                    code = e.Code,
                    message = e.Message
                }).ToArray()
            }
        };

        return Results.Problem(problemDetails);
    }
}
