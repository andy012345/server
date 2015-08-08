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
        [PacketHandler(RealmOp.CMSG_READY_FOR_ACCOUNT_DATA_TIMES)]
        public static PacketProcessResult HandleReadyForAccountDataTimes(PacketProcessor p)
        {
            p.sock.session.HandleReadyForAccountDataTimes();          

            return PacketProcessResult.Processed;
        }

        [PacketHandler(RealmOp.CMSG_UPDATE_ACCOUNT_DATA)]
        public static PacketProcessResult HandleUpdateAccountData(PacketProcessor p)
        {

            var type = p.packetReader.ReadUInt32();
            var time = p.packetReader.ReadUInt32();
            var size = p.packetReader.ReadUInt32();
            var data = p.packetReader.ReadBytes((int)p.packetReader.RemainingLength);

            p.sock.session.HandleUpdateAccountData(type, time, size, data);

            return PacketProcessResult.Processed;
        }

    
        [PacketHandler(RealmOp.CMSG_REQUEST_ACCOUNT_DATA)]
        public static PacketProcessResult HandleRequestAccountData(PacketProcessor p)
        {
            var type = p.packetReader.ReadUInt32();

            p.sock.session.HandleRequestAccountData(type);

            return PacketProcessResult.Processed;
        }
    }
}
