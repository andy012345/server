using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;
using System;
using Server;
using Shared;

namespace Server
{
    public interface IDataStoreManager : IGrainWithIntegerKey
    {
        Task<string> GetConnectionString();

        Task<PlayerCreateInfo> GetPlayerCreateInfo(UInt32 Class, UInt32 Race);
        Task<ChrClasses> GetChrClasses(UInt32 Class);
        Task<ChrRaces> GetChrRaces(UInt32 Race);
    }
}
