using CleanArchitecture.Domain.Entities.Security;

namespace CleanArchitecture.Domain.Entities;

public partial class User
{
    private User() { }

    public static Result<User> Create(string firstName, string lastName, string? email = null, string? password = null, Address? address = null, Gender? gender = null)
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

        User user = new()
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            HashedPassword = password,
            Address = address,
            Gender = gender
        };

        return Result<User>.Success(user);
    }

    public void AddRole(Role role) => Roles.Add(role);
}
