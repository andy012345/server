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
    public class ChrRacesStore : DBCStore<ChrRaces>
    {
        public ChrRaces Get(UInt32 Race)
        {
            return Get((int)Race);
        }
    }

    public class ChrRaces : DBCRecordBase
    {
        public UInt32 Race = 0;
        public UInt32 Flags = 0;
        public UInt32 Faction = 0;
        public UInt32 ExplorationData = 0;
        public UInt32 ModelMale = 0;
        public UInt32 ModelFemale = 0;
        public UInt32 Team = 0;
        public UInt32 CinematicSequence = 0;
        public UInt32 ExpensionRequired = 0;
        public string[] Name = new string [16];
        public string[] FemaleName = new string[16];
        public string[] NeutralName = new string[16];
        public string ShortName = "";
        //char const ChrRacesEntryfmt[] = "niixiixixxxxixssssssssssssssssxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxi";


        public override int Read()
        {
            Race = GetUInt32(0);
            Flags = GetUInt32(1);
            Faction = GetUInt32(2);
            ExplorationData = GetUInt32(3);
            ModelMale = GetUInt32(4);
            ModelFemale = GetUInt32(5);
            ShortName = GetString(6);
            Team = GetUInt32(7);
            CinematicSequence = GetUInt32(13);

            for (int i = 0; i < 16; ++i)
                Name[i] = GetString(14 + i);
            for (int i = 0; i < 16; ++i)
                FemaleName[i] = GetString(31 + i);
            for (int i = 0; i < 16; ++i)
                NeutralName[i] = GetString(48 + i);

            ExpensionRequired = GetUInt32(68);


            return (int)Race;
        }
    }
}
