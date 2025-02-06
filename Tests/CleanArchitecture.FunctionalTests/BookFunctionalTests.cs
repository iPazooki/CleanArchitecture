using Moq;
using DomainValidation;
using System.Net.Http.Json;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Application.Common;
using CleanArchitecture.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using CleanArchitecture.FunctionalTests.Abstractions;
using CleanArchitecture.Application.Entities.Books.Queries.Get;
using CleanArchitecture.Application.Entities.Books.Commands.Create;
using CleanArchitecture.Application.Entities.Books.Commands.Delete;
using CleanArchitecture.Application.Entities.Books.Commands.Update;

namespace CleanArchitecture.FunctionalTests;

public class BookFunctionalTests
{
    private readonly Mock<IApplicationUnitOfWork> _mockApplicationUnitOfWork = new();

    private HttpClient CreateHttpClient()
    {
        FunctionalWebApplicationFactory factory = new(serviceCollection =>
        {
            serviceCollection.AddScoped(_ => _mockApplicationUnitOfWork.Object);
        });

        return factory.CreateClient();
    }

    [Fact]
    public async Task CreateBookCommand_WithValidRequest_CreatesBook()
    {
        // Arrange
        _mockApplicationUnitOfWork.Setup(x =>
                x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _mockApplicationUnitOfWork.Setup(x => x.Books.Add(It.IsAny<Book>()));

        CreateBookCommand command = new("Test Book", "F");
        HttpClient httpClient = CreateHttpClient();

        // Act
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("/create-book/", command);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdateBookCommand_WithValidRequest_UpdatesBook()
    {
        // Arrange
        _mockApplicationUnitOfWork.Setup(x =>
                x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _mockApplicationUnitOfWork.Setup(x => x.Books.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Book { Title = "Test Book", Genre = Genre.Fiction });

        _mockApplicationUnitOfWork.Setup(x => x.Books.Update(It.IsAny<Book>()));

        UpdateBookCommand command = new(1, "Updated Book", "F");
        HttpClient httpClient = CreateHttpClient();

        // Act
        HttpResponseMessage response = await httpClient.PutAsJsonAsync("/update-book/", command);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task DeleteBookCommand_WithValidRequest_DeletesBook()
    {
        // Arrange
        _mockApplicationUnitOfWork.Setup(x =>
                x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _mockApplicationUnitOfWork.Setup(x => x.Books.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Book { Title = "Test Book", Genre = Genre.Fiction });

        _mockApplicationUnitOfWork.Setup(x => x.Books.Remove(It.IsAny<Book>()));

        DeleteBookCommand command = new(1);
        HttpClient httpClient = CreateHttpClient();

        // Act
        HttpRequestMessage request = new(HttpMethod.Delete, "/delete-book")
        {
            Content = JsonContent.Create(command)
        };
        HttpResponseMessage response = await httpClient.SendAsync(request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task GetBookQuery_WithValidId_ReturnsBook()
    {
        // Arrange
        Book book = new() { Id = 1, Title = "Test Book", Genre = Genre.Fiction };

        _mockApplicationUnitOfWork.Setup(x => x.Books.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        HttpClient httpClient = CreateHttpClient();

        // Act
        HttpResponseMessage response = await httpClient.GetAsync("/get-book/1");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(response.Content);
        Result<BookDto>? returnedBook = await response.Content.ReadFromJsonAsync<Result<BookDto>>();
        Assert.NotNull(returnedBook);
        Assert.NotNull(returnedBook.Value);
        Assert.Equal(book.Title, returnedBook.Value.Title);
        Assert.Equal(book.Genre, returnedBook.Value.Genre);
    }
}