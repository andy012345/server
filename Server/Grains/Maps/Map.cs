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
        public UInt32 InstanceID { get; set; } //for instanced maps
        public UInt32 RealmID { get; set; } //for non instanced maps
        public UInt32 ExpireTime { get; set; }
        public bool Exists { get; set; }
    }

    [Reentrant]
    [StorageProvider(ProviderName = "Default")]
    public class Map : Grain<MapState>, IMap
    {
        Dictionary<ObjectGUID, IObject> objectMap = new Dictionary<ObjectGUID, IObject>();
        List<IObject> activeObjects = new List<IObject>();

        public override async Task OnActivateAsync()
        {
            if (!_IsValid())
            {
                //Non instanced maps can create themselves
                var mapid = (UInt32)(this.GetPrimaryKeyLong() & 0xFFFFFFFF);
                var realmid = (UInt32)((this.GetPrimaryKeyLong() >> 32) & 0xFFFFFFFF);

                var datastore = GrainFactory.GetGrain<IDataStoreManager>(0);
                var mapentry = await datastore.GetMapEntry(mapid);

                //just create world maps as people go into them if they dont exist
                if (mapentry != null && !mapentry.IsInstance())
                    await Create(mapid, 0, realmid);
            }
            await base.OnActivateAsync();
        }

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
        public async Task Save() { if (_IsValid()) return; await WriteStateAsync(); }

        public IObject GetObjectByGUID(ObjectGUID guid)
        {
            IObject ret = null;
            objectMap.TryGetValue(guid, out ret);
            return ret;
        }

        public IObject GetObjectByGUID(UInt64 guid)
        {
            return GetObjectByGUID(new ObjectGUID(guid));
        }

        public Task<UInt32> GetMapID() { return Task.FromResult(State.MapID); }
        public Task<UInt32> GetInstanceID() { return Task.FromResult(State.InstanceID); }

        async Task<bool> AddObject(IObject obj)
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
