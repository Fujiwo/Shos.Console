namespace Shos.Console
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>Provides methods for console input and output.</summary>
    public static class ConsoleHelper
    {
        /// <summary>Reads a password from the console input and returns its hashed value.</summary>
        /// <param name="message">The message to display before reading the password.</param>
        /// <returns>The hashed value of the entered password.</returns>
        public static string ReadPassword(string message)
        {
            Console.Write($"{message}: ");
            using SecureString password = ReadSecureString();
            return ToHashString(password);
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
                        Console.Write('\b');
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

        /// <summary>Converts a SecureString password to its hashed string representation.</summary>
        /// <param name="password">The SecureString password.</param>
        /// <returns>The hashed string representation of the password.</returns>
        static string ToHashString(SecureString password)
        {
            var pointer = Marshal.SecureStringToBSTR(password);
            var result = ToHashString(Marshal.PtrToStringAuto(pointer) ?? "");
            Marshal.ZeroFreeBSTR(pointer);
            return result;
        }

        /// <summary>Converts a plain text string to its hashed string representation.</summary>
        /// <param name="text">The plain text string.</param>
        /// <returns>The hashed string representation of the text.</returns>
        static string ToHashString(string text)
        {
            var byteArray = Encoding.UTF8.GetBytes(text);
            using var sha256 = SHA256.Create();
            var hashValue = sha256.ComputeHash(byteArray);
            sha256.Clear();

            var stringBuilder = new StringBuilder();
            foreach (byte eachByte in hashValue)
                stringBuilder.Append(eachByte.ToString("x2"));
            return stringBuilder.ToString();
        }

        /// <summary>Provides methods for validating password characters.</summary>
        static class PasswordValidator
        {
            // Defines the pattern of characters available.
            const string allowedCharactersPattern = @"^[a-zA-Z0-9!@#$%^&*]+$";

            /// <summary>Determines whether the specified character is allowed in the password.</summary>
            /// <param name="this">The character to validate.</param>
            /// <returns>True if the character is allowed; otherwise, false.</returns>
            public static bool IsCharacterAllowed(char @this)
                => Regex.IsMatch(@this.ToString(), allowedCharactersPattern);
        }
    }
}
