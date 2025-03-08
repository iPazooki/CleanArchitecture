namespace CleanArchitecture.Application.Common.Errors;

/// <summary>
/// Represents an exception that occurs when one or more validation failures have occurred.
/// </summary>
public sealed class ApplicationValidationError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationValidationError"/> class with the specified validation failures.
    /// </summary>
    /// <param name="failures">The collection of validation failures.</param>
    public ApplicationValidationError(IEnumerable<ValidationFailure> failures)
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