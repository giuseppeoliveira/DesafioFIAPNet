using System.Security.Cryptography;

namespace SchoolAPI.Application;

public interface ICryptographyService
{
    (byte[] salt, byte[] cipherPassword) PasswordEncypt(string plainTextPassword);
    bool ComparePassword(string plainTextComparePassword, byte[] cipherActualPassword, byte[] salt);
}

public class CryptographyService : ICryptographyService
{
    private const int keySize = 256 / 8;
    private const int iterations = 600000;
    private static readonly HashAlgorithmName hashAlgorithmName = HashAlgorithmName.SHA256;

    public (byte[] salt, byte[] cipherPassword) PasswordEncypt(string plainTextPassword)
    {
        var salt = RandomNumberGenerator.GetBytes(keySize);

        var cipherPassword = Rfc2898DeriveBytes.Pbkdf2(plainTextPassword, salt, iterations, hashAlgorithmName, keySize);

        return (salt, cipherPassword);
    }

    public bool ComparePassword(string plainTextComparePassword, byte[] cipherActualPassword, byte[] salt)
    {
        var compareCriptoKey = Rfc2898DeriveBytes.Pbkdf2(plainTextComparePassword, salt, iterations, hashAlgorithmName, keySize);

        return compareCriptoKey.SequenceEqual(cipherActualPassword);
    }
}