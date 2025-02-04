using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Entities.Books.Commands.Create;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Exceptions;
using DomainValidation;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Application.UnitTests.Entities.Books;

public class CreateBookCommandHandlerTests
{
    [Fact]
    public async Task HandlesCreateBookCommandSuccessfully()
    {
        // Arrange
        Mock<IApplicationUnitOfWork> mockUnitOfWork = new();
        Mock<DbSet<Book>> mockBooksDbSet = new();

        mockUnitOfWork.Setup(uow => uow.Books).Returns(mockBooksDbSet.Object);
        mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success());

        CreateBookCommandHandler handler = new(mockUnitOfWork.Object);
        CreateBookCommand command = new("Test Title", "F");

        // Act
        Result<int> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value);
        mockBooksDbSet.Verify(repo => repo.Add(It.IsAny<Book>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandlesCreateBookCommandFailure()
    {
        // Arrange
        Mock<IApplicationUnitOfWork> mockUnitOfWork = new();
        Mock<DbSet<Book>> mockBooksDbSet = new();

        mockUnitOfWork.Setup(uow => uow.Books).Returns(mockBooksDbSet.Object);
        mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure("Error"));

        CreateBookCommandHandler handler = new(mockUnitOfWork.Object);
        CreateBookCommand command = new("Test Title", "F");

        // Act
        Result<int> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("An error occurred while creating the book.", result.Errors.First().Message);
        mockBooksDbSet.Verify(repo => repo.Add(It.IsAny<Book>()), Times.Once);
        mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandlesCreateBookCommandWithInvalidGenre()
    {
        // Arrange
        Mock<IApplicationUnitOfWork> mockUnitOfWork = new();
        Mock<DbSet<Book>> mockBooksDbSet = new();
        mockUnitOfWork.Setup(uow => uow.Books).Returns(mockBooksDbSet.Object);
        mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success());

        CreateBookCommandHandler handler = new(mockUnitOfWork.Object);
        CreateBookCommand command = new("Test Title", "InvalidGenre");

        // Act
        await Assert.ThrowsAsync<UnsupportedGenreException>(()=> handler.Handle(command, CancellationToken.None));
    }
}