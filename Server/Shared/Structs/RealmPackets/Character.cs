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
            Race = p.ReadByte();
            Class = p.ReadByte();
            Gender = p.ReadByte();
            Skin = p.ReadByte();
            Face = p.ReadByte();
            HairStyle = p.ReadByte();
            HairColor = p.ReadByte();
            FacialHair = p.ReadByte();
            Outfit = p.ReadByte();
        }
    }

    public struct CMSG_PLAYER_LOGIN
    {
        public ObjectGUID GUID;

        public void Read(PacketIn p)
        {
            GUID = p.ReadGUID();
        }

    }
}
