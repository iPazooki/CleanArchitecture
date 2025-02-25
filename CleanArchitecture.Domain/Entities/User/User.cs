using CleanArchitecture.Domain.Entities.Security;

namespace CleanArchitecture.Domain.Entities.User;

/// <summary>
/// Represents a user entity.
/// </summary>
public partial class User : AggregateRoot
{
    /// <summary>
    /// Gets or sets the first name of the user.
    /// </summary>
    public string FirstName { get; set; } 

    /// <summary>
    /// Gets or sets the last name of the user.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the email of the user.
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Gets or sets the password of the user.
    /// </summary>
    public string? HashedPassword { get; set; }

    /// <summary>
    /// Gets or sets the address of the user.
    /// </summary>
    public Address? Address { get; set; }

    /// <summary>
    /// Gets or sets the gender of the user.
    /// </summary>
    public Gender? Gender { get; set; }

    /// <summary>
    /// Gets or sets the roles of the user.
    /// </summary>
    public ICollection<Role> Roles { get; set; } = [];
}