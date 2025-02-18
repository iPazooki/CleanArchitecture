namespace CleanArchitecture.Application.Entities.Persons.Commands.Create;

public record CreatePersonCommand(string FirstName, string LastName, AddressRequest Address, int Gender) : IRequest<Result<int>>;