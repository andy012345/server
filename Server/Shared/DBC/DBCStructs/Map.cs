using System.Threading.Tasks;
using System;
using System.IO;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Linq;
using Shared;

namespace Server
{
    public class MapStore : DBCStore<MapEntry>
    {
        public MapEntry Get(UInt32 MapID)
        {
            return Get((int)MapID);
        }
    }

    public enum MapType
    {
        World,
        Party,
        Raid,
        PVP,
        Arena,
        None,
    }

    public class MapEntry : DBCRecordBase
    {
        public UInt32 MapID = 0xFFFFFFFF;
        public string InternalName = "Unknown";
        public MapType Type = 0;
        public UInt32 Flags = 0;
        public UInt32 EntranceMapID = 0;
        public float EntranceMapX = 0;
        public float EntranceMapY = 0;

        public override int Read()
        {
            MapID = GetUInt32(0);
            InternalName = GetString(1);
            Type = (MapType)GetUInt32(2);

            EntranceMapID = GetUInt32(59);
            EntranceMapX = GetFloat(60);
            EntranceMapY = GetFloat(61);
            
            return (int)MapID;
        }

        public bool IsInstance()
        {
            if (Type == MapType.Party || Type == MapType.Raid || Type == MapType.PVP || Type == MapType.Arena)
                return true;
            return false;
        }

        public bool IsDungeon()
        {
            if (Type == MapType.Party)
                return true;
            return false;
        }

        public bool IsRaid()
        {
            if (Type == MapType.Raid)
                return true;
            return false;
        }
    }
}
