namespace CleanArchitecture.Domain.Entities;

/// <summary>
/// Represents a user entity.
/// </summary>
public partial class User : AggregateRoot
{
    /// <summary>
    /// Gets or sets the first name of the user.
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// Gets or sets the last name of the user.
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    /// Gets or sets the email of the user.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Gets or sets the password of the user.
    /// </summary>
    public string? HashedPassword { get; init; }

    /// <summary>
    /// Gets or sets the address of the user.
    /// </summary>
    public Address? Address { get; init; }

    /// <summary>
    /// Gets or sets the gender of the user.
    /// </summary>
    public Gender? Gender { get; init; }
}
