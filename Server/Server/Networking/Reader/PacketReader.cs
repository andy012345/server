using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Shared;

namespace Server.Networking.Reader
{
    public class PacketReader : BinaryReader
    {
        public PacketReader(Stream s) : base(s)
        {
      
        }

        public long RemainingLength
        {
            get
            {
                return BaseStream.Length - BaseStream.Position;
            }
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
    }
}
