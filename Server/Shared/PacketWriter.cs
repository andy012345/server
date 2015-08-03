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
        public PacketWriter w;

        public Packet() { w = new PacketWriter(strm); }
    }

    public class PacketWriter : BinaryWriter
    {
        public PacketWriter(Stream s) : base(s) {}

        public void WriteBigInt(BigInteger b, int length)
        {
            byte[] bytes = b.ToByteArray();
            Write(bytes);
            for (int i = bytes.Length; i < length; ++i)
                Write((byte)0); //pad
        }

        public void WriteBigIntLength(BigInteger b, int length)
        {
            Write((byte)length);
            WriteBigInt(b, length);
        }
    }
}
