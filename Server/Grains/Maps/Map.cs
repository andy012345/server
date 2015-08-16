using Orleans;
using Orleans.Providers;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Server
{
    public class MapState : GrainState
    {
        public UInt32 MapID { get; set; }
        public UInt32 InstanceID { get; set; } //our index accessor
        public UInt32 RealmID { get; set; } //for non instanced maps
        public UInt32 ExpireTime { get; set; }
        public bool Exists { get; set; }
    }

    [Reentrant]
    [StorageProvider(ProviderName = "Default")]
    public class Map : Grain<MapState>, IMap
    {
        Dictionary<ObjectGUID, IObjectImpl> objectMap = new Dictionary<ObjectGUID, IObjectImpl>();
        List<IObjectImpl> activeObjects = new List<IObjectImpl>();

        public async Task Create(UInt32 MapID, UInt32 InstanceID, UInt32 RealmID)
        {
            if (_IsValid()) //already created
                return;

            State.MapID = MapID;
            State.InstanceID = InstanceID;
            State.RealmID = RealmID;

            State.Exists = true;
            await Save();
        }

        public bool _IsValid() { return State.Exists; }
        public Task<bool> IsValid() { return Task.FromResult(_IsValid()); }
        public async Task Save() { if (_IsValid()) return; await WriteStateAsync(); }

        public IObjectImpl GetObjectByGUID(ObjectGUID guid)
        {
            IObjectImpl ret = null;
            objectMap.TryGetValue(guid, out ret);
            return ret;
        }

        public IObjectImpl GetObjectByGUID(UInt64 guid)
        {
            return GetObjectByGUID(new ObjectGUID(guid));
        }

        public Task<UInt32> GetMapID() { return Task.FromResult(State.MapID); }
        public Task<UInt32> GetInstanceID() { return Task.FromResult(State.InstanceID); }

        public async Task<bool> AddObject(IObjectImpl obj)
        {
            var guid = await obj.GetGUID();
            objectMap.Add(guid, obj);
            await obj.SetMap(this);

            var isactivator = await obj.IsCellActivator();

            if (isactivator)
            {
                //well he gets updates, duh
                activeObjects.Add(obj);

                //todo: add ref to 3x3 cell around him
            }

            return true;
        }

    }
}
