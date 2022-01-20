using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace HookHook.Backend.Utilities
{
    /// <summary>
    /// Utility to hash and check users passwords
    /// </summary>
    public static class PasswordHash
    {
        /// <summary>
        /// Hashed salt password size
        /// </summary>
        private const int SaltSize = 128 / 8;
        /// <summary>
        /// Number of hashing iteration
        /// </summary>
        private const int Iterations = 10000;
        /// <summary>
        /// Final password hash size
        /// </summary>
        private const int HashSize = 256 / 8;
        /// <summary>
        /// Type of algorithm used to hash
        /// </summary>
        private const KeyDerivationPrf Prf = KeyDerivationPrf.HMACSHA256;

        /// <summary>
        /// Hash clear password
        /// </summary>
        /// <param name="password">User password</param>
        /// <returns>Hashed password</returns>
        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, Prf, Iterations, HashSize);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01;
            WriteByte(outputBytes, 1, (uint)Prf);
            WriteByte(outputBytes, 5, Iterations);
            WriteByte(outputBytes, 9, SaltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + SaltSize, subkey.Length);

            return Convert.ToBase64String(outputBytes);
        }

        /// <summary>
        /// Check if user password equals database password
        /// </summary>
        /// <param name="password">User password</param>
        /// <param name="hashed">Database password</param>
        /// <returns>true if they are equals</returns>
        public static bool VerifyPassword(string password, string hashed)
        {
            byte[] decodedHashed = Convert.FromBase64String(hashed);

            if (decodedHashed[0] != 0x01)
                return false;

            KeyDerivationPrf prf = (KeyDerivationPrf)ReadByte(decodedHashed, 1);
            int iterations = (int)ReadByte(decodedHashed, 5);
            int saltLength = (int)ReadByte(decodedHashed, 9);

            if (saltLength != SaltSize)
                return false;

            byte[] salt = new byte[saltLength];
            Buffer.BlockCopy(decodedHashed, 13, salt, 0, salt.Length);

            int subkeyLength = decodedHashed.Length - 13 - salt.Length;
            if (subkeyLength != HashSize)
                return false;

            byte[] expected = new byte[subkeyLength];
            Buffer.BlockCopy(decodedHashed, 13 + salt.Length, expected, 0, expected.Length);

            byte[] actual = KeyDerivation.Pbkdf2(password, salt, prf, iterations, subkeyLength);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }

        /// <summary>
        /// Write a Byte into a buffer
        /// </summary>
        /// <param name="buffer">Byte buffer</param>
        /// <param name="offset">Where to write</param>
        /// <param name="value">Value to write</param>
        private static void WriteByte(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }

        /// <summary>
        /// Read Byte from buffer
        /// </summary>
        /// <param name="buffer">Byte buffer</param>
        /// <param name="offset">Where to read</param>
        /// <returns></returns>
        private static uint ReadByte(byte[] buffer, int offset)
        {
            return ((uint)buffer[offset + 0] << 24)
                | ((uint)buffer[offset + 1] << 16)
                | ((uint)buffer[offset + 2] << 8)
                | (buffer[offset + 3]);
        }
    }
}