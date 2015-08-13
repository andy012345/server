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
    public class ChrClassesStore : DBCStore<ChrClasses>
    {
        public ChrClasses Get(UInt32 Class)
        {
            return Get((int)Class);
        }
    }

    public class ChrClasses : DBCRecordBase
    {
        public UInt32 Class = 0;
        public UInt32 powerType = 0;
        public string[] Name = new string[16];
        public string[] FemaleName = new string[16];
        public string[] NeutralName = new string[16];
        public UInt32 SpellFamily = 0;
        public UInt32 Flags = 0;
        public UInt32 CinematicSequence = 0;
        public UInt32 ExpansionRequired = 0;

        public override int Read()
        {
            Class = GetUInt32(0);
            powerType = GetUInt32(2);

            for (int i = 0; i < 16; ++i)
                Name[i] = GetString(4 + i);
            for (int i = 0; i < 16; ++i)
                FemaleName[i] = GetString(21 + i);
            for (int i = 0; i < 16; ++i)
                NeutralName[i] = GetString(38 + i);

            SpellFamily = GetUInt32(56);
            Flags = GetUInt32(57);
            CinematicSequence = GetUInt32(58);
            ExpansionRequired = GetUInt32(59);


            return (int)Class;
        }
    }
}
