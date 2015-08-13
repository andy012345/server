using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class SHA
    {
        public static string HashString(string s)
        {
            SHA1Managed sh = new SHA1Managed();
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            var hash = sh.ComputeHash(bytes);

            BigInteger test = new BigInteger(bytes);
            BigInteger test2 = BigInt.Hash(bytes);

            string test3 = test2.ToHexString();
            string test4 = test2.ToString();

            var sb = new StringBuilder();
            foreach (byte b in hash)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }

            return sb.ToString();
        }

        public static byte[] ReverseArray(byte[] b)
        {
            var ret = new byte[b.Length];

            for (int i = 0; i < b.Length;  ++i)
                ret[b.Length - 1 - i] = b[i];

            return ret;
        }
    }
}
