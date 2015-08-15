using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Server
{
    public interface PlayerData : UnitData, IGrainState
    {
        string Name { get; set; }
        string Account { get; set; }
        int Race { get; set; }
        int Class { get; set; }
        int Gender { get; set; }
        int RealmID { get; set; }
    }

    public class PlayerByNameIndexState : GrainState
    {
        ObjectGUID _GUID = new ObjectGUID(0);

        public ObjectGUID GUID { get { return _GUID; } set { _GUID = value; } }
    }

    [StorageProvider(ProviderName = "Default")]
    public class PlayerByNameIndex : Grain<PlayerByNameIndexState>, IPlayerByNameIndex
    {
        public override Task OnActivateAsync()
        {
            return base.OnActivateAsync();
        }

        public Task<ObjectGUID> GetPlayerGUID() { return Task.FromResult(State.GUID); }
        public Task<IPlayer> GetPlayer() { return Task.FromResult(GrainFactory.GetGrain<IPlayer>(State.GUID.ToInt64())); }
        public async Task Save() { await WriteStateAsync(); }

        public async Task<bool> SetPlayer(IPlayer plr)
        {
            ObjectGUID plrGUID = new ObjectGUID((UInt64)plr.GetPrimaryKeyLong());
            if (State.GUID != 0)
                return false;
            State.GUID = plrGUID;
            await Save();
            return true;
        }
    }

    public class PlayerImpl : Player, IPlayer { }
    public class Player : Unit<PlayerData>, IPlayerImpl
    {
        IAccount _Account = null;
        SessionStream _Stream = null;

        public override Task OnActivateAsync()
        {
            if (State.Account != null)
                _Account = GrainFactory.GetGrain<IAccount>(State.Account);
            return base.OnActivateAsync();
        }
 
        public override Task<string> VirtualCall() { return Task.FromResult("Virtual call from player"); }
        public Task<string> PlayerCall() { return Task.FromResult("Call from player"); }

        public Task Create()
        {
            SetObjectType(ObjectType.Player);
            return TaskDone.Done;
        }

        public Task Kick()
        {
            return TaskDone.Done;
        }

        public Task<string> GetAccount() { return Task.FromResult(State.Account); }
        public async Task OnLogin()
        {
            var account = GrainFactory.GetGrain<IAccount>(State.Account);
            var session = await account.GetRealmSession();
            _Stream = await session.GetSessionStream(); //cache this to send packets without tonnes of copying
        }

        public async Task SendPacket(PacketOut p)
        {
            if (_Stream == null)
                return;
            await _Stream.PacketStream.OnNextAsync(p.strm.ToArray());
        }

        public async Task<LoginErrorCode> Create(PlayerCreateData info)
        {
            await Create();
            GUID = (UInt64)this.GetPrimaryKeyLong(); //create cached variables

            State.Name = info.CreateData.Name;
            State.Account = info.AccountName;
            _Account = GrainFactory.GetGrain<IAccount>(State.Account);
            State.Race = info.CreateData.Race;
            State.Class = info.CreateData.Class;
            State.Gender = info.CreateData.Gender;
            State.RealmID = info.RealmID;
            Gender = info.CreateData.Gender;
            Skin = info.CreateData.Skin;
            Face = info.CreateData.Face;
            HairStyle = info.CreateData.HairStyle;
            HairColor = info.CreateData.HairColor;
            FacialHair = info.CreateData.FacialHair;
            Type = (int)TypeMask.TYPEMASK_PLAYER;

            var datastore = GrainFactory.GetGrain<IDataStoreManager>(0);
            var chrclass = await datastore.GetChrClasses(info.CreateData.Class);
            var chrrace = await datastore.GetChrRaces(info.CreateData.Race);
            var creationinfo = await datastore.GetPlayerCreateInfo(info.CreateData.Class, info.CreateData.Race);

            if (chrclass == null || chrrace == null || creationinfo == null)
                return LoginErrorCode.CHAR_CREATE_ERROR;

            //just set male model for now
            DisplayID = (int)chrrace.ModelMale;
            NativeDisplayID = (int)chrrace.ModelMale;

            State.PositionX = creationinfo.position_x;
            State.PositionY = creationinfo.position_y;
            State.PositionZ = creationinfo.position_z;

            State.Exists = true; //WE EXIST, YAY

            await Save();

            return LoginErrorCode.CHAR_CREATE_SUCCESS;
        }

        public Task<PacketOut> BuildEnum()
        {
            PacketOut p = new PacketOut();
            if (_IsValid())
            {
                p.Write(oGUID);
                p.WriteCString(State.Name);
                p.Write((byte)State.Race);
                p.Write((byte)State.Class);
                p.Write(Gender);
                p.Write(Skin);
                p.Write(Face);
                p.Write(HairStyle);
                p.Write(HairColor);
                p.Write(FacialHair);
                p.Write((byte)1); //level to do
                p.Write((int)0); //zone to do
                p.Write((int)0); //map to do
                p.Write(State.PositionX); //positionx
                p.Write(State.PositionY); //positiony
                p.Write(State.PositionZ); //positionz
                p.Write((int)0); //guildid
                p.Write((int)0); //charflags (stuff like hide helm, is a ghost from death, needs a rename, is locked)
                p.Write((int)0); //customisationflags (race change, faction change etc)
                p.Write((byte)0); //firstlogin
                p.Write((int)0); //petdisplayid
                p.Write((int)0); //petlevel
                p.Write((int)0); //petfamilyid

                //this is a loop of the item display slots, to do when implementing items
                for (int i = 0; i < 23; ++i)
                {
                    p.Write((int)0); //item display
                    p.Write((byte)0); //inventory type
                    p.Write((int)0); //enchantment
                }
            }
            else
            {
                p.Write((UInt64)0); //guid
                p.WriteCString("Unknown"); //name
                p.Write((byte)0); //race
                p.Write((byte)0); //class
                p.Write((byte)0); //gender
                p.Write((byte)0); //skin
                p.Write((byte)0); //face 
                p.Write((byte)0); //hairstyle
                p.Write((byte)0); //haircolor
                p.Write((byte)0); //facialhair
                p.Write((byte)1); //level
                p.Write((int)0); //zoneid
                p.Write((int)0); //mapid
                p.Write((float)0); //positionx
                p.Write((float)0); //positiony
                p.Write((float)0); //positionz
                p.Write((int)0); //guildid
                p.Write((int)0); //charflags
                p.Write((int)0); //customisationflags
                p.Write((byte)0); //firstlogin
                p.Write((int)0); //petdisplayid
                p.Write((int)0); //petlevel
                p.Write((int)0); //petfamilyid

                //this is a loop of the item display slots, to do when implementing items
                for (int i = 0; i < 23; ++i)
                {
                    p.Write((int)0); //item display
                    p.Write((byte)0); //inventory type
                    p.Write((int)0); //enchantment
                }
            }


            return Task.FromResult(p);
        }

        #region Customisation

        public byte Skin
        {
            get { return _GetByte((int)EUnitFields.PLAYER_BYTES, 0); }
            set { _SetByte((int)EUnitFields.PLAYER_BYTES, 0, value); }
        }
        public byte Face
        {
            get { return _GetByte((int)EUnitFields.PLAYER_BYTES, 1); }
            set { _SetByte((int)EUnitFields.PLAYER_BYTES, 1, value); }
        }
        public byte HairStyle
        {
            get { return _GetByte((int)EUnitFields.PLAYER_BYTES, 2); }
            set { _SetByte((int)EUnitFields.PLAYER_BYTES, 2, value); }
        }
        public byte HairColor
        {
            get { return _GetByte((int)EUnitFields.PLAYER_BYTES, 3); }
            set { _SetByte((int)EUnitFields.PLAYER_BYTES, 3, value); }
        }
        public byte FacialHair
        {
            get { return _GetByte((int)EUnitFields.PLAYER_BYTES_2, 0); }
            set { _SetByte((int)EUnitFields.PLAYER_BYTES_2, 0, value); }
        }
        public byte Gender
        {
            get { return _GetByte((int)EUnitFields.PLAYER_BYTES_3, 0); }
            set { _SetByte((int)EUnitFields.PLAYER_BYTES_3, 0, value); }
        }

        #endregion

        public override Task<bool> IsCellActivator() { return Task.FromResult(true); }
    }
}
