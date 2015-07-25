using Orleans;
using Orleans.Runtime;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;

namespace Orleans.Storage.MySQLDB
{
    using System.Configuration;
    using System.Data.Common;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Providers;
    using Storage;
    using MySql.Data.MySqlClient;

    /// <summary>
    /// A MySQLDB storage provider.
    /// </summary>
    /// <remarks>
    /// The storage provider should be included in a deployment by adding this line to the Orleans server configuration file:
    /// <Provider Type="Orleans.Storage.MySQLDB.MySQLJSONDBStorageProvider" Name="MySQLJSONDBStore" ConnectionStringName="MySQLDB"/>and this line to any grain that uses it:
    /// [StorageProvider(ProviderName = "MySQLJSONDBStore")]
    /// The name 'MySQLDBStore' is an arbitrary choice.
    /// If no connection string name is provided the provider will use MySQLDB InMemory storage.
    /// </remarks>
    public class MySQLJSONDBStorageProvider : IStorageProvider
    {
        MySqlConnection DatabaseConnection;
        string ConnectString;
        string Table = "OrleansGrainStorage";
        bool CustomTable = false;

        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            Log = providerRuntime.GetLogger(this.GetType().FullName);

            ConnectString = config.Properties["DataConnectionString"];

            if (config.Properties.ContainsKey("Table"))
            {
                Table = config.Properties["Table"];
                CustomTable = true;
            }

            CheckDatabaseConnection().Wait();

            return TaskDone.Done;
        }

        public Logger Log { get; protected set; }

        /// <summary>
        /// Storage provider name
        /// </summary>
        public string Name { get; protected set; }

        public Task Close()
        {
            DatabaseConnection.Dispose();

            return TaskDone.Done;
        }

        public async Task CheckDatabaseConnection()
        {
            if (DatabaseConnection != null && DatabaseConnection.State == System.Data.ConnectionState.Open)
                return;

            //if we're not open, dispose of old connection and make a new one
            if (DatabaseConnection != null)
                DatabaseConnection.Dispose();
            DatabaseConnection = new MySqlConnection(ConnectString);
            await DatabaseConnection.OpenAsync();

            if (DatabaseConnection != null && DatabaseConnection.State != System.Data.ConnectionState.Open)
                throw new Exception("MySQLStorage could not open a connection to the database");
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            await CheckDatabaseConnection();

            var table = GetTableName(grainState);
            string keyAsString = GetKey(grainReference);

            string query;

            if (CustomTable)
                query = string.Format("select * from `{0}` where `guid` = \"{1}\";", table, MySqlHelper.EscapeString(keyAsString));
            else
                query = string.Format("select * from `{0}` where `guid` = \"{1}\" AND `type` = \"{2}\";", table, MySqlHelper.EscapeString(keyAsString), MySqlHelper.EscapeString(grainType));

            MySqlCommand cmd = new MySqlCommand(query, DatabaseConnection);
            Log.Verbose(query);
            var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();

                for (int i = 0; i < reader.FieldCount; ++i)
                    dict.Add(reader.GetName(i), reader.GetValue(i));

                if (dict.ContainsKey("data"))
                {
                    try
                    {
                        object data = Newtonsoft.Json.JsonConvert.DeserializeObject(dict["data"].ToString(), grainState.GetType());
                        grainState.SetAll(((IGrainState)data).AsDictionary());
                    }
                    catch (Exception e) { grainState.SetAll(null); /* corruption? */ }
                }
                else
                    grainState.SetAll(null);
            }

            reader.Dispose();
            cmd.Dispose();
        }

        private string GetTableName(IGrainState grainState)
        {
            if (Table != null && Table.Length > 0)
                return Table;
            return grainState.GetType().Name; //use grain name generator if no table provided
        }

        private static string GetKey(GrainReference grainReference)
        {
            string keyAsString = null;
            string keyExt = null;

            bool success = true;

            try { keyAsString = grainReference.GetPrimaryKeyLong().ToString(); }
            catch (Exception e) { success = false; }
            if (!success)
            {
                keyAsString = grainReference.GetPrimaryKey(out keyExt).ToString();

                if (keyExt.Length > 0) //using string extension to guid, so we use the string!
                    keyAsString = keyExt;
            }

            if (keyAsString == null || keyAsString.Length == 0)
                throw new Exception(string.Format("MySQLStorage could not deduce key for grain reference {0}", grainReference.ToString()));
            return keyAsString;
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            await CheckDatabaseConnection();

            var table = GetTableName(grainState);
            var key = GetKey(grainReference);

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(grainState, Newtonsoft.Json.Formatting.Indented);

            string query;

            if (CustomTable)
                query = string.Format("replace into `{0}` (`guid`, `data`) VALUE (\"{1}\", \"{2}\");", table, key, MySqlHelper.EscapeString(data));
            else
                query = string.Format("replace into `{0}` (`guid`, `type`, `data`) VALUE (\"{1}\", \"{2}\", \"{3}\");", table, key, MySqlHelper.EscapeString(grainType), MySqlHelper.EscapeString(data));

            MySqlCommand com = new MySqlCommand(query, DatabaseConnection);
            Log.Verbose(query);
            await com.ExecuteNonQueryAsync();

            com.Dispose();
        }

        public async Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            await CheckDatabaseConnection();
            var table = GetTableName(grainState);
            var key = GetKey(grainReference);

            string query;
               
            if (CustomTable)
                query = string.Format("delete from `{0}` where `guid` = \"{1}\";", table, MySqlHelper.EscapeString(key));
            else
                query = string.Format("delete from `{0}` where `guid` = \"{1}\" AND `type` = \"{2}\";", table, MySqlHelper.EscapeString(key), MySqlHelper.EscapeString(grainType));

            MySqlCommand com = new MySqlCommand(query);
            Log.Verbose(query);
            await com.ExecuteNonQueryAsync();
        }

    }
}