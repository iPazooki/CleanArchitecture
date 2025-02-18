namespace CleanArchitecture.Application.Common.Exceptions;

/// <summary>
/// Represents an exception that occurs when one or more validation failures have occurred.
/// </summary>
public class CommonValidationException() : Exception("One or more validation failures have occurred.")
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommonValidationException"/> class with the specified validation failures.
    /// </summary>
    /// <param name="failures">The collection of validation failures.</param>
    public CommonValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        
    }

    /// <summary>
    /// Gets the dictionary of validation errors, where the key is the property name and the value is an array of error messages.
    /// </summary>
    public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();
}