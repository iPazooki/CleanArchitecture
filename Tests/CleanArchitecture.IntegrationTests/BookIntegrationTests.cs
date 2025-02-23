using CleanArchitecture.Application.Entities.Books.Commands.Create;
using CleanArchitecture.Application.Entities.Books.Commands.Delete;
using CleanArchitecture.Application.Entities.Books.Commands.Update;
using CleanArchitecture.Application.Entities.Books.Queries.Get;

namespace CleanArchitecture.IntegrationTests;

public class BookIntegrationTests : BaseIntegrationTest
{
    private async Task<Guid> CreateBookAsync(string title, string genre)
    {
        CreateBookCommand command = new(title, genre);
        Result<Guid> result = await Sender.Send(command);
        return result.Value;
    }

    [Fact]
    public async Task CreateBookCommand_WithValidRequest_CreatesBook()
    {
        Guid bookId = await CreateBookAsync("Test Book", "F");
        Assert.NotEqual(Guid.Empty, bookId);
    }

    [Fact]
    public async Task GetBookQuery_WithValidRequest_ReturnsBook()
    {
        Guid bookId = await CreateBookAsync("Another Book", "F");
        Result<BookResponse> result = await Sender.Send(new GetBookQuery(bookId));

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("Another Book", result.Value!.Title);
    }

    [Fact]
    public async Task UpdateBookCommand_WithValidRequest_UpdatesBook()
    {
        Guid bookId = await CreateBookAsync("Updatable Book", "F");
        Result updateResult = await Sender.Send(new UpdateBookCommand(bookId, "Updated Title", "M"));

        Assert.NotNull(updateResult);
        Assert.True(updateResult.IsSuccess);
    }

    [Fact]
    public async Task DeleteBookCommand_WithValidRequest_DeletesBook()
    {
        Guid bookId = await CreateBookAsync("Removable Book", "F");
        Result deleteResult = await Sender.Send(new DeleteBookCommand(bookId));

        Assert.NotNull(deleteResult);
        Assert.True(deleteResult.IsSuccess);
    }

    [Fact]
    public async Task CreateBookCommand_WithEmptyTitle_ThrowsException()
    {
        await Assert.ThrowsAsync<ApplicationValidationException>(
            () => CreateBookAsync(" ", "F"));
    }

    [Fact]
    public async Task CreateBookCommand_WithUnsupportedGenre_ThrowsException()
    {
        await Assert.ThrowsAsync<UnsupportedGenreException>(
            () => CreateBookAsync("Any Book", "InvalidGenre"));
    }

    [Fact]
    public async Task GetBookQuery_WithNotFound_ReturnsFailure()
    {
        Result<BookResponse> result = await Sender.Send(new GetBookQuery(Guid.NewGuid()));

        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task UpdateBookCommand_WithNotFoundBook_ReturnsFailure()
    {
        Result updateResult = await Sender.Send(new UpdateBookCommand(Guid.NewGuid(), "No Book", "F"));

        Assert.NotNull(updateResult);
        Assert.False(updateResult.IsSuccess);
        Assert.Single(updateResult.Errors);
    }

    [Fact]
    public async Task DeleteBookCommand_WithNotFoundBook_ReturnsFailure()
    {
        Result deleteResult = await Sender.Send(new DeleteBookCommand(Guid.NewGuid()));

        Assert.NotNull(deleteResult);
        Assert.False(deleteResult.IsSuccess);
        Assert.Single(deleteResult.Errors);
    }
}