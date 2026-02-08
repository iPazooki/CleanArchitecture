using CleanArchitecture.Application.Entities.Books.Commands.Create;
using CleanArchitecture.Application.Entities.Books.Commands.Delete;
using CleanArchitecture.Application.Entities.Books.Commands.Update;

namespace CleanArchitecture.IntegrationTests;

[Collection("DistributedApplication collection")]
public class BookIntegrationTests(DistributedApplicationFixture fixture) : BaseIntegrationTest(fixture), IAsyncLifetime
{
    private HttpClient _httpClient;

    [Fact]
    public async Task SendBookCommandWithValidRequestCreatesBook()
    {
        CreateBookCommand command = new("Test Book", "F");

        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/create-book/", command, CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task SendBookCommandWithValidRequestUpdatesBook()
    {
        CreateBookCommand createCommand = new("Initial Book", "F");
        using HttpResponseMessage createResponse = await _httpClient.PostAsJsonAsync("/create-book", createCommand, CancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        Result<Guid>? createdResult = await createResponse.Content.ReadFromJsonAsync<Result<Guid>>(CancellationToken);

        Assert.NotNull(createdResult);
        Assert.True(createdResult!.IsSuccess);

        UpdateBookCommand updateCommand = new(createdResult.Value, "Updated Book", "NF");

        using HttpResponseMessage updateResponse = await _httpClient.PutAsJsonAsync("/update-book", updateCommand, CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
    }

    [Fact]
    public async Task SendBookCommandWithValidRequestDeletesBook()
    {
        CreateBookCommand createCommand = new("Book To Delete", "F");
        using HttpResponseMessage createResponse = await _httpClient.PostAsJsonAsync("/create-book", createCommand, CancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        Result<Guid>? createdResult = await createResponse.Content.ReadFromJsonAsync<Result<Guid>>(CancellationToken);

        Assert.NotNull(createdResult);
        Assert.True(createdResult!.IsSuccess);

        DeleteBookCommand deleteCommand = new(createdResult.Value);

        using HttpResponseMessage deleteResponse = await _httpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, "/delete-book")
            {
                Content = JsonContent.Create(deleteCommand)
            },
            CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task SendBookQueryWithValidRequestGetsBook()
    {
        CreateBookCommand createCommand = new("Book To Get", "F");
        using HttpResponseMessage createResponse = await _httpClient.PostAsJsonAsync("/create-book", createCommand, CancellationToken);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        Result<Guid>? createdResult = await createResponse.Content.ReadFromJsonAsync<Result<Guid>>(CancellationToken);

        Assert.NotNull(createdResult);
        Assert.True(createdResult!.IsSuccess);

        using HttpResponseMessage getResponse = await _httpClient.GetAsync($"/get-book/{createdResult.Value}", CancellationToken);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    public async Task InitializeAsync()
    {
        _httpClient = App.CreateHttpClient("cleanarchitecture-api");

        await App.ResourceNotifications
            .WaitForResourceHealthyAsync("cleanarchitecture-api", CancellationToken)
            .WaitAsync(DefaultTimeout, CancellationToken);
    }

    public Task DisposeAsync()
    {
        _httpClient?.Dispose();

        return Task.CompletedTask;
    }
}
