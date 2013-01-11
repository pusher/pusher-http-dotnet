using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Pusher.Server
{
	internal class CryptoHelper
	{

		internal static string GetMd5Hash(string jsonData)
		{
			var hash = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(jsonData));
			return BytesToHex(hash);
		}

		private static string BytesToHex(IEnumerable<byte> byteArray)
		{
			return String.Concat(byteArray.Select(bytes => bytes.ToString("x2")).ToArray());
		}

		internal static string GetHmac256(string secret, string toSign)
		{
			var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
			var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(toSign));

			return BytesToHex(hash);
		}
	}
}
