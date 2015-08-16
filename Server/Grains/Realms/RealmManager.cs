using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using Orleans.Storage;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Shared;
using System.Linq;

namespace Server
{
    public class RealmMapKey
    {
        public UInt64 _key = 0;

        public RealmMapKey(UInt32 MapID, UInt32 RealmID)
        {
            _key = (UInt64)MapID;
            _key |= ((UInt64)RealmID) << 32;
        }

        public override int GetHashCode()
        {
            return _key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var asrealmkey = obj as RealmMapKey;
            if (asrealmkey != null)
                return _key.Equals(asrealmkey._key);
            return _key.Equals(obj);
        }
    }

    public class EqualityComparer : IEqualityComparer<RealmMapKey>
    {
        public bool Equals(RealmMapKey x, RealmMapKey y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(RealmMapKey x)
        {
            return x.GetHashCode();
        }

    }


    public class RealManagerState : GrainState
    {
        public Dictionary<int, Realm> RealmMap { get; set; }

        Dictionary<UInt64, List<UInt32>> _RealmInstances = new Dictionary<UInt64, List<UInt32>>();
        public Dictionary<UInt64, List<UInt32>> RealmInstaces { get { return _RealmInstances; } set { if (value != null) _RealmInstances = value; } }
    }

    [Reentrant]
    [StorageProvider(ProviderName = "Default")]
    public class RealmManager : Grain<RealManagerState>, IRealmManager
    {
        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            if (State.RealmMap == null)
                State.RealmMap = new Dictionary<int, Realm>();
        }

        public Task<Realm> GetRealm(int id)
        {
            Realm realm;
            if (State.RealmMap.TryGetValue(id, out realm))
                return Task.FromResult(realm);
            return Task.FromResult<Realm>(null);
        }

        public async Task RemoveRealm(int id)
        {
            State.RealmMap.Remove(id);
            await WriteStateAsync();
        }

        public async Task AddRealm(RealmSettings r)
        {
            var current = await GetRealm(r.ID);

            if (current != null)
                current.RealmSettings = r;
            else
            {
                Realm realm = new Realm();
                realm.RealmSettings = r;
                State.RealmMap.Add(r.ID, realm);
            }

            await WriteStateAsync();
        }

        public Task<Realm[]> GetRealms(int AccountLevel = 0)
        {
            var realms = State.RealmMap.Values.Where(rlm => rlm.RealmSettings.RequiredAccountLevel <= AccountLevel).ToArray();
            return Task.FromResult(realms);
        }

        public async Task PingRealm(int id)
        {
            var realm = await GetRealm(id);

            if (realm == null)
                return;

            realm.PingStatus();
            await WriteStateAsync();
        }

        public async Task<IMap> GetMap(UInt32 MapID, UInt32 InstanceID, UInt32 RealmID)
        {
            if (InstanceID != 0)
            {
                //just try and return the instance
                var map = GrainFactory.GetGrain<IMap>(InstanceID);
                if (await map.IsValid())
                    return map;
            }

            var realm = await GetRealm((int)RealmID);

            if (realm == null)
                return null;

            var datastore = GrainFactory.GetGrain<IDataStoreManager>(0);
            var mapentry = await datastore.GetMapEntry(MapID);

            if (mapentry == null)
                return null;

            //This API is for world maps only
            if (!mapentry.IsInstance())
                return await GetMap(MapID, RealmID);
            return null; //todo spawn instances
        }

        public async Task<IMap> GetMap(UInt32 MapID, UInt32 RealmID)
        {
            var realm = await GetRealm((int)RealmID);

            if (realm == null)
                return null;

            var datastore = GrainFactory.GetGrain<IDataStoreManager>(0);
            var mapentry = await datastore.GetMapEntry(MapID);

            if (mapentry == null)
                return null;

            //This API is for world maps only
            if (mapentry.IsInstance())
                return null;

            var key = new RealmMapKey(MapID, RealmID);

            IMap map = null;

            if (State.RealmInstaces.ContainsKey(key._key))
            {
                //todo: load balance? whatever do in future
                map = GrainFactory.GetGrain<IMap>((Int64)State.RealmInstaces[key._key].First());
                if (!(await map.IsValid())) //if not valid, dont use
                    map = null;
            }

            //if map is null, create and assign
            if (map == null)
                map = await CreateMap(MapID, RealmID);

            return map;
        }

        public Task AddMap(UInt32 MapID, UInt32 InstanceID, UInt32 RealmID)
        {
            var key = new RealmMapKey(MapID, RealmID);
            if (!State.RealmInstaces.ContainsKey(key._key))
                State.RealmInstaces.Add(key._key, new List<UInt32>());
            State.RealmInstaces[key._key].Add(InstanceID);
            return TaskDone.Done;
        }

        public async Task<IMap> CreateMap(UInt32 MapID, UInt32 RealmID)
        {
            var creator = GrainFactory.GetGrain<ICreator>(0);
            var instanceid = await creator.GenerateInstanceID();
            var map = GrainFactory.GetGrain<IMap>((Int64)instanceid);
            await map.Create(MapID, instanceid, RealmID);
            await AddMap(MapID, instanceid, RealmID);
            return map;
        }
    }
}
