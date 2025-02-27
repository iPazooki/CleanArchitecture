namespace Application.UnitTests.Common.Exceptions;

public class ApplicationValidationExceptionTests
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
        ApplicationValidationException exception = new(failures);

        // Assert
        Assert.Equal(2, exception.Errors.Count);
        Assert.Contains("Property1", exception.Errors.Keys);
        Assert.Contains("Property2", exception.Errors.Keys);
        Assert.Equal(["Error1", "Error2"], exception.Errors["Property1"]);
        Assert.Equal(["Error3"], exception.Errors["Property2"]);
    }

    [Fact]
    public void CommonValidationException_WithNoFailures_SetsEmptyErrorsProperty()
    {
        // Arrange
        List<ValidationFailure> failures = [];
        
        // Act
        ApplicationValidationException exception = new(failures);

        // Assert
        Assert.Empty(exception.Errors);
    }
}