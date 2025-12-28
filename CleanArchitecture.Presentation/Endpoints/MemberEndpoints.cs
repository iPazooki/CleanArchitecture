using CleanArchitecture.Application.Entities.Login.Commands;

namespace CleanArchitecture.Presentation.Endpoints;

internal static class MemberEndpoints
{
    public static void MapMemberEndpoints(this WebApplication app)
    {
        app.MapPost("/login", LoginMember)
            .WithSummary("Logs in a member")
            .WithDescription("Logs in a member with the specified details.");

        app.MapPost("/refresh-token", RefreshToken)
            .WithSummary("Refreshes the JWT token")
            .WithDescription("Refreshes the JWT token using the provided refresh token.");
    }

    private static async Task<IResult> LoginMember(ISender sender, LoginCommand command)
    {
        Result<JwtTokenResponse> result = await sender.Send(command).ConfigureAwait(false);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(string.Join(',', result.Errors.Select(x => x.Message)));
    }

    private static async Task<IResult> RefreshToken(ISender sender, RefreshTokenCommand command)
    {
        Result<JwtTokenResponse> result = await sender.Send(command).ConfigureAwait(false);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.BadRequest(string.Join(',', result.Errors.Select(x => x.Message)));
    }
}
