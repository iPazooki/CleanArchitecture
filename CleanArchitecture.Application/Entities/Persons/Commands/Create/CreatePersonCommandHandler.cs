using CleanArchitecture.Domain.Entities.Person;

namespace CleanArchitecture.Application.Entities.Persons.Commands.Create;

public class CreatePersonCommandHandler(IApplicationUnitOfWork applicationUnitOfWork) : IRequestHandler<CreatePersonCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        Address address = new(request.Address.City, request.Address.Street, request.Address.PostalCode);

        Result<Person> person = Person.Create(request.FirstName, request.LastName, address, (Gender)request.Gender);

        if (!person.IsSuccess)
        {
            return Result<int>.Failure(person.Errors.ToArray());
        }

        await applicationUnitOfWork.Persons.AddAsync(person.Value!, cancellationToken);

        Result result = await applicationUnitOfWork.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? Result<int>.Success(person.Value!.Id)
            : Result<int>.Failure(string.Format(GeneralErrors.GeneralCreateErrorMessage, nameof(Person)));
    }
}