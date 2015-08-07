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

namespace Server
{
    public class RealmSettings
    {
        public int ID;
        public string Address;
        public string Name;
        public string Lol;
        public int RealmID; //The real ID, for example, multiple front ends can point to the same realm (for debug testing around NATs mainly)
        public int RequiredAccountLevel = 0; //required account level to see this, default 0. Allows dev realms to be on the auth server but hidden from normal users.    
    }

    public class RealmStatus
    {

    }

    public class Realm
    {
        public RealmSettings RealmSettings = null;
        public RealmStatus RealmStatus = null;
    }

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
            return Task.FromResult(null);
        }

        public Task RemoveRealm(int id)
        {
            State.RealmMap.Remove(id);
            return TaskDone.Done;
        }

        public Task AddRealm(Realm r)
        {
            var current = GetRealm(r.ID);

            if (current != null)
                RemoveRealm(r.ID);



            return TaskDone.Done;
        }
    }
}
