using CleanArchitecture.Application.Entities.Books.Commands.Create;
using CleanArchitecture.Application.Entities.Books.Commands.Delete;
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

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task SendBookCommandWithValidRequestUpdatesBook()
    {
        CreateBookCommand createCommand = new("Initial Book", "F");
        using HttpResponseMessage createResponse = await _httpClient.PostAsJsonAsync("/api/v1/books", createCommand, CancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        Result<Guid>? createdResult = await createResponse.Content.ReadFromJsonAsync<Result<Guid>>(CancellationToken);

        Assert.NotNull(createdResult);
        Assert.True(createdResult!.IsSuccess);

        UpdateBookCommand updateCommand = new(createdResult.Value, "Updated Book", "NF");

        using HttpResponseMessage updateResponse = await _httpClient.PutAsJsonAsync($"/api/v1/books/{createdResult.Value}", updateCommand, CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        using HttpResponseMessage getResponse = await _httpClient.GetAsync($"/api/v1/books/{createdResult.Value}", CancellationToken);
        Result<BookResponse>? getResult = await getResponse.Content.ReadFromJsonAsync<Result<BookResponse>>(CancellationToken);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(getResult);
        Assert.True(getResult!.IsSuccess);
        Assert.Equal("Updated Book", getResult.Value!.Title);
        Assert.Equal("NF", getResult.Value.Genre);
    }

    [Fact]
    public async Task SendBookCommandWithValidRequestDeletesBook()
    {
        CreateBookCommand createCommand = new("Book To Delete", "F");
        using HttpResponseMessage createResponse = await _httpClient.PostAsJsonAsync("/api/v1/books", createCommand, CancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        Result<Guid>? createdResult = await createResponse.Content.ReadFromJsonAsync<Result<Guid>>(CancellationToken);

        Assert.NotNull(createdResult);
        Assert.True(createdResult!.IsSuccess);

        using HttpResponseMessage deleteResponse = await _httpClient.DeleteAsync($"/api/v1/books/{createdResult.Value}", CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task SendBookQueryWithValidRequestGetsBook()
    {
        CreateBookCommand createCommand = new("Book To Get", "F");
        using HttpResponseMessage createResponse = await _httpClient.PostAsJsonAsync("/api/v1/books", createCommand, CancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        Result<Guid>? createdResult = await createResponse.Content.ReadFromJsonAsync<Result<Guid>>(CancellationToken);

        Assert.NotNull(createdResult);
        Assert.True(createdResult!.IsSuccess);

        using HttpResponseMessage getResponse = await _httpClient.GetAsync($"/api/v1/books/{createdResult.Value}", CancellationToken);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
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

    [Fact]
    public async Task SendBookCommandWithInvalidGenreReturnsUnprocessableEntity()
    {
        CreateBookCommand command = new("Invalid Genre Book", "X");

        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/v1/books", command, CancellationToken);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
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
