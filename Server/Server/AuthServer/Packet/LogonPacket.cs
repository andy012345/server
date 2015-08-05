using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Net;
using Shared;

namespace Server.Networking
{
    class LogonPacketProcessor : PacketProcessor
    {
        public LogonPacketProcessor() : base() { dataNeeded = DefaultDataNeeded(); } //opcode

        AuthOp opcode;

        public override int DefaultDataNeeded() { return 1; }

        public override PacketProcessResult ProcessData()
        {
            opcode = (AuthOp)packetReader.ReadByte();

            return ProcessPacket();
        }

        PacketProcessResult ProcessPacket()
        {
            var handler = AuthServer.Main.LogonPacketHandler.GetHandler(opcode);

            if (handler == null)
            {
                Console.WriteLine("Recieved packet {0} which has no handler", opcode);
                return PacketProcessResult.Error;
            }

            return handler(this);
        }
    }

    public class LogonPacketHandler
    {
        Dictionary<UInt16, Func<PacketProcessor, PacketProcessResult>> PacketHandlers = new Dictionary<UInt16, Func<PacketProcessor, PacketProcessResult>>();

        public void Init()
        {
            var type = this.GetType();

            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);

            foreach (var method in methods)
            {
                var attrib = method.GetCustomAttribute<PacketHandlerAttribute>();

                if (attrib == null)
                    continue;

                var handlerDelegate = (Func<PacketProcessor, PacketProcessResult>)Delegate.CreateDelegate(typeof(Func<PacketProcessor, PacketProcessResult>), method);

                if (handlerDelegate == null)
                    continue;

                PacketHandlers.Add(attrib.Id, handlerDelegate);
            }
        }

        public Func<PacketProcessor, PacketProcessResult> GetHandler(AuthOp opcode)
        {
            UInt16 opshort = (UInt16)opcode;
            if (!PacketHandlers.ContainsKey(opshort))
                return null;
            return PacketHandlers[opshort];
        }

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

            var client = p.packetReader.ReadFourCC();
            var client_major = p.packetReader.ReadByte();
            var client_minor = p.packetReader.ReadByte();
            var client_revision = p.packetReader.ReadByte();
            var client_build = p.packetReader.ReadUInt16();
            var processor = p.packetReader.ReadFourCC();
            var os = p.packetReader.ReadFourCC();
            var locale = p.packetReader.ReadFourCC();
            var timezone = p.packetReader.ReadInt32();
            var ipaddr = new IPAddress(p.packetReader.ReadBytes(4));
            var account = p.packetReader.ReadString();

            if (p.sock != null && p.sock.session != null)
                p.sock.session.OnLogonChallenge(account);

            return PacketProcessResult.Processed;
        }

        [PacketHandler(AuthOp.REALM_LIST)]
        public static PacketProcessResult HandleRealmList(PacketProcessor p)
        {
            p.dataNeeded = 5; if (p.packetData.Length < p.dataNeeded) return PacketProcessResult.RequiresData;
            if (p.sock != null && p.sock.session != null)
                p.sock.session.OnRealmList();
            return PacketProcessResult.Processed;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class PacketHandlerAttribute : Attribute
    {
        public enum PacketType
        {
            AuthPacket,
            RealmPacket
        }

        public enum PacketFlags
        {
            None          =   0x0000,
        }

        public PacketType Type;
        public UInt16 Id = 0;
        public PacketFlags Flags = PacketFlags.None;

        public PacketHandlerAttribute(AuthOp op)
        {
            Type = PacketType.AuthPacket;
            Id = (UInt16)op;
        }
    }
}
