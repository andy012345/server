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
    public class DataStore<DataType>
    {
        string TableName = null;
        List<string> TableIndexes = new List<string>();
        List<DataType> TableDataIndex0 = null; //used if 0 index
        Dictionary<UInt32, List<DataType>> TableDataIndex1 = null; //used if 1 index
        Dictionary<UInt32, Dictionary<UInt32, List<DataType>>> TableDataIndex2 = null; //used if 2 indexes

        public DataStore(string table) { TableName = table; TableDataIndex0 = new List<DataType>(); }
        public DataStore(string table, string index) { TableName = table; TableDataIndex1 = new Dictionary<uint, List<DataType>>(); TableIndexes.Add(index); }
        public DataStore(string table, string index, string index2) { TableName = table; TableDataIndex2 = new Dictionary<uint, Dictionary<uint, List<DataType>>>(); TableIndexes.Add(index); TableIndexes.Add(index2); }

        public async Task Load(string constring)
        {
            await LoadImpl(await CreateConnection(constring));
        }

        async Task LoadImpl(MySqlConnection connection)
        {
            string query = string.Format("select * from `{0}`;", MySqlHelper.EscapeString(TableName));

            MySqlCommand cmd = new MySqlCommand(query, connection);
            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();

                for (int i = 0; i < reader.FieldCount; ++i)
                    dict.Add(reader.GetName(i), reader.GetValue(i));

                UInt32 IndexValue = 0;
                DataType data = MySQLGenerator.ToObject<DataType>(dict);

                if (TableIndexes.Count == 0)
                    Add(data);
                if (TableIndexes.Count == 1)
                {
                    if (!dict.ContainsKey(TableIndexes[0]))
                        throw new Exception("Table has incorrect index");
                    Add(Convert.ToUInt32(dict[TableIndexes[0]]), data);
                }

                if (TableIndexes.Count == 2)
                {
                    if (!dict.ContainsKey(TableIndexes[0]) || !dict.ContainsKey(TableIndexes[1]))
                        throw new Exception("Table has incorrect index");
                    Add(Convert.ToUInt32(dict[TableIndexes[0]]), Convert.ToUInt32(dict[TableIndexes[1]]), data);
                }


                Add(IndexValue, data);
            }

            reader.Dispose();
            cmd.Dispose();
            connection.Dispose();
        }

        async Task<MySqlConnection> CreateConnection(string ConnectionString)
        {
            var DatabaseConnection = new MySqlConnection(ConnectionString);
            await DatabaseConnection.OpenAsync();

            if (DatabaseConnection != null && DatabaseConnection.State != System.Data.ConnectionState.Open)
                throw new Exception("DataStore could not open a connection to the database");

            return DatabaseConnection;
        }

        void Add(DataType data)
        {
            Add(0, data);
        }

        void Add(UInt32 index, DataType data)
        {
            if (TableDataIndex1.ContainsKey(index))
                TableDataIndex1[index].Add(data);
            else
            {
                List<DataType> l = new List<DataType>();
                l.Add(data);
                TableDataIndex1.Add(index, l);
            }
        }
        void Add(UInt32 index, UInt32 index2, DataType data)
        {
            if (TableDataIndex2.ContainsKey(index))
            {
                if (TableDataIndex2[index].ContainsKey(index2))
                    TableDataIndex2[index][index2].Add(data);
                else
                {
                    List<DataType> l = new List<DataType>();
                    l.Add(data);
                    TableDataIndex2[index].Add(index2, l);
                }
            }
            else
            {
                Dictionary<UInt32, List<DataType>> d = new Dictionary<uint, List<DataType>>();
                List<DataType> l = new List<DataType>();
                l.Add(data);
                d.Add(index2, l);
                TableDataIndex2.Add(index, d);
            }
        }

        public DataType[] Get()
        {
            if (TableDataIndex0 == null)
                return null;
            return TableDataIndex0.ToArray();
        }

        public DataType Get(UInt32 index)
        {
            if (TableDataIndex1 == null || !TableDataIndex1.ContainsKey(index) || TableDataIndex1[index].Count == 0)
                return default(DataType);
            return TableDataIndex1[index][0];
        }

        public DataType[] GetArray(UInt32 index)
        {
            if (TableDataIndex1 == null || !TableDataIndex1.ContainsKey(index) || TableDataIndex1[index].Count == 0)
                return null;
            return TableDataIndex1[index].ToArray();
        }

        public DataType Get(UInt32 index, UInt32 index2)
        {
            if (TableDataIndex2 == null || !TableDataIndex2.ContainsKey(index) || TableDataIndex2[index].ContainsKey(index2) || TableDataIndex2[index][index2].Count == 0)
                return default(DataType);
            return TableDataIndex2[index][index2][0];
        }

        public DataType[] GetArray(UInt32 index, UInt32 index2)
        {
            if (TableDataIndex2 == null || !TableDataIndex2.ContainsKey(index) || TableDataIndex2[index].ContainsKey(index2) || TableDataIndex2[index][index2].Count == 0)
                return null;
            return TableDataIndex2[index][index2].ToArray();
        }
    }
}
