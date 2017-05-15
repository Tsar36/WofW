using System;
using System.Collections.Generic;

namespace WorldOfWords.Validation
{
    public interface ITokenValidation
    {
        string EncodeEmailAndIdToken(string token);
        string EncodeRoleToken(IEnumerable<string> token);
        string GetHashSha256(string token);
        string GenerateUnicToken();
        String Sha256Hash(String value);
        string RandomString(int size);
    }
}