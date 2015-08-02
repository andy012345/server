using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

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
            BinaryReader reader = new BinaryReader(packetData);

            opcode = (AuthOPCodes)reader.ReadByte();
            
            if (LogonPacketHandler.LogonPacketHasData(opcode))
            {
                dataNeeded = 4;
                if (packetData.Length < dataNeeded) return PacketProcessResult.PacketRequiresMoreData;

                byte error = reader.ReadByte();
                UInt16 dataSize = reader.ReadUInt16();

                dataNeeded = 4 + dataSize;
                if (packetData.Length < dataNeeded) return PacketProcessResult.PacketRequiresMoreData;
            }

            ProcessPacket();
            return PacketProcessResult.PacketProcessed;
        }

        void ProcessPacket()
        {
            var handler = AuthServer.Main.LogonPacketHandler.GetHandler(opcode);

            if (handler == null)
            {
                Console.WriteLine("Recieved packet {0} which has no handler", opcode);
            }

            handler(this);
        }
    }

    public class LogonPacketHandler
    {
        Dictionary<UInt16, Action<PacketProcessor>> PacketHandlers = new Dictionary<UInt16, Action<PacketProcessor>>();

        public void Init()
        {
            var type = this.GetType();

            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);

            foreach (var method in methods)
            {
                var attrib = method.GetCustomAttribute<PacketHandlerAttribute>();

                if (attrib == null)
                    continue;

                var handlerDelegate = (Action<PacketProcessor>)Delegate.CreateDelegate(typeof(Action<PacketProcessor>), method);

                if (handlerDelegate == null)
                    continue;

                PacketHandlers.Add(attrib.Id, handlerDelegate);
            }
        }

        public static bool LogonPacketHasData(AuthOPCodes op)
        {
            switch (op)
            {
                case AuthOPCodes.XFER_INITIATE:
                case AuthOPCodes.XFER_DATA:
                case AuthOPCodes.XFER_ACCEPT:
                case AuthOPCodes.XFER_RESUME:
                    {
                        return false;
                    }
            }

            return true;
        }

        public Action<PacketProcessor> GetHandler(AuthOPCodes opcode)
        {
            UInt16 opshort = (UInt16)opcode;
            if (!PacketHandlers.ContainsKey(opshort))
                return null;
            return PacketHandlers[opshort];
        }

        [PacketHandler(AuthOPCodes.AUTH_LOGON_CHALLENGE)]
        public static void HandleLogonAuthChallenge(PacketProcessor p)
        {
            Console.WriteLine("RECIEVED LOGON CHALLENGE BITCHES!");
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
            PacketFlagNone          =   0x0000,
            PackeetFlagHasNoData    =   0x0001,
        }

        public PacketType Type;
        public UInt16 Id = 0;
        public PacketFlags Flags = PacketFlags.PacketFlagNone;

        public PacketHandlerAttribute(AuthOPCodes op)
        {
            Type = PacketType.AuthPacket;
            Id = (UInt16)op;

            if (!LogonPacketHandler.LogonPacketHasData(op))
                Flags |= PacketFlags.PackeetFlagHasNoData;
        }
    }
}
