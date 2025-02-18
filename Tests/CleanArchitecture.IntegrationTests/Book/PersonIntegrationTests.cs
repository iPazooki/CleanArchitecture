using CleanArchitecture.Application.Entities.Persons.Queries.Get;
using CleanArchitecture.Application.Entities.Persons.Commands.Create;

namespace CleanArchitecture.IntegrationTests.Book;

public class PersonIntegrationTests : BaseIntegrationTest
{
    private async Task<int> CreatePersonAsync(string firstName, string lastName)
    {
        AddressRequest address = new("City", "Street", "PostalCode");
        CreatePersonCommand command = new(firstName, lastName, address, (int)Gender.Male);
        Result<int> result = await Sender.Send(command);
        return result.Value;
    }

    [Fact]
    public async Task CreatePersonCommand_WithValidRequest_CreatesPerson()
    {
        int personId = await CreatePersonAsync("Test", "Person");
        Assert.NotEqual(0, personId);
    }

    [Fact]
    public async Task GetPersonQuery_WithValidRequest_ReturnsPerson()
    {
        int personId = await CreatePersonAsync("Another", "Person");
        Result<PersonResponse> result = await Sender.Send(new GetPersonQuery(personId));

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("Another Person",  $"{result.Value!.FirstName} {result.Value!.LastName}");
    }

    [Fact]
    public async Task CreatePersonCommand_WithEmptyName_ThrowsException()
    {
        await Assert.ThrowsAsync<CommonValidationException>(
            () => CreatePersonAsync(" ", string.Empty));
    }

    [Fact]
    public async Task GetPersonQuery_WithNotFound_ReturnsFailure()
    {
        Result<PersonResponse> result = await Sender.Send(new GetPersonQuery(9999));

        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
    }
}