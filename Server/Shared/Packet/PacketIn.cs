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
    public class PacketIn : PacketBase
    {
        BinaryReader _reader = null;
        MemoryStream _strm = null;

        public PacketIn()
        {
            Create();
        }

        public PacketIn(byte[] data)
        {
            Create();
            Write(data, 0, data.Length);
        }

        public void Reset()
        {
            _strm.Position = 0;
            _strm.SetLength(0);
        }

        void Create() { _strm = new MemoryStream(); _reader = new BinaryReader(_strm); }

        public long RemainingLength
        {
            get
            {
                return _strm.Length - _strm.Position;
            }
        }

        public long Length
        {
            get
            {
                return _strm.Length;
            }
        }

        public long Position
        {
            get { return _strm.Position; }
            set { _strm.Position = value; }
        }

        public byte[] GetBuffer() { return _strm.GetBuffer(); }

        public ObjectGUID ReadGUID()
        {
            UInt64 guid = ReadUInt64();
            return new ObjectGUID(guid);
        }

        public string ReadFourCC()
        {
            byte[] tmp = ReadBytes(4);
            byte[] s = null;

            if (tmp[3] == 0)
            {
                s = new byte[3];
                s[0] = tmp[2];
                s[2] = tmp[0];
            }
            else
            {
                s = new byte[4];

                for (int i = 0; i < 4; ++i)
                    s[0] = tmp[3 - i];
            }

            return Encoding.ASCII.GetString(s);
        }

        public string ReadCString()
        {
            byte tmp;
            var buf = new List<Byte>();

            while ((tmp = ReadByte()) != 0)
                buf.Add(tmp);

            return Encoding.UTF8.GetString(buf.ToArray());
        }

        public BigInteger ReadBigInteger(int byteCount)
        {
            var bytes = ReadBytes(byteCount);

            return new BigInteger(bytes);
        }

        public UInt16 ReadUInt16BE()
        {
            var val = ReadInt16();

            var retval = IPAddress.NetworkToHostOrder(val);

            return ((UInt16)retval);
        }


        public int PeekChar() { return _reader.PeekChar(); }
        public int Read() { return _reader.Read(); }
        public int Read(byte[] buffer, int index, int count) { return _reader.Read(buffer, index, count); }
        public int Read(char[] buffer, int index, int count) { return _reader.Read(); }
        public bool ReadBoolean() { return _reader.ReadBoolean(); }
        public byte ReadByte() { return _reader.ReadByte(); }
        public byte[] ReadBytes(int count) { return _reader.ReadBytes(count); }
        public char ReadChar() { return _reader.ReadChar(); }
        public char[] ReadChars(int count) { return _reader.ReadChars(count); }
        public decimal ReadDecimal() { return _reader.ReadDecimal(); }
        public double ReadDouble() { return _reader.ReadDouble(); }
        public short ReadInt16() { return _reader.ReadInt16(); }
        public int ReadInt32() { return _reader.ReadInt32(); }
        public long ReadInt64() { return _reader.ReadInt64(); }
        public sbyte ReadSByte() { return _reader.ReadSByte(); }
        public float ReadSingle() { return _reader.ReadSingle(); }
        public string ReadString() { return _reader.ReadString(); }
        public ushort ReadUInt16() { return _reader.ReadUInt16(); }
        public uint ReadUInt32() { return _reader.ReadUInt32(); }
        public ulong ReadUInt64() { return _reader.ReadUInt64(); }

        public void Write(byte[] data, int offset, int count) { _strm.Write(data, offset, count); }
    }
}
