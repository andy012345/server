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

            loaders.Add(LoadDBC());
            loaders.Add(_PlayerCreateInfo.Load(ConnectionString));

            await Task.WhenAll(loaders);
        }


        CharStartOutfitStore _CharacterOutfitStore = new CharStartOutfitStore();
        ChrClassesStore _ChrClassesStore = new ChrClassesStore();
        ChrRacesStore _ChrRacesStore = new ChrRacesStore();
        MapStore _MapStore = new MapStore();

        public async Task LoadDBC()
        {
            await _CharacterOutfitStore.Load("CharStartOutfit.dbc");
            await _ChrClassesStore.Load("ChrClasses.dbc");
            await _ChrRacesStore.Load("ChrRaces.dbc");
            await _MapStore.Load("Map.dbc");
        }
    }
}
