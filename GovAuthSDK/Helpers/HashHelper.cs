using System.Security.Cryptography;
using System.Text;

namespace GovAuthSDK.Helpers
{
    internal static class HashHelper
    {
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static bool Match(string hashedPassword, string password)
        {
            var inputHashed = HashPassword(password);
            return hashedPassword == inputHashed;
        }
    }
}
