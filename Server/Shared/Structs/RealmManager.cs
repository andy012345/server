using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shared
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
        public int Cat = 1;

        public UInt16 Port = 9002;
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
        public RealmStatus RealmStatus = new RealmStatus();

        public float GetPopulationStatus()
        {
            return 1.0f; //todo, i believe 0 = new, 1 = medium, 2 = high, 3 = full
        }

        public void PingStatus() { RealmStatus.PingStatus(); }
        public bool IsOnline() { return RealmStatus.IsOnline(); }
        public void SetOffline() { RealmStatus.SetOffline(); }
    }
}
