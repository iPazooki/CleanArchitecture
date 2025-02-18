namespace CleanArchitecture.Domain.Entities.Person;

public partial class Person
{
    private Person(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static Result<Person> Create(string firstName, string lastName)
    {
        HashSet<Error> errors = [];

        if (string.IsNullOrWhiteSpace(firstName))
        {
            errors.Add(PersonErrors.FirstNameIsRequired);
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            errors.Add(PersonErrors.LastNameIsRequired);
        }

        if (errors.Count != 0)
        {
            return Result<Person>.Failure(errors.ToArray());
        }

        Person person = new(firstName, lastName);

        return Result<Person>.Success(person);
    }

    public static Result<Person> Create(string firstName, string lastName, Address? address, Gender? gender)
    {
        Result<Person> result = Create(firstName, lastName);

        if (!result.IsSuccess)
        {
            return result;
        }

        Person person = result.Value!;

        person.Address = address;
        person.Gender = gender;

        return Result<Person>.Success(person);
    }
}