using CleanArchitecture.Application.Entities.Persons.Queries.Get;
using CleanArchitecture.Application.Entities.Persons.Commands.Create;

namespace CleanArchitecture.Presentation.Endpoints;

public static class PersonEndpoints
{
    public static void MapPersonEndpoints(this WebApplication app)
    {
        app.MapPost("/create-person", CreatePerson)
            .WithOpenApi()
            .WithSummary("Creates a new person")
            .WithDescription("Creates a new person with the specified details.")
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.Created, "The ID of the created person", typeof(int)))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.BadRequest, "Invalid input parameters"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request", typeof(ProblemDetails)));

        app.MapGet("/get-person/{id:int}", GetPerson)
            .WithOpenApi()
            .WithSummary("Gets a person by ID")
            .WithDescription("Gets a person with the specified ID.")
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.OK, "The person was found", typeof(PersonResponse)))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.NotFound, "The person was not found"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request", typeof(ProblemDetails)));
    }

    private static async Task<IResult> GetPerson(ISender sender, int id)
    {
        Result<PersonResponse> result = await sender.Send(new GetPersonQuery(id));

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.NotFound();
    }

    private static async Task<IResult> CreatePerson(ISender sender, CreatePersonCommand command)
    {
        Result<int> result = await sender.Send(command);

        return !result.IsSuccess
            ? Results.BadRequest(string.Join(',', result.Errors.Select(x => x.Message)))
            : Results.Created($"/create-person/{result.Value}", result);
    }
}