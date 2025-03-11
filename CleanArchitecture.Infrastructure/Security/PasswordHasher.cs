using System.Globalization;
using System.Security.Cryptography;

namespace CleanArchitecture.Infrastructure.Security;

internal sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;   // 128 bit
    private const int KeySize = 32;    // 256 bit
    private const int Iterations = 100_000;

    public string HashPassword(string password)
    {
        using Rfc2898DeriveBytes algorithm = new(password, SaltSize, Iterations, HashAlgorithmName.SHA256);

        string key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
        string salt = Convert.ToBase64String(algorithm.Salt);

        return $"{Iterations}.{salt}.{key}";
    }

    public bool VerifyPassword(string password, string? hashedPassword)
    {
        if (hashedPassword is null)
        {
            return false;
        }

        string[] parts = hashedPassword.Split('.', 3);
        int iterations = int.Parse(parts[0], CultureInfo.InvariantCulture);
        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] key = Convert.FromBase64String(parts[2]);

        using Rfc2898DeriveBytes algorithm = new(password, salt, iterations, HashAlgorithmName.SHA256);
        byte[] keyToCheck = algorithm.GetBytes(KeySize);

        return keyToCheck.SequenceEqual(key);
    }
}
