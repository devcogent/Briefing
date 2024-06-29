using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Security.Cryptography;
using System.Text;
namespace Briefing.Helper
{
    public class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;
        private const int Iterations = 10000;

        public static (string Hash, string Salt) HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Compute hash
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: Iterations,
                numBytesRequested: HashSize);

            return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }

        public static bool VerifyPassword(string password, string hashedPassword, string salt)
        {
            // Decode salt from Base64
            byte[] saltBytes = Convert.FromBase64String(salt);

            // Compute hash with retrieved salt
            byte[] hashToVerify = KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: Iterations,
                numBytesRequested: HashSize);

            // Compare the hashes
            return hashedPassword == Convert.ToBase64String(hashToVerify);
        }


    }

}

