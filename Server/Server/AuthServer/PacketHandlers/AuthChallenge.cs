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
            if (p.packetData.Length < 4)
            {
                p.dataNeeded = 4; //1 opcode, 1 version, 2 data_size
                return PacketProcessResult.RequiresData;
            }

            byte proto_version = p.packetReader.ReadByte();
            UInt16 data_size = p.packetReader.ReadUInt16();

            if (p.packetData.Length < 4 + data_size)
            {
                p.dataNeeded = 4 + data_size; //1 opcode, 1 version, 2 data_size, data_size data
                return PacketProcessResult.RequiresData;
            }

            AuthLogonChallenge challenge = new AuthLogonChallenge();

            challenge.client = p.packetReader.ReadFourCC();
            challenge.client_major = p.packetReader.ReadByte();
            challenge.client_minor = p.packetReader.ReadByte();
            challenge.client_revision = p.packetReader.ReadByte();
            challenge.client_build = p.packetReader.ReadUInt16();
            challenge.processor = p.packetReader.ReadFourCC();
            challenge.os = p.packetReader.ReadFourCC();
            challenge.locale = p.packetReader.ReadFourCC();
            challenge.category = p.packetReader.ReadInt32();
            challenge.ipaddr = new IPAddress(p.packetReader.ReadBytes(4));
            challenge.account = p.packetReader.ReadString();

            if (p.sock != null && p.sock.session != null)
                p.sock.session.OnLogonChallenge(challenge);

            return PacketProcessResult.Processed;
        }


    }
}
