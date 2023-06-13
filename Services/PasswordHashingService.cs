using System.Security.Cryptography;
using System.Text;
using api.Interfaces;

namespace api.Services
{
    public class PasswordHashingService : IPasswordHashingService
    {
        public PasswordHashResult HashPassword(string password)
        {
            using var hmac = new HMACSHA512();
            var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            var passwordSalt = hmac.Key;

            return new PasswordHashResult()
            {
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
        }

        public bool ComparePassword(byte[] passwordSalt, byte[] passwordHash, string password)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            for (var i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != passwordHash[i]) return false;
            }

            return true;
        }
    }
}