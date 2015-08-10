using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public interface UnitData : ObjectData, IGrainState
    {
        int unit_test { get; set; }
    }

    public class UnitImpl : Unit<UnitData>, IUnit { }
    public class Unit<T> : Object<T>, IUnitImpl
        where T : class, IGrainState
    {
        public override Task<string> VirtualCall() { return Task.FromResult("Virtual call from unit"); }
        public Task<string> UnitCall() { return Task.FromResult("Call from unit"); }

    }



}
