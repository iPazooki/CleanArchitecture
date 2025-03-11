using CleanArchitecture.Application.Entities.Login.Commands;

namespace CleanArchitecture.Presentation.Endpoints;

internal static class MemberEndpoints
{
    public static void MapMemberEndpoints(this WebApplication app)
    {
        app.MapPost("/login", LoginMember)
            .WithOpenApi()
            .WithSummary("Logs in a member")
            .WithDescription("Logs in a member with the specified details.")
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.OK, "JWT Token is created", typeof(int)))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.BadRequest, "Invalid input parameters"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError,
                "An error occurred while processing the request", typeof(ProblemDetails)));
    }

    private static async Task<IResult> LoginMember(ISender sender, LoginCommand command)
    {
        Result<string> result = await sender.Send(command).ConfigureAwait(false);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(string.Join(',', result.Errors.Select(x => x.Message)));
    }
}
