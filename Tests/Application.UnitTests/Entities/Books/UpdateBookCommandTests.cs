using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Entities.Books.Commands.Update;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;
using DomainValidation;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Application.UnitTests.Entities.Books;

public class UpdateBookCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldUpdateBookAndReturnSuccess()
    {
        // Arrange
        Mock<IApplicationUnitOfWork> mockUnitOfWork = new();
        Mock<DbSet<Book>> mockBooksDbSet = new();
        Book book = new()
        {
            Title = "Old Title",
            Genre = Genre.Fiction
        };

        mockUnitOfWork.Setup(uow => uow.Books).Returns(mockBooksDbSet.Object);
        mockBooksDbSet.Setup(dbSet => dbSet.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);
        mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success);

        UpdateBookCommandHandler handler = new(mockUnitOfWork.Object);
        UpdateBookCommand command = new(1, "New Title", "F");

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("New Title", book.Title);
        mockBooksDbSet.Verify(dbSet => dbSet.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBookNotFound()
    {
        // Arrange
        Mock<IApplicationUnitOfWork> mockUnitOfWork = new();
        Mock<DbSet<Book>> mockBooksDbSet = new();

        mockUnitOfWork.Setup(uow => uow.Books).Returns(mockBooksDbSet.Object);
        mockBooksDbSet.Setup(dbSet => dbSet.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        UpdateBookCommandHandler handler = new(mockUnitOfWork.Object);
        UpdateBookCommand command = new(1, "New Title", "F");

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.Errors.Count() == 1);
        Assert.Equal("Book Not Found.", result.Errors.First().Message);
        mockBooksDbSet.Verify(dbSet => dbSet.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}