namespace CleanArchitecture.Application.Entities.Persons.Queries.Get;

public record GetPersonQuery(int Id) : IRequest<Result<PersonResponse>>;