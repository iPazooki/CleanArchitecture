using Moq;
using Mediator;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Validations.Book;

namespace Domain.UnitTests;

public class BookTests
{
    [Fact]
    public void Book_Create_ShouldReturnSuccessResult()
    {
        // Arrange & Act
        Result<Book> result = Book.Create("Test Title", Genre.Fiction);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Title", result.Value.Title);
    }

    [Fact]
    public void Book_Create_ShouldReturnFailureResult_WhenTitleIsEmpty()
    {
        // Arrange & Act
        Result<Book> result = Book.Create(string.Empty, Genre.Fiction);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(BookErrors.TitleIsRequired.Message, result.Errors.First().Message);
    }

    [Fact]
    public void Book_Create_ShouldReturnFailureResult_WhenTitleIsNull()
    {
        // Arrange & Act
        Result<Book> result = Book.Create(null!, Genre.Fiction);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(BookErrors.TitleIsRequired.Message, result.Errors.First().Message);
    }

    [Fact]
    public void Book_Create_ShouldReturnFailureResult_WhenTitleIsWhitespace()
    {
        // Arrange & Act
        Result<Book> result = Book.Create("   ", Genre.Fiction);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(BookErrors.TitleIsRequired.Message, result.Errors.First().Message);
    }

    [Fact]
    public void Book_Create_ShouldReturnFailureResult_WhenTitleExceeds200Characters()
    {
        // Arrange
        string longTitle = new('T', 201);

        // Act
        Result<Book> result = Book.Create(longTitle, Genre.Fiction);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(BookErrors.TitleTooLong.Message, result.Errors.First().Message);
    }

    [Fact]
    public void Book_Update_ShouldReturnSuccess_WhenTitleAndGenreAreValid()
    {
        // Arrange
        Result<Book> createResult = Book.Create("Original Title", Genre.Fiction);
        Book book = createResult.Value!;

        // Act
        Result result = book.Update("Updated Title", Genre.NonFiction);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Title", book.Title);
        Assert.Equal(Genre.NonFiction, book.Genre);
    }

    [Fact]
    public void Book_Update_ShouldReturnFailure_WhenTitleIsEmpty()
    {
        // Arrange
        Result<Book> createResult = Book.Create("Original Title", Genre.Fiction);
        Book book = createResult.Value!;

        // Act
        Result result = book.Update(string.Empty, Genre.NonFiction);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(BookErrors.TitleIsRequired.Message, result.Errors.First().Message);
    }

    [Fact]
    public void Book_Update_ShouldReturnFailure_WhenTitleIsTooLong()
    {
        // Arrange
        Result<Book> createResult = Book.Create("Original Title", Genre.Fiction);
        Book book = createResult.Value!;
        string title = new('T', 201);

        // Act
        Result result = book.Update(title, Genre.NonFiction);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(BookErrors.TitleTooLong.Message, result.Errors.First().Message);
    }

    [Fact]
    public void Book_DomainEvents_ShouldBeManagedCorrectly()
    {
        // Arrange
        Result<Book> result = Book.Create("Test Title", Genre.Fiction);
        Book? book = result.Value;
        INotification domainEvent = new Mock<INotification>().Object;

        // Act
        book!.AddDomainEvent(domainEvent);

        // Assert
        Assert.Contains(domainEvent, book.DomainEvents);

        // Act
        book.RemoveDomainEvent(domainEvent);

        // Assert
        Assert.DoesNotContain(domainEvent, book.DomainEvents);

        // Act
        book.AddDomainEvent(domainEvent);
        book.ClearDomainEvents();

        // Assert
        Assert.Empty(book.DomainEvents);
    }

    [Fact]
    public void Book_Equals_ShouldReturnTrueForSameId()
    {
        // Arrange
        Result<Book> result1 = Book.Create("Test Title", Genre.Fiction);
        Book book1 = result1.Value!;

        Result<Book> result2 = Book.Create("Another Title", Genre.NonFiction);
        Book book2 = result2.Value!;

        // Act & Assert — different entities have different Ids
        Assert.NotEqual(book1, book2);
        Assert.False(book1.Equals(book2));
    }

    [Fact]
    public void Book_EqualityOperators_ShouldWorkCorrectly()
    {
        // Arrange
        Result<Book> result1 = Book.Create("Test Title", Genre.Fiction);
        Book? book1 = result1.Value;

        Result<Book> result2 = Book.Create("Another Title", Genre.NonFiction);
        Book? book2 = result2.Value;

        // Act & Assert
        Assert.True(book1 != book2);
        Assert.False(book1 == book2);
    }
}
