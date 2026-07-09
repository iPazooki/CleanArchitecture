using CleanArchitecture.Application.Entities.Books.Commands.Create;
using CleanArchitecture.Application.Entities.Books.Commands.Update;
using CleanArchitecture.Application.Entities.Books.Queries.Get;

namespace CleanArchitecture.IntegrationTests;

[Collection("DistributedApplication collection")]
public class BookIntegrationTests(DistributedApplicationFixture fixture) : BaseIntegrationTest(fixture), IAsyncLifetime
{
    private HttpClient _httpClient;

    [Fact]
    public async Task SendBookCommandWithValidRequestCreatesBook()
    {
        CreateBookCommand command = new("Test Book", "F");

        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/v1/books", command, CancellationToken);
        Guid createdId = await response.Content.ReadFromJsonAsync<Guid>(CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotEqual(Guid.Empty, createdId);
    }

    [Fact]
    public async Task SendBookCommandWithValidRequestUpdatesBook()
    {
        Guid createdId = await CreateBookAsync("Initial Book", "F");

        UpdateBookCommand updateCommand = new(createdId, "Updated Book", "NF");

        using HttpResponseMessage updateResponse = await _httpClient.PutAsJsonAsync($"/api/v1/books/{createdId}", updateCommand, CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        using HttpResponseMessage getResponse = await _httpClient.GetAsync($"/api/v1/books/{createdId}", CancellationToken);
        BookResponse? book = await getResponse.Content.ReadFromJsonAsync<BookResponse>(CancellationToken);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(book);
        Assert.Equal("Updated Book", book.Title);
        Assert.Equal("NF", book.Genre);
    }

    [Fact]
    public async Task SendBookCommandWithValidRequestDeletesBook()
    {
        Guid createdId = await CreateBookAsync("Book To Delete", "F");

        using HttpResponseMessage deleteResponse = await _httpClient.DeleteAsync($"/api/v1/books/{createdId}", CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task SendBookQueryWithValidRequestGetsBook()
    {
        Guid createdId = await CreateBookAsync("Book To Get", "F");

        using HttpResponseMessage getResponse = await _httpClient.GetAsync($"/api/v1/books/{createdId}", CancellationToken);
        BookResponse? book = await getResponse.Content.ReadFromJsonAsync<BookResponse>(CancellationToken);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(book);
        Assert.Equal(createdId, book.Id);
        Assert.Equal("Book To Get", book.Title);
        Assert.Equal("F", book.Genre);
    }

    [Fact]
    public async Task SendBookQueryWithUnknownIdReturnsNotFound()
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"/api/v1/books/{Guid.NewGuid()}", CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SendBookCommandWithUnknownIdReturnsNotFoundOnUpdate()
    {
        UpdateBookCommand command = new(Guid.NewGuid(), "Updated Book", "NF");

        using HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"/api/v1/books/{command.Id}", command, CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SendBookCommandWithUnknownIdReturnsNotFoundOnDelete()
    {
        using HttpResponseMessage response = await _httpClient.DeleteAsync($"/api/v1/books/{Guid.NewGuid()}", CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>Domain error path: Genre.FromCode rejects the code.</summary>
    [Fact]
    public async Task SendBookCommandWithInvalidGenreReturnsUnprocessableEntity()
    {
        CreateBookCommand command = new("Invalid Genre Book", "X");

        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/v1/books", command, CancellationToken);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    /// <summary>
    /// FluentValidation path, which used to answer 400 because its errors carried no code.
    /// </summary>
    [Fact]
    public async Task SendBookCommandWithTooShortTitleReturnsUnprocessableEntity()
    {
        CreateBookCommand command = new("ab", "F");

        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/v1/books", command, CancellationToken);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    private async Task<Guid> CreateBookAsync(string title, string genre)
    {
        CreateBookCommand command = new(title, genre);

        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/v1/books", command, CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        return await response.Content.ReadFromJsonAsync<Guid>(CancellationToken);
    }

    public async ValueTask InitializeAsync()
    {
        _httpClient = App.CreateHttpClient("cleanarchitecture-api");

        await App.ResourceNotifications
            .WaitForResourceHealthyAsync("cleanarchitecture-api", CancellationToken)
            .WaitAsync(DefaultTimeout, CancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        _httpClient?.Dispose();

        return ValueTask.CompletedTask;
    }
}
