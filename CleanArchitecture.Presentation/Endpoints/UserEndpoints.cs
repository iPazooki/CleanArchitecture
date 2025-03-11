using CleanArchitecture.Application.Entities.Users.Queries.Get;
using CleanArchitecture.Application.Entities.Users.Commands.Create;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Presentation.Endpoints;

internal static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapPost("/create-user", CreateUser)
            .WithOpenApi()
            .WithSummary("Creates a new user")
            .WithDescription("Creates a new user with the specified details.")
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.Created, "The ID of the created user", typeof(int)))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.BadRequest, "Invalid input parameters"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request", typeof(ProblemDetails)));

        app.MapGet("/get-user/{id:Guid}", GetUser)
            .WithOpenApi()
            .WithSummary("Gets a user by ID")
            .WithDescription("Gets a user with the specified ID.")
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.OK, "The user was found", typeof(UserResponse)))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.NotFound, "The user was not found"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request", typeof(ProblemDetails)))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.Unauthorized, "The request requires user authentication", typeof(ProblemDetails)))
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
