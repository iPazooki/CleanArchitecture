namespace CleanArchitecture.Domain.Entities.Security;

public sealed class Permission
{
    public required int Id { get; set; }

    public required string Name { get; init; }
}