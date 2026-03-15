using CleanArchitecture.Api.Configuration;
using CleanArchitecture.Api.Extensions;
using CleanArchitecture.Infrastructure.Security;

namespace CleanArchitecture.Api.Endpoints;

internal static class BookEndpoints
{
    public static void MapBookEndpoints(this RouteGroupBuilder group)
    {
        RouteGroupBuilder books = group.MapGroup("/books")
            .WithTags("Books");

        books.MapPost("/", CreateBook)
            .WithSummary("Creates a new book")
            .WithDescription("Creates a new book with the specified title and genre.")
            .Produces<Result<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(EditorPolicy.Name);

        books.MapPut("/{id:guid}", UpdateBook)
            .WithSummary("Updates an existing book")
            .WithDescription("Updates an existing book with the specified ID.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(EditorPolicy.Name);

        books.MapDelete("/{id:guid}", DeleteBook)
            .WithSummary("Deletes an existing book")
            .WithDescription("Deletes an existing book with the specified ID.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(EditorPolicy.Name);

        books.MapGet("/{id:guid}", GetBook)
            .WithSummary("Gets a book by ID")
            .WithDescription("Gets a book with the specified ID.")
            .Produces<Result<BookResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(ViewerPolicy.Name);

        books.MapGet("/", GetBooks)
            .WithSummary("Gets all books")
            .WithDescription("Gets all books without pagination")
            .Produces<Result<IEnumerable<BookResponse>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(ViewerPolicy.Name);
    }

    private static async Task<IResult> GetBook(ISender sender, Guid id)
    {
        Result<BookResponse> result = await sender.Send(new GetBookQuery(id)).ConfigureAwait(false);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.NotFound();
    }

    private static async Task<IResult> GetBooks(ISender sender)
    {
        Result<IEnumerable<BookResponse>> result = await sender.Send(new GetBooksQuery()).ConfigureAwait(false);

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.NotFound();
    }

    private static async Task<IResult> DeleteBook(ISender sender, Guid id)
    {
        DeleteBookCommand command = new(id);
        Result result = await sender.SendWithRetryAsync(command).ConfigureAwait(false);

        return result.ToNoContentResponse();
    }

    private static async Task<IResult> UpdateBook(ISender sender, Guid id, [FromBody] UpdateBookCommand command)
    {
        // Ensure the ID from the route is used
        UpdateBookCommand updatedCommand = command with { Id = id };
        Result result = await sender.SendWithRetryAsync(updatedCommand).ConfigureAwait(false);

        return result.ToNoContentResponse();
    }

    private static async Task<IResult> CreateBook(ISender sender, CreateBookCommand command)
    {
        Result<Guid> result = await sender.Send(command).ConfigureAwait(false);

        return result.ToCreatedResponse(new Uri($"/api/v1/books/{result.Value}", UriKind.Relative));
    }
}
