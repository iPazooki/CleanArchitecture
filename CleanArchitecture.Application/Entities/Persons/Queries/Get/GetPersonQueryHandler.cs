using CleanArchitecture.Domain.Entities.Person;

namespace CleanArchitecture.Application.Entities.Persons.Queries.Get;

public class GetPersonQueryHandler(IApplicationUnitOfWork applicationUnitOfWork) : IRequestHandler<GetPersonQuery, Result<PersonResponse>>
{
    public async Task<Result<PersonResponse>> Handle(GetPersonQuery request, CancellationToken cancellationToken)
    {
        Person? person = await applicationUnitOfWork.Persons.FindAsync(keyValues: [request.Id], cancellationToken);

        return person is null
            ? Result<PersonResponse>.Failure("Person Not Found.")
            : Result<PersonResponse>.Success(person.ToResponse());
    }
}

public static class PersonExtensions
{
    public static PersonResponse ToResponse(this Person person) =>
        new(person.Id, person.FirstName, person.LastName, new AddressResponse(person.Address?.City, person.Address?.Street, person.Address?.PostalCode));
}