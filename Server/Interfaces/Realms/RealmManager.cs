using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;
using System;
using Server;
using Shared;

namespace Server
{
    public interface IRealmManager : IGrainWithIntegerKey
    {
        Task<Realm> GetRealm(int id);
        Task RemoveRealm(int id);
        Task AddRealm(RealmSettings r);
        Task<Realm[]> GetRealms(int AccountLevel = 0);
        Task PingRealm(int id);
    }
}
