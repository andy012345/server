using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Networking.Reader
{
    public class PacketReader : BinaryReader
    {
        public PacketReader(Stream s) : base(s)
        {
      
        }

        public string ReadFourCC()
        {
            byte[] tmp = ReadBytes(4);
            return Encoding.ASCII.GetString(tmp);
        }

    }
}
