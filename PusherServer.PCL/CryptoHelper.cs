using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCLCrypto;

namespace PusherServer
{
    internal class CryptoHelper
    {
        internal static string GetMd5Hash(string jsonData)
        {
            var hasher = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Md5);
            byte[] hash = hasher.HashData(Encoding.UTF8.GetBytes(jsonData));
            string hashBase64 = BytesToHex(hash);

            return hashBase64;
        }

        private static string BytesToHex(IEnumerable<byte> byteArray)
        {
            return String.Concat(byteArray.Select(bytes => bytes.ToString("x2")).ToArray());
        }

        internal static string GetHmac256(string secret, string toSign)
        {
            var algorithm = WinRTCrypto.MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacSha256);
            CryptographicHash hasher = algorithm.CreateHash(Encoding.UTF8.GetBytes(secret));
            hasher.Append(Encoding.UTF8.GetBytes(toSign));
            byte[] mac = hasher.GetValueAndReset();
            string macBase64 = BytesToHex(mac);

            return macBase64;
        }
    }
}
