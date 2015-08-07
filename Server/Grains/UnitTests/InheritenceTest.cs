using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public interface PlayerData : UnitData, IGrainState
    {
        int player_test { get; set; }
    }

    public interface UnitData : ObjectData, IGrainState
    {
        int unit_test { get; set; }
    }

    public interface ObjectData : IGrainState
    {
        int object_test { get; set; }
    }

    public class PlayerGrainTestImpl : PlayerGrainTest, IPlayerTestImpl { }
    public class UnitGrainTestImpl : UnitGrainTest<UnitData>, IUnitTestImpl { }
    public class ObjectGrainTestImpl : ObjectGrainTest<ObjectData>, IObjectTestImpl { }

    public class PlayerGrainTest : UnitGrainTest<PlayerData>, IPlayerTest, IUnitTest, IObjectTest
    {

        public override Task<string> VirtualCall() { return Task.FromResult("Virtual call from player");  }
        public Task<string> PlayerCall() { return Task.FromResult("Call from player"); }

    }

    public class UnitGrainTest<T> : ObjectGrainTest<T>, IUnitTest, IObjectTest
        where T : class, IGrainState
    {
        public override Task<string> VirtualCall() { return Task.FromResult("Virtual call from unit"); }
        public Task<string> UnitCall() { return Task.FromResult("Call from unit"); }

    }

    [StorageProvider(ProviderName = "Default")]
    public class ObjectGrainTest<T> : Grain<T>, IObjectTest
        where T : class, IGrainState
    {
        public virtual Task<string> VirtualCall() { return Task.FromResult("Virtual call from object"); }
        public Task<string> ObjectCall() { return Task.FromResult("Call from object"); }
    }
}
