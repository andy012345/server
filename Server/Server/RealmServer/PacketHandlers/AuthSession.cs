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
        [PacketHandler(RealmOp.CMSG_AUTH_SESSION)]
        public static PacketProcessResult HandleAuthSession(PacketProcessor p)
        {

            CMSG_AUTH_SESSION auth = new CMSG_AUTH_SESSION();
            UInt32 unk, unk2, unk3, unk4, unk5;
            UInt64 unk6;

            auth.Build = p.packetReader.ReadUInt32();
            unk = p.packetReader.ReadUInt32();
            auth.Account = p.packetReader.ReadCString();
            unk2 = p.packetReader.ReadUInt32();

            auth.Seed = p.packetReader.ReadUInt32();
            unk3 = p.packetReader.ReadUInt32();
            unk4 = p.packetReader.ReadUInt32();
            unk5 = p.packetReader.ReadUInt32();
            unk6 = p.packetReader.ReadUInt64();
            auth.Digest = p.packetReader.ReadBigInteger(20);

            var decompressedDataSize = p.packetReader.ReadInt32();
            var compressedData = p.packetReader.ReadBytes((int)(p.packetData.Length - p.packetData.Position)); //read remaining array
            auth.AddonData = Shared.ZLib.Decompress(compressedData);

            var realmprocessor = p as RealmPacketProcessor;

            //get the encryption keys first because errors are encrypted too
            p.sock.session.GetSessionKeyFromAuthAccount(auth.Account).Wait();
            realmprocessor.SetupCrypto(p.sock.session.GetSessionKey().Result);
            p.sock.session.HandleAuthSession(auth, realmprocessor.Seed);

            return PacketProcessResult.Processed;
        }
    }
}
