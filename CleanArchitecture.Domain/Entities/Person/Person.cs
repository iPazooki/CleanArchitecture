namespace CleanArchitecture.Domain.Entities.Person;

/// <summary>
/// Represents a person entity.
/// </summary>
public partial class Person : AggregateRoot
{
    /// <summary>
    /// Gets or sets the first name of the person.
    /// </summary>
    public string FirstName { get; set; } 

    /// <summary>
    /// Gets or sets the last name of the person.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the address of the person.
    /// </summary>
    public Address? Address { get; set; }

    /// <summary>
    /// Gets or sets the gender of the person.
    /// </summary>
    public Gender? Gender { get; set; }
}