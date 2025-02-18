namespace Application.UnitTests.Common;

public class ValidationTest
{
    [Fact]
    public async Task ValidationBehaviour_WithNoValidators_CallsNextHandler()
    {
        // Arrange
        IEnumerable<IValidator<SampleRequest>> validators = [];
        
        ValidationBehaviour<SampleRequest, SampleResponse> behavior = new(validators);
        
        SampleRequest request = new();
        
        Mock<RequestHandlerDelegate<SampleResponse>> nextHandler = new();
        
        nextHandler.Setup(n => n()).ReturnsAsync(new SampleResponse());

        // Act
        SampleResponse response = await behavior.Handle(request, nextHandler.Object, CancellationToken.None);

        // Assert
        nextHandler.Verify(n => n(), Times.Once);
        Assert.NotNull(response);
    }

    [Fact]
    public async Task ValidationBehaviour_WithValidRequest_CallsNextHandler()
    {
        // Arrange
        Mock<IValidator<SampleRequest>> validator = new();
        
        validator.Setup(v =>
                v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        
        List<IValidator<SampleRequest>> validators = [validator.Object];
        
        ValidationBehaviour<SampleRequest, SampleResponse> behavior = new(validators);
        
        SampleRequest request = new();
        Mock<RequestHandlerDelegate<SampleResponse>> nextHandler = new();
        
        nextHandler.Setup(n => n()).ReturnsAsync(new SampleResponse());

        // Act
        SampleResponse response = await behavior.Handle(request, nextHandler.Object, CancellationToken.None);

        // Assert
        nextHandler.Verify(n => n(), Times.Once);
        Assert.NotNull(response);
    }

    [Fact]
    public async Task ValidationBehaviour_WithInvalidRequest_ThrowsCommonValidationException()
    {
        // Arrange
        Mock<IValidator<SampleRequest>> validator = new();
        
        validator.Setup(v =>
                v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
                { new("Property", "Error") }));
        
        List<IValidator<SampleRequest>> validators = [validator.Object];
        
        ValidationBehaviour<SampleRequest, SampleResponse> behavior = new(validators);
        
        SampleRequest request = new();
        
        Mock<RequestHandlerDelegate<SampleResponse>> nextHandler = new();

        // Act & Assert
        await Assert.ThrowsAsync<CommonValidationException>(() =>
            behavior.Handle(request, nextHandler.Object, CancellationToken.None));
    }
}

public class SampleRequest
{
    public string Property { get; set; } = String.Empty;
}

public class SampleResponse
{
    public string Result { get; set; } = String.Empty;
}