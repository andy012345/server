using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;
using System;
using Server;
using Shared;

namespace Server
{
    public interface ICreator : IGrainWithIntegerKey
    {
        Task<UInt32> GenerateInstanceID();
        Task<ObjectGUID> GenerateGUID(ObjectType objectType);

        Task<Tuple<LoginErrorCode, IPlayer>> CreatePlayer(PlayerCreateData info);
    }
}
