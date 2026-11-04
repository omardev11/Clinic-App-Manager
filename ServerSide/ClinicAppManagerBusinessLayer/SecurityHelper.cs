using Konscious.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicAppManagerBusinessLayer
{
    public static class SecurityHelper
    {
        public enum EnMode { AddNewMode, UpdateMode, DeleteMode }

        public static string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[16];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Hash the password using Argon2id
            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = 8; // Number of threads
                argon2.MemorySize = 65536; // 64 MB memory usage
                argon2.Iterations = 4; // Number of iterations

                // Generate the hash
                byte[] hash = argon2.GetBytes(32); // Output hash size in bytes

                // Combine the salt and hash for storage
                string saltBase64 = Convert.ToBase64String(salt);
                string hashBase64 = Convert.ToBase64String(hash);

                return $"{saltBase64}:{hashBase64}"; // Format: "salt:hash"
            }
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            // Split the stored hash into salt and hash
            string[] parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            string originalHash = parts[1];

            // Hash the input password with the same salt
            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = 8;
                argon2.MemorySize = 65536;
                argon2.Iterations = 4;

                byte[] hash = argon2.GetBytes(32);
                string hashBase64 = Convert.ToBase64String(hash);

                // Compare the hashes
                return hashBase64 == originalHash;
            }
        }
    }
}
