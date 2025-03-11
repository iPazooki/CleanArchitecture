namespace CleanArchitecture.Domain.Entities.Security;

public sealed class Role
{
    public int Id { get; set; }

    public required string Name { get; init; }

    public ICollection<User>? Users { get; init; }

    public ICollection<Permission> Permissions { get; init; } = [];
}
