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
    public class RealmSettings
    {
        public int ID;
        public string Address;
        public string Name;
        public string Lol;
        public int RealmID; //The real ID, for example, multiple front ends can point to the same realm (for debug testing around NATs mainly)
        public int RequiredAccountLevel = 0; //required account level to see this, default 0. Allows dev realms to be on the auth server but hidden from normal users.    
        public int MaxPlayers = 1000;
        public int Cat = 0;
    }

    public class RealmStatus
    {
        public int CurrentPlayers = 0;
        DateTime LastPing;
        public void PingStatus() { LastPing = DateTime.UtcNow; }

        public bool IsOnline()
        {
            TimeSpan elapsed = new TimeSpan(DateTime.UtcNow.Ticks - LastPing.Ticks);

            //If realm hasn't pinged for 5 minutes or more it's offline
            if (elapsed.TotalMinutes >= 5)
                return false;
            return true;
        }

        public void SetOffline() { LastPing = DateTime.UtcNow.AddMinutes(-10); }
    }

    public class Realm
    {
        public RealmSettings RealmSettings = null;
        public RealmStatus RealmStatus = null;

        public float GetPopulationStatus()
        {
            return 1.0f; //todo, i believe 0 = new, 1 = medium, 2 = high, 3 = full
        }

        public void PingStatus() { RealmStatus.PingStatus(); }
        public bool IsOnline() { return RealmStatus.IsOnline(); }
        public void SetOffline() { RealmStatus.SetOffline(); }
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
