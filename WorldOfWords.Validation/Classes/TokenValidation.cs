using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNet.Identity;

namespace WorldOfWords.Validation.Classes
{
    public class TokenValidation : ITokenValidation
    {
        public static string EmailAndIdToken { get; private set; }
        public static string ClaimsToken { get; private set; }
        public static PasswordHasher Hasher = new PasswordHasher();
        private static readonly Random Random = new Random();
        public static string StringCharacters = "1234567890abcdefhijklmnopqrstyuxzwvAQWERTYUIOPSDFGHJKLZXCVBNM"; 

        public string EncodeEmailAndIdToken(string token)
        {
            EmailAndIdToken = Base64Encode(token);
            return EmailAndIdToken;
        }

        public string EncodeRoleToken(IEnumerable<string> token)
        {
            var _token = "";
            foreach (var claimToken in token)
            {
                if (_token != "")
                    _token += "|" + claimToken;
                else
                    _token += claimToken;
            }
            ClaimsToken = Base64Encode(_token);
            return ClaimsToken;
        }

        public string GetHashSha256(string token)
        {
            var bytes = Encoding.UTF8.GetBytes(token);
            var hasher = new SHA256Managed();
            var hash = hasher.ComputeHash(bytes);
            return hash.Aggregate(string.Empty, (current, x) => current + string.Format("{0:x2}", x));
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public string GenerateUnicToken()
        {
            return Hasher.HashPassword(StringCharacters);
        }

        public String Sha256Hash(String value)
        {
            using (SHA256 hash = SHA256.Create())
            {
                return String.Join("", hash
                  .ComputeHash(Encoding.UTF8.GetBytes(value))
                  .Select(item => item.ToString("x2")));
            }
        }

        public string RandomString(int size)
        {
            var chars = Enumerable.Range(0, size)
                                   .Select(x => StringCharacters[Random.Next(0, StringCharacters.Length)]);
            return new string(chars.ToArray());
        }
    }
}