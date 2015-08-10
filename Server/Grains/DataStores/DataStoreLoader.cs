using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public partial class DataStoreManager
    {
        DataStore<PlayerCreateInfo> _PlayerCreateInfo = new DataStore<PlayerCreateInfo>("playercreateinfo", "class", "race");

        public async Task Load()
        {
            if (Loaded)
                return;
            await LoadConnectionDetails();

            List<Task> loaders = new List<Task>();

            loaders.Add(_PlayerCreateInfo.Load(ConnectionString));
        }
    }
}
