using CleanArchitecture.Application.Entities.Books.Queries.Get;
using CleanArchitecture.Application.Entities.Books.Commands.Create;
using CleanArchitecture.Application.Entities.Books.Commands.Delete;
using CleanArchitecture.Application.Entities.Books.Commands.Update;

namespace CleanArchitecture.IntegrationTests.Book;

public class BookIntegrationTests : BaseIntegrationTest
{
    private async Task<int> CreateBookAsync(string title, string genre)
    {
        CreateBookCommand command = new(title, genre);
        Result<int> result = await Sender.Send(command);
        return result.Value;
    }

    [Fact]
    public async Task CreateBookCommand_WithValidRequest_CreatesBook()
    {
        int bookId = await CreateBookAsync("Test Book", "F");
        Assert.NotEqual(0, bookId);
    }

    [Fact]
    public async Task GetBookQuery_WithValidRequest_ReturnsBook()
    {
        int bookId = await CreateBookAsync("Another Book", "F");
        Result<BookResponse> result = await Sender.Send(new GetBookQuery(bookId));

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("Another Book", result.Value!.Title);
    }

    [Fact]
    public async Task UpdateBookCommand_WithValidRequest_UpdatesBook()
    {
        int bookId = await CreateBookAsync("Updatable Book", "F");
        Result updateResult = await Sender.Send(new UpdateBookCommand(bookId, "Updated Title", "M"));

        Assert.NotNull(updateResult);
        Assert.True(updateResult.IsSuccess);
    }

    [Fact]
    public async Task DeleteBookCommand_WithValidRequest_DeletesBook()
    {
        int bookId = await CreateBookAsync("Removable Book", "F");
        Result deleteResult = await Sender.Send(new DeleteBookCommand(bookId));

        Assert.NotNull(deleteResult);
        Assert.True(deleteResult.IsSuccess);
    }

    [Fact]
    public async Task CreateBookCommand_WithEmptyTitle_ThrowsException()
    {
        await Assert.ThrowsAsync<CommonValidationException>(
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
        Result<BookResponse> result = await Sender.Send(new GetBookQuery(9999));

        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task UpdateBookCommand_WithNotFoundBook_ReturnsFailure()
    {
        Result updateResult = await Sender.Send(new UpdateBookCommand(9999, "No Book", "F"));

        Assert.NotNull(updateResult);
        Assert.False(updateResult.IsSuccess);
        Assert.Single(updateResult.Errors);
    }

    [Fact]
    public async Task DeleteBookCommand_WithNotFoundBook_ReturnsFailure()
    {
        Result deleteResult = await Sender.Send(new DeleteBookCommand(9999));

        Assert.NotNull(deleteResult);
        Assert.False(deleteResult.IsSuccess);
        Assert.Single(deleteResult.Errors);
    }
}