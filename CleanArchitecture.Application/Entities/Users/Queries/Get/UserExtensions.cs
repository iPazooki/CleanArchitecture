using CleanArchitecture.Domain.Entities.User;

namespace CleanArchitecture.Application.Entities.Users.Queries.Get;

internal static class UserExtensions
{
    public static UserResponse ToResponse(this User user) =>
        new(user.Id, user.FirstName, user.LastName, new AddressResponse(user.Address?.City, user.Address?.Street, user.Address?.PostalCode));
}