using CleanArchitecture.Application.Entities.Users.Queries.Get;
using CleanArchitecture.Application.Entities.Users.Commands.Create;

namespace CleanArchitecture.IntegrationTests;

public class UserIntegrationTests : BaseIntegrationTest
{
    private async Task<Guid> CreateUserAsync(string firstName, string lastName, string email = "", string password = "")
    {
        AddressRequest address = new("City", "Street", "PostalCode");
        CreateUserCommand command = new(firstName, lastName, email, password, address, (int)Gender.Male);
        Result<Guid> result = await Sender.Send(command);
        return result.Value;
    }

    [Fact]
    public async Task CreateUserCommand_WithValidRequest_CreatesUser()
    {
        Guid userId = await CreateUserAsync("Test", "User");
        Assert.NotEqual(Guid.Empty, userId);
    }

    [Fact]
    public async Task GetUserQuery_WithValidRequest_ReturnsUser()
    {
        Guid userId = await CreateUserAsync("Another", "User");
        Result<UserResponse> result = await Sender.Send(new GetUserQuery(userId));

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("Another User",  $"{result.Value!.FirstName} {result.Value!.LastName}");
    }

    [Fact]
    public async Task CreateUserCommand_WithEmptyName_ThrowsException()
    {
        await Assert.ThrowsAsync<ApplicationValidationException>(
            () => CreateUserAsync(" ", string.Empty));
    }

    [Fact]
    public async Task GetUserQuery_WithNotFound_ReturnsFailure()
    {
        Result<UserResponse> result = await Sender.Send(new GetUserQuery(Guid.NewGuid()));

        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
    }
    
    [Fact]
    public async Task CreateUserCommand_WithEmailWithoutPassword_ThrowsException()
    {
        await Assert.ThrowsAsync<ApplicationValidationException>(
            () => CreateUserAsync("Test", "User", "test@example.com", ""));
    }

    [Fact]
    public async Task CreateUserCommand_WithPasswordLessThanSixCharacters_ThrowsException()
    {
        await Assert.ThrowsAsync<ApplicationValidationException>(
            () => CreateUserAsync("Test", "User", "test@example.com", "12345"));
    }
}