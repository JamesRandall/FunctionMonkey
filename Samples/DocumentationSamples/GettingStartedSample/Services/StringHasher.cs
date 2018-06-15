using System;
using System.Security.Cryptography;
using System.Text;

namespace GettingStartedSample.Services
{
    internal class StringHasher : IStringHasher
    {
        public string Hash(string value)
        {
            SHA256 sha256 = new SHA256Managed();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
            string result = Convert.ToBase64String(bytes);
            return result;
        }
    }
}
