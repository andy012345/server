using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using Orleans.Storage;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Linq;
using Shared;
using MySql.Data.MySqlClient;

namespace Server
{
    [Reentrant]
    [StatelessWorker]
    public partial class DataStoreManager : Grain, IDataStoreManager
    {
        bool Loaded = false;
        string ConnectionString = null;

        public Task<string> GetConnectionString() { return Task.FromResult(ConnectionString); }

        public override async Task OnActivateAsync()
        {
            await Load();
            await base.OnActivateAsync();
        }
        
        public async Task LoadConnectionDetails()
        {
            if (System.IO.File.Exists("Config-Server.xml") == false)
                return;

            TextReader configReader = File.OpenText("Config-Server.xml");

            if (configReader == null)
                throw new Exception("Unable to open Config-Server.xml");

            var doc = new XmlDocument();
            var xmlReader = XmlReader.Create(configReader);

            await Task.Factory.StartNew(() => { doc.Load(xmlReader); });

            var els = doc.GetElementsByTagName("DataStore");

            if (els.Count == 0)
                throw new Exception("Cannot find configuration details");

            var el = els[0] as XmlElement;

            string str = el.GetAttribute("DataConnectionString");

            if (str == null)
                throw new Exception("Config is missing connection details");

            ConnectionString = str;
        }
    }
}
