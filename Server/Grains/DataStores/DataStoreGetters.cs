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

        public Task<PlayerClassLevelStats> GetPlayerClassLevelStats(UInt32 Class, UInt32 Level)
        {
            var entry = _PlayerClassLevelStats.Get(Class, Level);
            return Task.FromResult(entry);
        }

        public Task<PlayerCreateInfoAction[]> GetPlayerCreateInfoAction(UInt32 Class, UInt32 Race)
        {
            var entry = _PlayerCreateInfoAction.GetArray(Class, Race);
            return Task.FromResult(entry);
        }

        public Task<PlayerCreateInfoItem[]> GetPlayerCreateInfoItem(UInt32 Class, UInt32 Race)
        {
            var entry = _PlayerCreateInfoItem.GetArray(Class, Race);
            return Task.FromResult(entry);
        }

        public Task<PlayerCreateInfoSkills[]> GetPlayerCreateInfoSkills(UInt32 Class, UInt32 Race)
        {
            var ourClassMask = (UInt32)(1 << (int)Class);
            var ourRaceMask = (UInt32)(1 << (int)Race);
            var entry = _PlayerCreateInfoSkills.Get();

            var results = entry.Where(e => (e.raceMask == 0 || (e.raceMask & ourRaceMask) != 0) && (e.classMask == 0 || (e.classMask & ourClassMask) != 0));
            return Task.FromResult(results.ToArray());
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
