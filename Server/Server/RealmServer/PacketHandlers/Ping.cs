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
        [PacketHandler(RealmOp.CMSG_PING)]
        public static PacketProcessResult HandlePing(PacketProcessor p)
        {
            CMSG_PING pkt = new CMSG_PING();
            pkt.Read(p.currentPacket);

            PacketOut pout = new PacketOut(RealmOp.SMSG_PONG);
            pout.Write(pkt.ping);
            pout.Finalise();
            p.sock.Send(pout.strm.ToArray());

            return PacketProcessResult.Processed;
        }

    }
}
