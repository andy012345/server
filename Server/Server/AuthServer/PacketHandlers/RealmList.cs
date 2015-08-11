using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using Server.Networking;

namespace Server.AuthServer
{
    public partial class LogonPacketHandler
    {
        [PacketHandler(AuthOp.REALM_LIST)]
        public static PacketProcessResult HandleRealmList(PacketProcessor p)
        {
            p.dataNeeded = 5; if (p.currentPacket.Length < p.dataNeeded) return PacketProcessResult.RequiresData;
            if (p.sock != null && p.sock.session != null)
                p.sock.session.OnRealmList();
            return PacketProcessResult.Processed;
        }
    }
}
