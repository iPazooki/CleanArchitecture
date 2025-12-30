using CleanArchitecture.Presentation.Configuration;

namespace CleanArchitecture.Presentation.Endpoints;

internal static class BookEndpoints
{
    public static void MapBookEndpoints(this WebApplication app)
    {
        app.MapPost("/create-book", CreateBook)
            .WithSummary("Creates a new book")
            .WithDescription("Creates a new book with the specified title and genre.")
            .Produces<Result<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        app.MapPut("/update-book", UpdateBook)
            .WithSummary("Updates an existing book")
            .WithDescription("Updates an existing book with the specified ID.")
            .Produces<Result<Guid>>(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        app.MapDelete("/delete-book", DeleteBook)
            .WithSummary("Deletes an existing book")
            .WithDescription("Deletes an existing book with the specified ID.")
            .Produces<Result<Guid>>(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        app.MapGet("/get-book/{id:Guid}", GetBook)
            .WithSummary("Gets a book by ID")
            .WithDescription("Gets a book with the specified ID.")
            .Produces<Result<BookResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
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
