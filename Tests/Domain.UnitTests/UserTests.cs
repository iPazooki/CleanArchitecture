using User = CleanArchitecture.Domain.Entities.User.User;

namespace Domain.UnitTests;

public class UserTests
{
    [Fact]
    public void User_Create_ShouldReturnSuccess_WhenValidData()
    {
        // Arrange & Act
        Result<User> result = User.Create("John", "Doe");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("John", result.Value.FirstName);
        Assert.Equal("Doe", result.Value.LastName);
    }
    
    [Fact]
    public void User_Create_ShouldReturnSuccess_WhenValidDataAddressGender()
    {
        // Arrange & Act
        Result<User> result = User.Create("John", "Doe", new Address("City", "Street", "Postal"), Gender.Male);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("John", result.Value.FirstName);
        Assert.Equal("Doe", result.Value.LastName);
        Assert.Equal("City", result.Value.Address!.City);
        Assert.Equal("Street", result.Value.Address!.Street);
        Assert.Equal("Postal", result.Value.Address!.PostalCode);
        Assert.Equal(Gender.Male, result.Value.Gender);
    }

    [Fact]
    public void User_Create_ShouldReturnFailure_WhenFirstNameIsEmpty()
    {
        // Arrange & Act
        Result<User> result = User.Create("", "Doe");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(UserErrors.FirstNameIsRequired, result.Errors);
    }
    
    [Fact]
    public void User_Create_ShouldReturnSuccess_WhenValidDataWithEmailPassword()
    {
        // Arrange & Act
        Result<User> result = User.Create(
            "John",
            "Doe",
            "john@example.com",
            "myPassword",
            new Address("City", "Street", "Postal"),
            Gender.Male
        );

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("John", result.Value.FirstName);
        Assert.Equal("Doe", result.Value.LastName);
        Assert.Equal("john@example.com", result.Value.Email);
        Assert.Equal("myPassword", result.Value.HashedPassword);
        Assert.Equal("City", result.Value.Address!.City);
        Assert.Equal("Street", result.Value.Address!.Street);
        Assert.Equal("Postal", result.Value.Address!.PostalCode);
        Assert.Equal(Gender.Male, result.Value.Gender);
    }

    [Fact]
    public void User_Create_ShouldReturnFailure_WhenLastNameIsWhitespace()
    {
        // Arrange & Act
        Result<User> result = User.Create("John", "   ");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(UserErrors.LastNameIsRequired, result.Errors);
    }
}