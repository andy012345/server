using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct UpdateField
    {
        [FieldOffset(0)]
        public byte Byte1;
        [FieldOffset(1)]
        public byte Byte2;
        [FieldOffset(2)]
        public byte Byte3;
        [FieldOffset(3)]
        public byte Byte4;

        [FieldOffset(0)]
        public float Float;

        [FieldOffset(0)]
        public short Int16Low;
        [FieldOffset(2)]
        public short Int16High;

        [FieldOffset(0)]
        public int Int32;

        [FieldOffset(0)]
        public ushort UInt16Low;
        [FieldOffset(2)]
        public ushort UInt16High;
        [FieldOffset(0)]
        public uint UInt32;

        public void Set(UInt32 val) { UInt32 = val; }
        public void Set(int val) { Int32 = val; }
        public void Set(float val) { Float = val; }
        public void Set(int index, byte val)
        {
            // clear the old value
            UInt32 &= ~(uint)(0xFF << (index * 8));
            // add the new value
            UInt32 |= (uint)(val << (index * 8));
        }

        public UInt32 GetUInt32() { return UInt32; }
        public Int32 GetInt32() { return Int32; }
        public float GetFloat() { return Float; }
        public byte GetByte(int index) { return (byte)(UInt32 >> (index * 8)); }
    }

}
