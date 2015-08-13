using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    class Array
    {
        public static void Reverse<T>(T[] buffer, int length)
        {
            for (int i = 0; i < length / 2; i++)
            {
                T temp = buffer[i];
                buffer[i] = buffer[length - i - 1];
                buffer[length - i - 1] = temp;
            }
        }

        public static void Reverse<T>(T[] buffer)
        {
            Reverse(buffer, buffer.Length);
        }
    }
}
