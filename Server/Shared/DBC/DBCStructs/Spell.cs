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
using System.Runtime.InteropServices;

namespace Server
{
    public class SpellStore : DBCStore<SpellEntry>
    {
        public SpellEntry Get(UInt32 SpellID)
        {
            return Get((int)SpellID);
        }
    }

    public enum SpellEnums
    {
        MaxSpellReagents = 8,
        MaxSpellEffects = 3,
    }

    public class SpellEntry : DBCRecordBase
    {
        UInt32 Id = 0;
        UInt32 Category;
        UInt32 Dispel;
        UInt32 Mechanic;
        UInt32 Attributes;
        UInt32 AttributesEx;
        UInt32 AttributesEx2;
        UInt32 AttributesEx3;
        UInt32 AttributesEx4;
        UInt32 AttributesEx5;
        UInt32 AttributesEx6;
        UInt32 AttributesEx7;
        UInt32 Stances;
        UInt32 StancesNot;
        UInt32 Targets;

        UInt32 TargetCreatureType;
        UInt32 RequiresSpellFocus;
        UInt32 FacingCasterFlags;
        UInt32 CasterAuraState;
        UInt32 TargetAuraState;
        UInt32 CasterAuraStateNot;
        UInt32 TargetAuraStateNot;
        UInt32 CasterAuraSpell;
        UInt32 TargetAuraSpell;
        UInt32 ExcludeCasterAuraSpell;
        UInt32 ExcludeTargetAuraSpell;
        UInt32 CastingTimeIndex;
        UInt32 RecoveryTime;
        UInt32 CategoryRecoveryTime;
        UInt32 InterruptFlags;
        UInt32 AuraInterruptFlags;
        UInt32 ChannelInterruptFlags;
        UInt32 ProcFlags;
        UInt32 ProcChance;
        UInt32 ProcCharges;
        UInt32 MaxLevel;
        UInt32 BaseLevel;
        UInt32 SpellLevel;
        UInt32 DurationIndex;
        UInt32 PowerType;
        UInt32 ManaCost;
        UInt32 ManaCostPerLevel;
        UInt32 ManaPerSecond;
        UInt32 ManaPerSecondPerLevel;
        UInt32 RangeIndex;
        float Speed;
        UInt32 ModalNextSpell;
        UInt32 StackAmount;
        UInt32[] Totem = new UInt32[2];
        //Int32[] Reagent = new Int32[SpellEnums.MaxSpellReagents];


        public override int Read()
        {
            //            char const SpellEntryfmt[] = "niiiiiiiiiiiixixiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiifxiiiiiiiiiiiiiiiiiiiiiiiiiiiifffiiiiiiiiiiiiiiiiiiiiifffiiiiiiiiiiiiiiifffiiiiiiiiiiiiixssssssssssssssssxssssssssssssssssxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxiiiiiiiiiiixfffxxxiiiiixxfffxx";
            var i = 0;

            Id = GetUInt32(i++);
            Category = GetUInt32(i++);
            Dispel = GetUInt32(i++);
            Mechanic = GetUInt32(i++);
            Attributes = GetUInt32(i++);
            AttributesEx = GetUInt32(i++);
            AttributesEx2 = GetUInt32(i++);
            AttributesEx3 = GetUInt32(i++);
            AttributesEx4 = GetUInt32(i++);
            AttributesEx5 = GetUInt32(i++);
            AttributesEx6 = GetUInt32(i++);
            AttributesEx7 = GetUInt32(i++);
            Stances = GetUInt32(i++);
            i += 1; //ignore unk
            StancesNot = GetUInt32(i++);
            i += 1; //ignore unk
            Targets = GetUInt32(i++);
            TargetCreatureType = GetUInt32(i++);
            RequiresSpellFocus = GetUInt32(i++);
            FacingCasterFlags = GetUInt32(i++);
            CasterAuraState = GetUInt32(i++);
            TargetAuraState = GetUInt32(i++);
            CasterAuraStateNot = GetUInt32(i++);
            TargetAuraStateNot = GetUInt32(i++);
            CasterAuraSpell = GetUInt32(i++);
            TargetAuraSpell = GetUInt32(i++);
            ExcludeCasterAuraSpell = GetUInt32(i++);
            ExcludeTargetAuraSpell = GetUInt32(i++);
            CastingTimeIndex = GetUInt32(i++);
            RecoveryTime = GetUInt32(i++);
            CategoryRecoveryTime = GetUInt32(i++);
            InterruptFlags = GetUInt32(i++);



            return (int)Id;
        }

    }
}
