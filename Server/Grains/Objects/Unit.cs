﻿using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Server
{
    public interface UnitData : ObjectData
    {
        int unit_test { get; set; }
    }

    public class UnitImpl : Unit<UnitData>, IUnit { }
    public class Unit<T> : Object<T>, IUnitImpl
        where T : class, UnitData
    {
        public override Task<string> VirtualCall() { return Task.FromResult("Virtual call from unit"); }
        public Task<string> UnitCall() { return Task.FromResult("Call from unit"); }

        public override Task OnActivateAsync()
        {
            State.UpdateFlags = ObjectUpdateFlags.UPDATEFLAG_LIVING | ObjectUpdateFlags.UPDATEFLAG_STATIONARY_POSITION;

            return base.OnActivateAsync();
        }


        #region Unitfield Getters and Setters
        public int DisplayID
        {
            get { return _GetInt32((int)EUnitFields.UNIT_FIELD_DISPLAYID); }
            set { _SetInt32((int)EUnitFields.UNIT_FIELD_DISPLAYID, value); }
        }

        public int NativeDisplayID
        {
            get { return _GetInt32((int)EUnitFields.UNIT_FIELD_NATIVEDISPLAYID); }
            set { _SetInt32((int)EUnitFields.UNIT_FIELD_NATIVEDISPLAYID, value); }
        }

        public int Faction
        {
            get { return _GetInt32((int)EUnitFields.UNIT_FIELD_FACTIONTEMPLATE); }
            set { _SetInt32((int)EUnitFields.UNIT_FIELD_FACTIONTEMPLATE, value); }
        }

        public Task<int> GetDisplayID() { return Task.FromResult(DisplayID); }
        public Task<int> GetNativeDisplayID() { return Task.FromResult(NativeDisplayID); }

        public override bool _IsUnit() { return true; }

        #endregion
    }
}
