using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Server
{
    public partial class DataStoreManager
    {
        public Task<MapEntry> GetMapEntry(UInt32 MapID)
        {
            var entry = _MapStore.Get(MapID);
            return Task.FromResult(entry);
        }

        public Task<PlayerCreateInfo> GetPlayerCreateInfo(UInt32 Class, UInt32 Race)
        {
            var entry = _PlayerCreateInfo.Get(Class, Race);

            return Task.FromResult(entry);
        }

        public Task<ChrClasses> GetChrClasses(UInt32 Class)
        {
            return Task.FromResult(_ChrClassesStore.Get(Class));
        }

        public Task<ChrRaces> GetChrRaces(UInt32 Race)
        {
            return Task.FromResult(_ChrRacesStore.Get(Race));
        }
    }
}
