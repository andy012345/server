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
        int player_test { get; set; }
    }

    public class PlayerByNameIndexState : GrainState
    {
        public IPlayer PlayerPtr { get; set; }
    }

    [StorageProvider(ProviderName = "Default")]
    public class PlayerByNameIndex : Grain<PlayerByNameIndexState>, IPlayerByNameIndex
    {
        public Task<IPlayer> GetPlayer() { return Task.FromResult(State.PlayerPtr); }
        public async Task Save() { await WriteStateAsync(); }

        public async Task<bool> SetPlayer(IPlayer plr)
        {
            if (State.PlayerPtr != plr)
                return false;
            State.PlayerPtr = plr;
            await Save();
            return true;
        }
    }

    public class PlayerImpl : Player, IPlayer { }
    public class Player : Unit<PlayerData>, IPlayerImpl
    {
        /*public byte HairStyle
        {
           // get { return State.}
        }*/

        public override Task<string> VirtualCall() { return Task.FromResult("Virtual call from player"); }
        public Task<string> PlayerCall() { return Task.FromResult("Call from player"); }

        public Task Create()
        {
            SetObjectType(ObjectType.Player);
            return TaskDone.Done;
        }

        public async Task Create(CMSG_CHAR_CREATE creationData)
        {
            await Create();
            Gender = creationData.Gender;
            Skin = creationData.Skin;
            Face = creationData.Face;
            HairStyle = creationData.HairStyle;
            HairColor = creationData.HairColor;
            FacialHair = creationData.FacialHair;
            //Outfit = creationData.HairColor;
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
    }
}
