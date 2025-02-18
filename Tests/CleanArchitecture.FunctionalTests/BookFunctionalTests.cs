using CleanArchitecture.Domain.Entities.Book;
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
        FunctionalWebApplicationFactory factory = new(services =>
        {
            services.AddScoped(_ => _mockApplicationUnitOfWork.Object);
        });
        return factory.CreateClient();
    }

    [Fact]
    public async Task CreateBookCommand_WithValidRequest_CreatesBook()
    {
        _mockApplicationUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        _mockApplicationUnitOfWork
            .Setup(x => x.Books.Add(It.IsAny<Book>()));

        HttpClient httpClient = CreateHttpClient();
        CreateBookCommand command = new("Test Book", "F");

        HttpResponseMessage response = await httpClient.PostAsJsonAsync("/create-book/", command);

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task CreateBookCommand_WithFailingSave_ReturnsError()
    {
        _mockApplicationUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure("Unable to save"));
        _mockApplicationUnitOfWork
            .Setup(x => x.Books.Add(It.IsAny<Book>()));

        HttpClient httpClient = CreateHttpClient();
        CreateBookCommand command = new("Failing Book", "F");

        HttpResponseMessage response = await httpClient.PostAsJsonAsync("/create-book/", command);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBookCommand_WithValidRequest_UpdatesBook()
    {
        _mockApplicationUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        _mockApplicationUnitOfWork
            .Setup(x => x.Books.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Book.Create("Original", Genre.Fiction).Value);
        _mockApplicationUnitOfWork
            .Setup(x => x.Books.Update(It.IsAny<Book>()));

        HttpClient httpClient = CreateHttpClient();
        UpdateBookCommand command = new(1, "Updated Title", "F");

        HttpResponseMessage response = await httpClient.PutAsJsonAsync("/update-book/", command);

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task UpdateBookCommand_WithNotFound_ReturnsError()
    {
        _mockApplicationUnitOfWork
            .Setup(x => x.Books.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        HttpClient httpClient = CreateHttpClient();
        UpdateBookCommand command = new(999, "Does Not Exist", "F");

        HttpResponseMessage response = await httpClient.PutAsJsonAsync("/update-book/", command);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBookCommand_WithValidRequest_DeletesBook()
    {
        _mockApplicationUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        _mockApplicationUnitOfWork
            .Setup(x => x.Books.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Book.Create("Delete Me", Genre.Fiction).Value);
        _mockApplicationUnitOfWork
            .Setup(x => x.Books.Remove(It.IsAny<Book>()));

        HttpClient httpClient = CreateHttpClient();
        DeleteBookCommand command = new(1);

        HttpRequestMessage request = new(HttpMethod.Delete, "/delete-book")
        {
            Content = JsonContent.Create(command)
        };
        HttpResponseMessage response = await httpClient.SendAsync(request);

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task DeleteBookCommand_WithNotFound_ReturnsError()
    {
        _mockApplicationUnitOfWork
            .Setup(x => x.Books.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        HttpClient httpClient = CreateHttpClient();
        DeleteBookCommand command = new(999);

        HttpRequestMessage request = new(HttpMethod.Delete, "/delete-book")
        {
            Content = JsonContent.Create(command)
        };
        HttpResponseMessage response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetBookQuery_WithValidId_ReturnsBook()
    {
        Book book = Book.Create("Test Book", Genre.Fiction).Value!;
        _mockApplicationUnitOfWork
            .Setup(x => x.Books.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        HttpClient httpClient = CreateHttpClient();

        HttpResponseMessage response = await httpClient.GetAsync("/get-book/1");

        Assert.True(response.IsSuccessStatusCode);
        Result<BookResponse>? returnedBook = await response.Content.ReadFromJsonAsync<Result<BookResponse>>();
        Assert.NotNull(returnedBook?.Value);
        Assert.Equal("Test Book", returnedBook.Value!.Title);
    }

    [Fact]
    public async Task GetBookQuery_WithNotFound_ReturnsError()
    {
        _mockApplicationUnitOfWork
            .Setup(x => x.Books.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        HttpClient httpClient = CreateHttpClient();

        HttpResponseMessage response = await httpClient.GetAsync("/get-book/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}