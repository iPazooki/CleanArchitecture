using CleanArchitecture.Presentation.Configuration;

namespace CleanArchitecture.Presentation.Endpoints;

internal static class BookEndpoints
{
    public static void MapBookEndpoints(this WebApplication app)
    {
        app.MapPost("/create-book", CreateBook)
            .WithOpenApi()
            .WithSummary("Creates a new book")
            .WithDescription("Creates a new book with the specified title and genre.")
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.Created, "The ID of the created book", typeof(int)))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.BadRequest, "Invalid input parameters"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request", typeof(ProblemDetails)));

        app.MapPut("/update-book", UpdateBook)
            .WithOpenApi()
            .WithSummary("Updates an existing book")
            .WithDescription("Updates an existing book with the specified ID.")
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.NoContent, "The book was successfully updated"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.BadRequest, "Invalid input parameters"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request", typeof(ProblemDetails)));

        app.MapDelete("/delete-book", DeleteBook)
            .WithOpenApi()
            .WithSummary("Deletes an existing book")
            .WithDescription("Deletes an existing book with the specified ID.")
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.NoContent, "The book was successfully deleted"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.BadRequest, "Invalid input parameters"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request", typeof(ProblemDetails)));

        app.MapGet("/get-book/{id:Guid}", GetBook)
            .WithOpenApi()
            .WithSummary("Gets a book by ID")
            .WithDescription("Gets a book with the specified ID.")
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.OK, "The book was found", typeof(BookResponse)))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.NotFound, "The book was not found"))
            .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request", typeof(ProblemDetails)));
    }

    private static async Task<IResult> GetBook(ISender sender, Guid id)
    {
        Result<BookResponse> result = await sender.Send(new GetBookQuery(id)).ConfigureAwait(false);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.NotFound();
    }

    private static async Task<IResult> DeleteBook(ISender sender, [FromBody] DeleteBookCommand command)
    {
        Result result = await sender.SendWithRetryAsync(command).ConfigureAwait(false);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(string.Join(',', result.Errors.Select(x => x.Message)));
    }

    private static async Task<IResult> UpdateBook(ISender sender, UpdateBookCommand command)
    {
        Result result = await sender.SendWithRetryAsync(command).ConfigureAwait(false);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(string.Join(',', result.Errors.Select(x => x.Message)));
    }

    private static async Task<IResult> CreateBook(ISender sender, CreateBookCommand command)
    {
        Result<Guid> result = await sender.Send(command).ConfigureAwait(false);

        return !result.IsSuccess
            ? Results.BadRequest(string.Join(',', result.Errors.Select(x => x.Message)))
            : Results.Created($"/create-book/{result.Value}", result);
    }
}
