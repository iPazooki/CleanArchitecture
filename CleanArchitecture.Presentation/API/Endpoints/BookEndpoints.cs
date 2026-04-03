using CleanArchitecture.Api.Configuration;
using CleanArchitecture.Api.Extensions;
using CleanArchitecture.Application.Common;
using CleanArchitecture.Infrastructure.Security;
using Microsoft.AspNetCore.OutputCaching;

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
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .RequireAuthorization(EditorPolicy.Name);

        books.MapPut("/{id:guid}", UpdateBook)
            .WithSummary("Updates an existing book")
            .WithDescription("Updates an existing book with the specified ID.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
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
            .RequireAuthorization(ViewerPolicy.Name)
            .CacheOutput("GetBook");

        books.MapGet("/", GetBooks)
            .WithSummary("Gets all books")
            .WithDescription("Gets books with pagination support")
            .Produces<Result<PaginatedResponse<BookResponse>>>(StatusCodes.Status200OK)
            .RequireAuthorization(ViewerPolicy.Name)
            .CacheOutput("GetBooks");
    }

    private static async Task<IResult> GetBook(ISender sender, Guid id)
    {
        Result<BookResponse> result = await sender.Send(new GetBookQuery(id)).ConfigureAwait(false);

        return result.ToProblemDetails();
    }

    private static async Task<IResult> GetBooks(ISender sender, int page = 1, int pageSize = 10)
    {
        Result<PaginatedResponse<BookResponse>> result = await sender.Send(new GetBooksQuery(page, pageSize)).ConfigureAwait(false);

        return result.ToProblemDetails();
    }

    private static async Task<IResult> DeleteBook(ISender sender, IOutputCacheStore cacheStore, Guid id, CancellationToken cancellationToken)
    {
        DeleteBookCommand command = new(id);
        Result result = await sender.SendWithRetryAsync(command).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            await cacheStore.EvictByTagAsync("books", cancellationToken).ConfigureAwait(false);
        }

        return result.ToNoContentResponse();
    }

    private static async Task<IResult> UpdateBook(ISender sender, IOutputCacheStore cacheStore, Guid id, [FromBody] UpdateBookCommand command, CancellationToken cancellationToken)
    {
        // Ensure the ID from the route is used
        UpdateBookCommand updatedCommand = command with { Id = id };
        Result result = await sender.SendWithRetryAsync(updatedCommand).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            await cacheStore.EvictByTagAsync("books", cancellationToken).ConfigureAwait(false);
        }

        return result.ToNoContentResponse();
    }

    private static async Task<IResult> CreateBook(ISender sender, IOutputCacheStore cacheStore, CreateBookCommand command, CancellationToken cancellationToken)
    {
        Result<Guid> result = await sender.SendWithRetryAsync(command).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            await cacheStore.EvictByTagAsync("books", cancellationToken).ConfigureAwait(false);
        }

        return result.ToCreatedResponse(id => new Uri($"/api/v1/books/{id}", UriKind.Relative));
    }
}