namespace CleanArchitecture.Application.Entities.Persons.Queries.Get;

public record PersonResponse(int Id, string FirstName, string LastName, AddressResponse? Address);

public record AddressResponse(string? City, string? Street, string? PostalCode);