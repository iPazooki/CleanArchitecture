using CleanArchitecture.Domain.Entities.Person;

namespace Domain.UnitTests;

public class PersonTests
{
    [Fact]
    public void Person_Create_ShouldReturnSuccess_WhenValidData()
    {
        // Arrange & Act
        Result<Person> result = Person.Create("John", "Doe");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("John", result.Value.FirstName);
        Assert.Equal("Doe", result.Value.LastName);
    }

    [Fact]
    public void Person_Create_ShouldReturnFailure_WhenFirstNameIsEmpty()
    {
        // Arrange & Act
        Result<Person> result = Person.Create("", "Doe");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(PersonErrors.FirstNameIsRequired, result.Errors);
    }

    [Fact]
    public void Person_Create_ShouldReturnFailure_WhenLastNameIsWhitespace()
    {
        // Arrange & Act
        Result<Person> result = Person.Create("John", "   ");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(PersonErrors.LastNameIsRequired, result.Errors);
    }
}