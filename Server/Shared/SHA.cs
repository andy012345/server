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

            var sb = new StringBuilder();
            foreach (byte b in hash)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }

            return sb.ToString();
        }
    }
}
