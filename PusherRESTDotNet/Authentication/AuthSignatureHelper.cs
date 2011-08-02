using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace PusherRESTDotNet.Authentication
{
    internal static class AuthSignatureHelper
    {
        public static string GetAuthString(string authData, string applicationSecretKey)
        {
            var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(applicationSecretKey));
            var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(authData));

            return BytesToHex(hash);
        }

        private static string BytesToHex(IEnumerable<byte> byteArray)
        {
            return String.Concat(byteArray.Select(bytes => bytes.ToString("x2")).ToArray());
        }
    }
}