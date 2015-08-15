using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using Orleans.Concurrency;
using Orleans.Providers;

namespace Server
{
    public class ObjectCreatorState : GrainState
    {
        UInt64 _MaxPlayerGuid = 1;
        UInt64 _MaxObjectGuid = 1;
        UInt32 _MaxInstanceGuid = 1;

        public UInt64 MaxPlayerGUID { get { return _MaxPlayerGuid; } set { _MaxPlayerGuid = value; } }
        public UInt64 MaxObjectGUID { get { return _MaxObjectGuid; } set { _MaxObjectGuid = value; } }
        public UInt32 MaxInstanceGUID { get { return _MaxInstanceGuid; } set { _MaxInstanceGuid = value; } }
    }

    [Reentrant]
    [StorageProvider(ProviderName = "Default")]
    class ObjectCreator : Grain<ObjectCreatorState>, IObjectCreator
    {
        public async Task<ObjectGUID> GenerateGUID(ObjectType objectType)
        {
            switch (objectType)
            {
                case ObjectType.Player: return await GeneratePlayerGUID();
                default: return new ObjectGUID(0);
            }
        }

        public Task<UInt32> GenerateInstanceID()
        {
            var ret = State.MaxInstanceGUID;
            State.MaxInstanceGUID += 1;
            return Task.FromResult(ret);
        }

        public async Task<IMap> CreateInstance(UInt32 MapID, UInt32 RealmID)
        {
            var datastore = GrainFactory.GetGrain<IDataStoreManager>(0);

            var mapentry = await datastore.GetMapEntry(MapID);

            if (mapentry == null)
                return null;

            UInt64 mapkey = (UInt64)MapID;
            UInt32 instanceid = 0;

            if (mapentry.IsInstance())
            {
                instanceid = await GenerateInstanceID();
                mapkey |= ((UInt64)instanceid << 32);
            }
            else
                mapkey |= ((UInt64)RealmID << 32);

            var map = GrainFactory.GetGrain<IMap>((long)mapkey);
            map.Create(MapID, instanceid, RealmID);
            return map;
        }

        private Task<ObjectGUID> GeneratePlayerGUID()
        {
            var guid = new ObjectGUID(State.MaxPlayerGUID | (UInt64)HighGuid.HIGHGUID_PLAYER);
            State.MaxPlayerGUID += 1;
            return Task.FromResult(guid);
        }

        public async Task<Tuple<LoginErrorCode, IPlayer>> CreatePlayer(PlayerCreateData info)
        {
            LoginErrorCode err = LoginErrorCode.CHAR_CREATE_SUCCESS;
            var guid = await GeneratePlayerGUID();
            var plr = GrainFactory.GetGrain<IPlayer>(guid.ToInt64());

            err = await plr.Create(info);

            var ret = new Tuple<LoginErrorCode, IPlayer>(err, err == LoginErrorCode.CHAR_CREATE_SUCCESS ? plr : null);
            return ret;
        }
    }
}
