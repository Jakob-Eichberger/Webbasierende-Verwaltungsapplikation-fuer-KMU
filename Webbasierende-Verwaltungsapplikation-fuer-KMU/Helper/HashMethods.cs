using System;
using System.Security.Cryptography;
using System.Text;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Helper
{
    public class HashMethods
    {
        public static string GenerateSecret(int bits = 1024)
        {
            byte[] data = new byte[bits / 8];
            RandomNumberGenerator rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(data);
            return Convert.ToBase64String(data);
        }

        public static string GenerateURLSafeSecret(int bits = 1024) => GenerateSecret(bits).Replace('+', '-').Replace('/', '_').Replace("=", "");

        public static string HashPassword(string secret, string password) => HashPassword(Convert.FromBase64String(secret), password);

        public static string HashPassword(byte[] secret, string password)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashed = new HMACSHA256(secret).ComputeHash(dataBytes);
            return Convert.ToBase64String(hashed);
        }
    }
}
