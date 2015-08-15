using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;
using System;
using Server;

namespace Server
{
    public interface IMap : IGrainWithIntegerKey
    {
        Task<UInt32> GetMapID();
        Task<UInt32> GetInstanceID();
        Task Create(UInt32 MapID, UInt32 InstanceID, UInt32 RealmID);
    }
}
