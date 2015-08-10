using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class ObjectGUID
    {
        UInt64 _value = 0;

        ObjectGUID(UInt64 g) { _value = g; }

        public override bool Equals(object obj)
        {
            return _value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public UInt64 ToUInt64() { return _value; }
        public Int64 ToInt64() { return (Int64)_value; }

        public byte[] ToPackedBytes()
        {
            MemoryStream strm = new MemoryStream();
            BinaryWriter w = new BinaryWriter(strm);
            w.Write((byte)0); //to fill later, mask

            var bytes = BitConverter.GetBytes(_value);

            byte mask = 0;

            for (var i = 0; i < bytes.Length; ++i)
            {
                if (bytes[i] == 0)
                    continue;
                mask |= (byte)(1 << i);
                w.Write(bytes[i]);
            }

            strm.Position = 0;
            w.Write(mask);
            strm.Position = strm.Length;
            return strm.ToArray();
        }

        public static bool operator ==(ObjectGUID x, ObjectGUID y)
        {
            return x._value == y._value;
        }
        public static bool operator !=(ObjectGUID x, ObjectGUID y)
        {
            return x._value != y._value;
        }

        public static bool operator >=(ObjectGUID x, ObjectGUID y)
        {
            return x._value >= y._value;
        }

        public static bool operator <(ObjectGUID x, ObjectGUID y)
        {
            return x._value < y._value;
        }

        public static bool operator >(ObjectGUID x, ObjectGUID y)
        {
            return x._value > y._value;
        }

        public static bool operator <=(ObjectGUID x, ObjectGUID y)
        {
            return x._value <= y._value;
        }

        public static bool operator ==(ObjectGUID x, UInt64 y)
        {
            return x._value == y;
        }
        public static bool operator !=(ObjectGUID x, UInt64 y)
        {
            return x._value != y;
        }

        public static bool operator >=(ObjectGUID x, UInt64 y)
        {
            return x._value >= y;
        }

        public static bool operator <(ObjectGUID x, UInt64 y)
        {
            return x._value < y;
        }

        public static bool operator >(ObjectGUID x, UInt64 y)
        {
            return x._value > y;
        }

        public static bool operator <=(ObjectGUID x, UInt64 y)
        {
            return x._value <= y;
        }
    }
}
