using CleanArchitecture.Application.Entities.Users.Queries.Get;
using CleanArchitecture.Application.Entities.Users.Commands.Create;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Presentation.Endpoints;

internal static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapPost("/create-user", CreateUser)
            .WithSummary("Creates a new user")
            .WithDescription("Creates a new user with the specified details.");

        app.MapGet("/get-user/{id:Guid}", GetUser)
            .WithSummary("Gets a user by ID")
            .WithDescription("Gets a user with the specified ID.")
            .RequireAuthorization(Permissions.CanRead.ToString());
    }

    private static async Task<IResult> GetUser(ISender sender, Guid id)
    {
        Result<UserResponse> result = await sender.Send(new GetUserQuery(id)).ConfigureAwait(false);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.NotFound();
    }

    private static async Task<IResult> CreateUser(ISender sender, CreateUserCommand command)
    {
        Result<Guid> result = await sender.Send(command).ConfigureAwait(false);

        return !result.IsSuccess
            ? Results.BadRequest(string.Join(',', result.Errors.Select(x => x.Message)))
            : Results.Created($"/create-user/{result.Value}", result);
    }
}
