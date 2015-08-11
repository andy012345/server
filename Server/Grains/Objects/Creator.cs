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
        public UInt64 MaxPlayerGuid = 0;
        public UInt64 MaxObjectGuid = 0;
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

        private Task<ObjectGUID> GeneratePlayerGUID()
        {
            var guid = new ObjectGUID(State.MaxPlayerGuid);
            State.MaxPlayerGuid += 1;
            return Task.FromResult(guid);
        }

        public async Task<IPlayer> CreatePlayer(CMSG_CHAR_CREATE creationData)
        {
            var guid = await GeneratePlayerGUID();
            var plr = GrainFactory.GetGrain<IPlayer>(guid.ToInt64());

            await plr.Create(creationData);

            return plr;
        }
    }
}
