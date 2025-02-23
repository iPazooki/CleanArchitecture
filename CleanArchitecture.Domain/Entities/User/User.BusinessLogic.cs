namespace CleanArchitecture.Domain.Entities.User;

public partial class User
{
    private User(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static Result<User> Create(string firstName, string lastName)
    {
        HashSet<Error> errors = [];

        if (string.IsNullOrWhiteSpace(firstName))
        {
            errors.Add(UserErrors.FirstNameIsRequired);
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            errors.Add(UserErrors.LastNameIsRequired);
        }

        if (errors.Count != 0)
        {
            return Result<User>.Failure(errors.ToArray());
        }

        User user = new(firstName, lastName);

        return Result<User>.Success(user);
    }

    public static Result<User> Create(string firstName, string lastName, Address? address, Gender? gender)
    {
        Result<User> result = Create(firstName, lastName);

        if (!result.IsSuccess)
        {
            return result;
        }

        User user = result.Value!;

        user.Address = address;
        user.Gender = gender;

        return Result<User>.Success(user);
    }

    public static Result<User> Create(string firstName, string lastName, string? email, string? password, Address? address, Gender? gender)
    {
        Result<User> result = Create(firstName, lastName, address, gender);

        if (!result.IsSuccess)
        {
            return result;
        }

        User user = result.Value!;

        user.Email = email;
        user.HashedPassword = password;

        return Result<User>.Success(user);
    }
}