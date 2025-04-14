namespace CleanArchitecture.Domain.Entities;

public partial class RefreshToken
{
    private RefreshToken() { }

    public static Result<RefreshToken> Create(string token, User user, DateTimeOffset expiresDate)
    {
        HashSet<Error> errors = [];

        if (string.IsNullOrWhiteSpace(token))
        {
            errors.Add(new Error("Token cannot be empty."));
        }

        if (expiresDate <= DateTimeOffset.UtcNow)
        {
            errors.Add(new Error("Expiration date must be in the future."));
        }

        if (errors.Count != 0)
        {
            return Result<RefreshToken>.Failure(errors.ToArray());
        }

        return Result<RefreshToken>.Success(new RefreshToken
        {
            Token = token,
            User = user,
            UserId = user.Id,
            ExpiresDate = expiresDate
        });
    }

}
