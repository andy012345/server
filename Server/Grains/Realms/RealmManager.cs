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
    public interface RealManagerState : IGrainState
    {
        Dictionary<int, Realm> RealmMap { get; set; }
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
        }
    }
}
