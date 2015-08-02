using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Net;

namespace Server.Networking
{
    public enum AuthOPCodes : byte
    {
        AUTH_LOGON_CHALLENGE = 0x00,
        AUTH_LOGON_PROOF = 0x01,
        AUTH_RECONNECT_CHALLENGE = 0x02,
        AUTH_RECONNECT_PROOF = 0x03,
        REALM_LIST = 0x10,
        XFER_INITIATE = 0x30,
        XFER_DATA = 0x31,
        XFER_ACCEPT = 0x32,
        XFER_RESUME = 0x33,
        XFER_CANCEL = 0x34,
        Maximum = 100,

        Unknown = byte.MaxValue,
    }

    class LogonPacketProcessor : PacketProcessor
    {
        public LogonPacketProcessor() { dataNeeded = 1; } //opcode

        AuthOPCodes opcode;

        public override PacketProcessResult ProcessData()
        {
            opcode = (AuthOPCodes)packetReader.ReadByte();

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

        public Func<PacketProcessor, PacketProcessResult> GetHandler(AuthOPCodes opcode)
        {
            UInt16 opshort = (UInt16)opcode;
            if (!PacketHandlers.ContainsKey(opshort))
                return null;
            return PacketHandlers[opshort];
        }

        [PacketHandler(AuthOPCodes.AUTH_LOGON_CHALLENGE)]
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

        public PacketHandlerAttribute(AuthOPCodes op)
        {
            Type = PacketType.AuthPacket;
            Id = (UInt16)op;
        }
    }
}
