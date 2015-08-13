using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public enum TypeMask
    {
        TYPEMASK_OBJECT = 0x0001,
        TYPEMASK_ITEM = 0x0002,
        TYPEMASK_CONTAINER = 0x0006,                       // TYPEMASK_ITEM | 0x0004
        TYPEMASK_UNIT = 0x0008,                       // creature
        TYPEMASK_PLAYER = 0x0010,
        TYPEMASK_GAMEOBJECT = 0x0020,
        TYPEMASK_DYNAMICOBJECT = 0x0040,
        TYPEMASK_CORPSE = 0x0080,
        TYPEMASK_SEER = TYPEMASK_PLAYER | TYPEMASK_UNIT | TYPEMASK_DYNAMICOBJECT
    };

    public enum HighGuid
    {
        HIGHGUID_ITEM = 0x4000,                      // blizz 4000
        HIGHGUID_CONTAINER = 0x4000,                      // blizz 4000
        HIGHGUID_PLAYER = 0x0000,                      // blizz 0000
        HIGHGUID_GAMEOBJECT = 0xF110,                      // blizz F110
        HIGHGUID_TRANSPORT = 0xF120,                      // blizz F120 (for GAMEOBJECT_TYPE_TRANSPORT)
        HIGHGUID_UNIT = 0xF130,                      // blizz F130
        HIGHGUID_PET = 0xF140,                      // blizz F140
        HIGHGUID_VEHICLE = 0xF150,                      // blizz F550
        HIGHGUID_DYNAMICOBJECT = 0xF100,                      // blizz F100
        HIGHGUID_CORPSE = 0xF101,                      // blizz F100
        HIGHGUID_MO_TRANSPORT = 0x1FC0,                      // blizz 1FC0 (for GAMEOBJECT_TYPE_MO_TRANSPORT)
        HIGHGUID_INSTANCE = 0x1F40,                      // blizz 1F40
        HIGHGUID_GROUP = 0x1F50
    };

    public class ObjectGUID
    {
        UInt64 _value = 0;

        public ObjectGUID(UInt64 g) { _value = g; }

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

        public static bool operator ==(ObjectGUID x, UInt64 y)
        {
            return x._value == y;
        }
        public static bool operator !=(ObjectGUID x, UInt64 y)
        {
            return x._value != y;
        }

        public static bool operator ==(ObjectGUID x, Int64 y)
        {
            return x._value == (UInt64)y;
        }
        public static bool operator !=(ObjectGUID x, Int64 y)
        {
            return x._value != (UInt64)y;
        }

        public static bool operator ==(ObjectGUID x, int y)
        {
            return x._value == (UInt64)y;
        }
        public static bool operator !=(ObjectGUID x, int y)
        {
            return x._value != (UInt64)y;
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
