using CleanArchitecture.Api.Extensions;
using CleanArchitecture.Application.Common;
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
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .RequireAuthorization(PolicyNames.Editor);

        books.MapPut("/{id:guid}", UpdateBook)
            .WithSummary("Updates an existing book")
            .WithDescription("Updates an existing book with the specified ID.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .RequireAuthorization(PolicyNames.Editor);

        books.MapDelete("/{id:guid}", DeleteBook)
            .WithSummary("Deletes an existing book")
            .WithDescription("Deletes an existing book with the specified ID.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(PolicyNames.Admin);

        books.MapGet("/{id:guid}", GetBook)
            .WithSummary("Gets a book by ID")
            .WithDescription("Gets a book with the specified ID.")
            .Produces<BookResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(PolicyNames.Viewer);

        books.MapGet("/", GetBooks)
            .WithSummary("Gets all books")
            .WithDescription("Gets books with pagination support")
            .Produces<PaginatedResponse<BookResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .RequireAuthorization(PolicyNames.Viewer);
    }

    private static async Task<IResult> GetBook(ISender sender, Guid id, CancellationToken cancellationToken)
    {
        Result<BookResponse> result = await sender.Send(new GetBookQuery(id), cancellationToken).ConfigureAwait(false);

        return result.ToProblemDetails();
    }

    private static async Task<IResult> GetBooks([AsParameters] GetBooksQuery query, ISender sender, CancellationToken cancellationToken)
    {
        Result<PaginatedResponse<BookResponse>> result = await sender.Send(query, cancellationToken).ConfigureAwait(false);

        return result.ToProblemDetails();
    }

    private static async Task<IResult> DeleteBook(ISender sender, Guid id, CancellationToken cancellationToken)
    {
        Result result = await sender.Send(new DeleteBookCommand(id), cancellationToken).ConfigureAwait(false);

        return result.ToNoContentResponse();
    }

    private static async Task<IResult> UpdateBook(ISender sender, Guid id, [FromBody] UpdateBookCommand command, CancellationToken cancellationToken)
    {
        // The route id wins over whatever the body claims.
        UpdateBookCommand updatedCommand = command with { Id = id };

        Result result = await sender.Send(updatedCommand, cancellationToken).ConfigureAwait(false);

        return result.ToNoContentResponse();
    }

    private static async Task<IResult> CreateBook(ISender sender, CreateBookCommand command, CancellationToken cancellationToken)
    {
        Result<Guid> result = await sender.Send(command, cancellationToken).ConfigureAwait(false);

        return result.ToCreatedResponse(id => new Uri($"/api/v1/books/{id}", UriKind.Relative));
    }
}
