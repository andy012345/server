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
    public class CharStartOutfitStore : DBCStore<CharStartOutfit>
    {
        public CharStartOutfit Get(UInt32 Class, UInt32 Race, UInt32 Gender)
        {
            return RecordDataIndexed.Values.ToArray().Where(c => c.Class == Class && c.Race == Race && c.Gender == Gender).FirstOrDefault();
        }
    }

    public class CharStartOutfit : DBCRecordBase
    {
        public UInt32 ID = 0;
        public UInt32 Race = 0;
        public UInt32 Class = 0;
        public UInt32 Gender = 0;
        public UInt32[] Items = new UInt32[24];

        public override int Read()
        {
            ID = GetUInt32(0);

            var tmp = GetUInt32(1);
            Race = tmp & 0xFF;
            Class = (tmp >> 8) & 0xFF;
            Gender = (tmp >> 16) & 0xFF;

            for (int i = 0; i < Items.Length; ++i)
                Items[i] = GetUInt32(2 + i);

            return (int)ID;
        }
    }
}
