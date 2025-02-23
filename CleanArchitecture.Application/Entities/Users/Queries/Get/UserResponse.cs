namespace CleanArchitecture.Application.Entities.Users.Queries.Get;

public record UserResponse(Guid Id, string FirstName, string LastName, AddressResponse? Address);

public record AddressResponse(string? City, string? Street, string? PostalCode);