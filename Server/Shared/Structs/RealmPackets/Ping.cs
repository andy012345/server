using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public struct CMSG_PING
    {
        public UInt32 ping;
        public UInt32 latency;

        public void Read(PacketIn p)
        {
            ping = p.ReadUInt32();
            latency = p.ReadUInt32();
        }
    }
    
}
