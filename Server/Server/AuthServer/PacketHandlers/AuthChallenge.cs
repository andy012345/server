using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Shared;
using Server.Networking;

namespace Server.AuthServer
{
    public partial class LogonPacketHandler
    {
        [PacketHandler(AuthOp.AUTH_LOGON_CHALLENGE)]
        public static PacketProcessResult HandleLogonAuthChallenge(PacketProcessor p)
        {
            if (p.currentPacket.Length < 4)
            {
                p.dataNeeded = 4; //1 opcode, 1 version, 2 data_size
                return PacketProcessResult.RequiresData;
            }

            byte proto_version = p.currentPacket.ReadByte();
            UInt16 data_size = p.currentPacket.ReadUInt16();

            if (p.currentPacket.Length < 4 + data_size)
            {
                p.dataNeeded = 4 + data_size; //1 opcode, 1 version, 2 data_size, data_size data
                return PacketProcessResult.RequiresData;
            }

            AuthLogonChallenge challenge = new AuthLogonChallenge();

            challenge.client = p.currentPacket.ReadFourCC();
            challenge.client_major = p.currentPacket.ReadByte();
            challenge.client_minor = p.currentPacket.ReadByte();
            challenge.client_revision = p.currentPacket.ReadByte();
            challenge.client_build = p.currentPacket.ReadUInt16();
            challenge.processor = p.currentPacket.ReadFourCC();
            challenge.os = p.currentPacket.ReadFourCC();
            challenge.locale = p.currentPacket.ReadFourCC();
            challenge.category = p.currentPacket.ReadInt32();
            challenge.ipaddr = new IPAddress(p.currentPacket.ReadBytes(4));
            challenge.account = p.currentPacket.ReadString();

            if (p.sock != null && p.sock.session != null)
                p.sock.session.OnLogonChallenge(challenge);

            return PacketProcessResult.Processed;
        }


    }
}
