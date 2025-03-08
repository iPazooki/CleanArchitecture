namespace Application.UnitTests.Common.Exceptions;

public class ApplicationValidationErrorTests
{
    [Fact]
    public void CommonValidationException_WithFailures_SetsErrorsProperty()
    {
        // Arrange
        List<ValidationFailure> failures =
        [
            new("Property1", "Error1"),
            new("Property1", "Error2"),
            new("Property2", "Error3")
        ];

        // Act
        ApplicationValidationError error = new(failures);

        // Assert
        Assert.Equal(2, error.Errors.Count);
        Assert.Contains("Property1", error.Errors.Keys);
        Assert.Contains("Property2", error.Errors.Keys);
        Assert.Equal(["Error1", "Error2"], error.Errors["Property1"]);
        Assert.Equal(["Error3"], error.Errors["Property2"]);
    }

    [Fact]
    public void CommonValidationException_WithNoFailures_SetsEmptyErrorsProperty()
    {
        // Arrange
        List<ValidationFailure> failures = [];
        
        // Act
        ApplicationValidationError error = new(failures);

        // Assert
        Assert.Empty(error.Errors);
    }
}