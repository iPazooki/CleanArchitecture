using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Entities.Books.Commands.Delete;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;
using DomainValidation;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Application.UnitTests.Entities.Books;

public class DeleteBookCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldDeleteBookAndReturnSuccess()
    {
        // Arrange
        Mock<IApplicationUnitOfWork> mockUnitOfWork = new();
        Mock<DbSet<Book>> mockBooksDbSet = new();
        Book book = new()
        {
            Title = "Test Title",
            Genre = Genre.Fiction
        };

        mockUnitOfWork.Setup(uow => uow.Books).Returns(mockBooksDbSet.Object);
        mockBooksDbSet.Setup(dbSet => dbSet.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);
        mockBooksDbSet.Setup(dbSet => dbSet.Remove(It.IsAny<Book>()));
        mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success);

        DeleteBookCommandHandler handler = new(mockUnitOfWork.Object);
        DeleteBookCommand command = new(1);

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        mockBooksDbSet.Verify(dbSet => dbSet.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>()), Times.Once);
        mockBooksDbSet.Verify(dbSet => dbSet.Remove(It.IsAny<Book>()), Times.Once);
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

        DeleteBookCommandHandler handler = new(mockUnitOfWork.Object);
        DeleteBookCommand command = new(1);

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.Errors.Count() == 1);
        Assert.Equal("Book Not Found.", result.Errors.First().Message);
        mockBooksDbSet.Verify(dbSet => dbSet.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>()), Times.Once);
        mockBooksDbSet.Verify(dbSet => dbSet.Remove(It.IsAny<Book>()), Times.Never);
        mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}