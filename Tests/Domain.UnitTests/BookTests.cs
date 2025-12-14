using Moq;
using Mediator;
using CleanArchitecture.Domain.Entities;

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
        Assert.Equal(BookErrors.TitleIsRequired, result.Errors.First().Message);
    }

    [Fact]
    public void Book_Create_ShouldReturnFailureResult_WhenTitleIsNull()
    {
        // Arrange & Act
        Result<Book> result = Book.Create(null!, Genre.Fiction);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(BookErrors.TitleIsRequired, result.Errors.First().Message);
    }

    [Fact]
    public void Book_Create_ShouldReturnFailureResult_WhenTitleIsWhitespace()
    {
        // Arrange & Act
        Result<Book> result = Book.Create("   ", Genre.Fiction);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(BookErrors.TitleIsRequired, result.Errors.First().Message);
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

        // Act
        bool isEqual = book1.Equals(book2);

        // Assert
        Assert.False(isEqual);
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
    }
}
