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
        public Task<PlayerCreateInfo> GetPlayerCreateInfo(UInt32 Class, UInt32 Race)
        {
            var entry = _PlayerCreateInfo.Get(Class, Race);

            return Task.FromResult(entry);
        }
    }
}
