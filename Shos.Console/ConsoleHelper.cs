namespace Shos.Console;

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>Provides methods for console input and output.</summary>
public static partial class ConsoleHelper
{
    const int PasswordHashIterations = 600_000;
    const int PasswordHashSaltLength = 16;
    const int PasswordHashKeyLength = 32;
    const string PasswordHashAlgorithm = "shos-pbkdf2-sha256";
    const string PasswordHashVersion = "v1";

    static readonly string passwordHashIterationsText = PasswordHashIterations.ToString(CultureInfo.InvariantCulture);

    /// <summary>Reads a password from the console input and returns its salted PBKDF2-SHA256 hash.</summary>
    /// <param name="message">The message to display before reading the password.</param>
    /// <returns>A versioned hash suitable for storage and verification with <see cref="VerifyPassword"/>.</returns>
    /// <remarks>The returned hash is not compatible with values returned by earlier SHA-256-only versions.</remarks>
    public static string ReadPassword(string message)
    {
        Console.Write($"{message}: ");
        using var password = ReadSecureString();
        return HashPassword(ToPlainText(password));
    }

    /// <summary>Reads a password from the console input and verifies it against a versioned hash created by <see cref="HashPassword"/> or <see cref="ReadPassword"/>.</summary>
    /// <param name="message">The message to display before reading the password.</param>
    /// <param name="passwordHash">The versioned hash to verify against.</param>
    /// <returns><see langword="true"/> when the password matches a supported hash; otherwise, <see langword="false"/>.</returns>
    public static bool ReadAndVerifyPassword(string message, string passwordHash)
    {
        ArgumentNullException.ThrowIfNull(passwordHash);

        Console.Write($"{message}: ");
        using var password = ReadSecureString();
        return VerifyPassword(ToPlainText(password), passwordHash);
    }

    /// <summary>Creates a salted PBKDF2-SHA256 hash for the specified password.</summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>A versioned hash suitable for storage and verification with <see cref="VerifyPassword"/>.</returns>
    /// <remarks>Hashes from earlier SHA-256-only versions are not supported by <see cref="VerifyPassword"/>.</remarks>
    public static string HashPassword(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        var salt = RandomNumberGenerator.GetBytes(PasswordHashSaltLength);
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[]? derivedKey = null;
        try {
            derivedKey = Rfc2898DeriveBytes.Pbkdf2(passwordBytes, salt, PasswordHashIterations, HashAlgorithmName.SHA256, PasswordHashKeyLength);
            return $"{PasswordHashAlgorithm}${PasswordHashVersion}${passwordHashIterationsText}${Convert.ToBase64String(salt)}${Convert.ToBase64String(derivedKey)}";
        } finally {
            CryptographicOperations.ZeroMemory(passwordBytes);
            if (derivedKey is not null)
                CryptographicOperations.ZeroMemory(derivedKey);
            CryptographicOperations.ZeroMemory(salt);
        }
    }

    /// <summary>Verifies a password against a versioned hash created by <see cref="HashPassword"/>.</summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="passwordHash">The versioned hash to verify against.</param>
    /// <returns><see langword="true"/> when the password matches a supported hash; otherwise, <see langword="false"/>.</returns>
    public static bool VerifyPassword(string password, string passwordHash)
    {
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(passwordHash);

        if (passwordHash.Split('$') is not [PasswordHashAlgorithm, PasswordHashVersion, var iterationsText, var saltText, var expectedKeyText] ||
            iterationsText != passwordHashIterationsText)
            return false;

        byte[] salt;
        byte[] expectedKey;
        try {
            salt = Convert.FromBase64String(saltText);
            expectedKey = Convert.FromBase64String(expectedKeyText);
        } catch (FormatException) {
            return false;
        }

        if (salt.Length != PasswordHashSaltLength || expectedKey.Length != PasswordHashKeyLength) {
            CryptographicOperations.ZeroMemory(salt);
            CryptographicOperations.ZeroMemory(expectedKey);
            return false;
        }

        var passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[]? actualKey = null;
        try {
            actualKey = Rfc2898DeriveBytes.Pbkdf2(passwordBytes, salt, PasswordHashIterations, HashAlgorithmName.SHA256, PasswordHashKeyLength);
            return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
        } finally {
            CryptographicOperations.ZeroMemory(passwordBytes);
            CryptographicOperations.ZeroMemory(salt);
            CryptographicOperations.ZeroMemory(expectedKey);
            if (actualKey is not null)
                CryptographicOperations.ZeroMemory(actualKey);
        }
    }

    /// <summary>Reads a SecureString from the console input.</summary>
    /// <returns>The SecureString read from the console input.</returns>
    static SecureString ReadSecureString()
    {
        var password = new SecureString();
        while (Add(password, Console.ReadKey(true)))
            ;
        password.MakeReadOnly();
        return password;
    }

    /// <summary>Adds a character to the SecureString password based on the console input.</summary>
    /// <param name="password">The SecureString to add the character to.</param>
    /// <param name="input">The console key input.</param>
    /// <returns>True if the input is not Enter; otherwise, false.</returns>
    static bool Add(SecureString password, ConsoleKeyInfo input)
    {
        switch (input.Key) {
            case ConsoleKey.Enter:
                Console.WriteLine();
                return false;
            case ConsoleKey.Backspace:
                if (password.Length > 0) {
                    Console.Write("\b \b");
                    password.RemoveAt(password.Length - 1);
                }
                break;
            case ConsoleKey when PasswordValidator.IsCharacterAllowed(input.KeyChar):
                Console.Write('*');
                password.AppendChar(input.KeyChar);
                break;
        }
        return true;
    }

    /// <summary>Converts a SecureString password to plain text.</summary>
    /// <param name="password">The SecureString password.</param>
    /// <returns>The plain-text password.</returns>
    static string ToPlainText(SecureString password)
    {
        var pointer = Marshal.SecureStringToBSTR(password);
        try {
            return Marshal.PtrToStringBSTR(pointer) ?? "";
        } finally {
            Marshal.ZeroFreeBSTR(pointer);
        }
    }

    /// <summary>Provides methods for validating password characters.</summary>
    static partial class PasswordValidator
    {
        /// <summary>Determines whether the specified character is allowed in the password.</summary>
        /// <param name="this">The character to validate.</param>
        /// <returns>True if the character is allowed; otherwise, false.</returns>
        public static bool IsCharacterAllowed(char @this)
            => AllowedCharactersRegex().IsMatch(@this.ToString());

        // Defines the pattern of characters available.
        [GeneratedRegex(@"^[a-zA-Z0-9!@#$%^&*]+$")]
        private static partial Regex AllowedCharactersRegex();
    }
}
