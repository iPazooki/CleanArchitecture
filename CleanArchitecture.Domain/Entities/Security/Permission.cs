namespace CleanArchitecture.Domain.Entities.Security;

public sealed class Permission
{
    public required int Id { get; init; }

    public required string Name { get; init; }
}
