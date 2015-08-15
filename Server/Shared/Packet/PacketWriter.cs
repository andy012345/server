using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public enum PacketType
    {
        AuthPacket,
        RealmPacket,
        Unknown
    }

    public class PacketOut : PacketBase
    {
        [NonSerialized]
        BinaryWriter _w = null;
        MemoryStream _strm = null;

        PacketType type = PacketType.Unknown;

        public BinaryWriter w
        {
            get
            {
                if (_w == null)
                    _w = new BinaryWriter(strm);
                return _w;
            }
        }

        public MemoryStream strm
        {
            get
            {
                if (_strm == null)
                {
                    _strm = new MemoryStream();
                    _w = null;
                }
                return _strm;
            }
            set
            {
                _strm = value;
                _w = null;
            }
        }


        public PacketOut() { }
        public PacketOut(AuthOp op) { Write((byte)op); type = PacketType.AuthPacket; }
        public PacketOut(RealmOp op)
        {
            type = PacketType.RealmPacket;

            Write((UInt16)0); //size
            Write((UInt16)op); //op
        }

        public void Finalise()
        {
            if (type == PacketType.RealmPacket)
            {
                strm.Position = 0; //size
                //Int16 szhs = IPAddress.HostToNetworkOrder((Int16)(strm.Length - 2));
                WriteBE((UInt16)(strm.Length - 2)); //don't include the opcode in the size
                strm.Position = strm.Length;
            }
        }

        public void WriteBE(ushort val)
        {
            Int16 sval = (Int16)val;
            sval = IPAddress.NetworkToHostOrder(sval);
            Write((UInt16)sval);
        }

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

        public void Write(PacketOut p)
        {
            Write(p.strm.ToArray());
        }

        public void Write(ObjectGUID guid) { Write(guid.ToUInt64()); }

        public void Write(PackedGUID guid)
        {
            Write(guid.mask);
            Write(guid.guidbytes);
        }
    }
}
