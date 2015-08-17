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
        DataStore<PlayerCreateInfoAction> _PlayerCreateInfoAction = new DataStore<PlayerCreateInfoAction>("playercreateinfo_action", "class", "race");
        DataStore<PlayerCreateInfoItem> _PlayerCreateInfoItem = new DataStore<PlayerCreateInfoItem>("playercreateinfo_item", "class", "race");
        DataStore<PlayerCreateInfoSkills> _PlayerCreateInfoSkills = new DataStore<PlayerCreateInfoSkills>("playercreateinfo_skills");
        DataStore<PlayerCreateInfoSpellCustom> _PlayerCreateInfoSpellCustom = new DataStore<PlayerCreateInfoSpellCustom>("playercreateinfo_spell_custom");
        DataStore<PlayerClassLevelStats> _PlayerClassLevelStats = new DataStore<PlayerClassLevelStats>("player_classlevelstats", "class", "level");
        DataStore<PlayerXPForLevel> _PlayerXPForLevel = new DataStore<PlayerXPForLevel>("player_xp_for_level", "Level");
        DataStore<PlayerLevelStats> _PlayerLevelStats = new DataStore<PlayerLevelStats>("player_levelstats", "class", "race");

        public async Task Load()
        {
            if (Loaded)
                return;
            await LoadConnectionDetails();

            List<Task> loaders = new List<Task>();

            loaders.Add(LoadDBC());
            loaders.Add(_PlayerCreateInfo.Load(ConnectionString));
            loaders.Add(_PlayerCreateInfoAction.Load(ConnectionString));
            loaders.Add(_PlayerCreateInfoItem.Load(ConnectionString));
            loaders.Add(_PlayerCreateInfoSkills.Load(ConnectionString));
            loaders.Add(_PlayerCreateInfoSpellCustom.Load(ConnectionString));
            loaders.Add(_PlayerClassLevelStats.Load(ConnectionString));
            loaders.Add(_PlayerXPForLevel.Load(ConnectionString));
            loaders.Add(_PlayerLevelStats.Load(ConnectionString));

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
