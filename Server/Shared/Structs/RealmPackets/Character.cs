using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public struct CMSG_CHAR_CREATE
    {
        public string Name;
        public byte Class;
        public byte Race;
        public byte Gender;
        public byte Skin;
        public byte Face;
        public byte HairStyle;
        public byte HairColor;
        public byte FacialHair;
        public byte Outfit;

        public void Read(PacketIn p)
        {
            Name = p.ReadCString();
            Class = p.ReadByte();
            Race = p.ReadByte();
            Gender = p.ReadByte();
            Skin = p.ReadByte();
            Face = p.ReadByte();
            HairStyle = p.ReadByte();
            HairColor = p.ReadByte();
            FacialHair = p.ReadByte();
            Outfit = p.ReadByte();
        }
    }
}
