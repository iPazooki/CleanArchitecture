namespace CleanArchitecture.Domain.Entities;

/// <summary>
/// Represents a person entity.
/// </summary>
/// <param name="firstName">The first name of the person.</param>
/// <param name="lastName">The last name of the person.</param>
public class Person(string firstName, string lastName) : BaseEntity
{
    /// <summary>
    /// Gets or sets the first name of the person.
    /// </summary>
    public string FirstName { get; set; } = firstName;

    /// <summary>
    /// Gets or sets the last name of the person.
    /// </summary>
    public string LastName { get; set; } = lastName;

    /// <summary>
    /// Gets or sets the address of the person.
    /// </summary>
    public Address? Address { get; set; }

    /// <summary>
    /// Gets or sets the gender of the person.
    /// </summary>
    public Gender? Gender { get; set; }
}