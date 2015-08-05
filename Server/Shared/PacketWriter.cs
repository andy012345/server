using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Packet
    {
        public MemoryStream strm = new MemoryStream();
        public BinaryWriter w;

        public Packet() { w = new BinaryWriter(strm); }
        public Packet(AuthOp op)  { w = new BinaryWriter(strm); Write((byte)op); }
        
        public void Write(ushort value) { w.Write(value);  }
        public void Write(uint value) { w.Write(value); }
        public void Write(ulong value) { w.Write(value);  }
        public void Write(string value) { w.Write(value);  }
        public void Write(float value) { w.Write(value);  }
        public void Write(long value) { w.Write(value);  }
        public void Write(int value) { w.Write(value);  }
        public void Write(short value) { w.Write(value);  }
        public void Write(byte[] buffer) { w.Write(buffer); }
        public void Write(char ch) { w.Write(ch); }
        public void Write(char[] chars) { w.Write(chars); }
        public void Write(byte value) { w.Write(value); }
        public void Write(decimal value) { w.Write(value); }
        public void Write(double value) { w.Write(value); }
        public void Write(sbyte value) { w.Write(value);  }
        public void Write(bool value) { w.Write(value); }
        public void Write(char[] chars, int index, int count) { w.Write(chars, index, count); }
        public void Write(byte[] buffer, int index, int count) { w.Write(buffer, index, count);  }

        public void WriteBigInt(BigInteger b, int length)
        {
            byte[] bytes = b.GetBytes();
            int bytes_copy_amount = Math.Min(length, bytes.Length);
            Write(bytes, 0, bytes_copy_amount);
            for (int i = bytes.Length; i < length; ++i)
                Write((byte)0); //pad
        }

        public void WriteBigIntLength(BigInteger b, int length)
        {
            Write((byte)length);
            WriteBigInt(b, length);
        }

        public void WriteCString(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            Write(bytes);
            Write((byte)0); //null terminator
        }
    }
}
