using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Shared
{
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
            None = 0x0000,
        }

        public PacketType Type;
        public UInt32 Id = 0;
        public PacketFlags Flags = PacketFlags.None;

        public PacketHandlerAttribute(AuthOp op)
        {
            Type = PacketType.AuthPacket;
            Id = (UInt32)op;
        }


        public PacketHandlerAttribute(RealmOp op)
        {
            Type = PacketType.RealmPacket;
            Id = (UInt32)op;
        }
    }

}
