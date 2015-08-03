using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class BigInt
    {
        public static BigInteger FromRand(int byteLength)
        {
            Random rnd = new Random();
            byte[] data = new byte[byteLength];
            rnd.NextBytes(data);

            return FromBytesUnsigned(data);
        }

        public static BigInteger FromBytesUnsigned(byte[] b)
        {
            byte[] fixed_b = new byte[b.Length + 1];
            b.CopyTo(fixed_b, 0);
            fixed_b[b.Length] = 0;
            return new BigInteger(fixed_b);
        }
    }
}
