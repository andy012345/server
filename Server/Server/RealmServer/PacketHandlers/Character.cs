using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Net;
using Shared;
using Server.Networking;
using System.IO.Compression;

namespace Server.RealmServer
{
    public partial class RealmPacketHandler
    {
        [PacketHandler(RealmOp.CMSG_CHAR_ENUM)]
        public static PacketProcessResult HandleCharEnum(PacketProcessor p)
        {
            p.sock.session.HandleCharEnum();          

            return PacketProcessResult.Processed;
        }      
    }
}
