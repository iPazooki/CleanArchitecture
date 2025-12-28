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
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
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

        byte[] keyToCheck = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            KeySize);

        return keyToCheck.SequenceEqual(key);
    }
}
