using DomainValidation;
using CleanArchitecture.IntegrationTests.Abstractions;
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
        // Act
        int bookId = await CreateBookAsync("Test Book", "F");

        // Assert
        Assert.NotEqual(0, bookId);
    }

    [Fact]
    public async Task GetBookQuery_WithValidRequest_ReturnsBook()
    {
        // Arrange
        int bookId = await CreateBookAsync("Test Book", "F");
        GetBookQuery query = new(bookId);

        // Act
        Result<BookDto> result = await Sender.Send(query);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Book", result.Value.Title);
        Assert.Equal("F", result.Value.Genre);
    }

    [Fact]
    public async Task UpdateBookCommand_WithValidRequest_UpdatesBook()
    {
        // Arrange
        int bookId = await CreateBookAsync("Test Book", "F");
        UpdateBookCommand updateCommand = new(bookId, "Updated Book", "M");

        // Act
        Result result = await Sender.Send(updateCommand);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteBookCommand_WithValidRequest_DeletesBook()
    {
        // Arrange
        int bookId = await CreateBookAsync("Test Book", "F");
        DeleteBookCommand deleteCommand = new(bookId);

        // Act
        Result result = await Sender.Send(deleteCommand);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }
}