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
        [PacketHandler(AuthOp.AUTH_LOGON_PROOF)]
        public static PacketProcessResult HandleLogonAuthProof(PacketProcessor p)
        {
            p.dataNeeded = 75; //1 op, 32 A, 20 M1, 20 crc_hash, 1 number_of_keys, 1 unk
            if (p.packetData.Length < p.dataNeeded) return PacketProcessResult.RequiresData;

            AuthLogonProof proof = new AuthLogonProof();

            proof.A = p.packetReader.ReadBytes(32);
            proof.M1 = p.packetReader.ReadBytes(20);
            proof.crchash = p.packetReader.ReadBytes(20);
            proof.number_of_keys = p.packetReader.ReadByte();
            proof.unk = p.packetReader.ReadByte();

            if (p.sock != null && p.sock.session != null)
                p.sock.session.OnLogonProof(proof);

            return PacketProcessResult.Processed;
        }

    }
}
