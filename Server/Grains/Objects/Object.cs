using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using Orleans.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Server
{
    public enum TypeID
    {
        TYPEID_OBJECT = 0,
        TYPEID_ITEM = 1,
        TYPEID_CONTAINER = 2,
        TYPEID_UNIT = 3,
        TYPEID_PLAYER = 4,
        TYPEID_GAMEOBJECT = 5,
        TYPEID_DYNAMICOBJECT = 6,
        TYPEID_CORPSE = 7
    }

    public interface ObjectData : IGrainState
    {
        bool Exists { get; set; }

        float PositionX { get; set; }
        float PositionY { get; set; }
        float PositionZ { get; set; }
        float Orientation { get; set; }

        UInt32 MapID { get; set; }
        UInt32 InstanceID { get; set; }

        ObjectType ObjType { get; set; }
        UpdateField[] UpdateFields { get; set; }

        ObjectUpdateFlags UpdateFlags { get; set; }
    }

    public class ObjectImpl : Object<ObjectData>, IObject
    {
    }
   
    public class Object<T> : BaseObject<T>, IObjectImpl
        where T : class, ObjectData
    {
        protected ObjectGUID oGUID = null;
        protected PackedGUID pGUID = null;

        public async override Task OnActivateAsync()
        {
            if (State.UpdateFields != null)
                GUID = (UInt64)this.GetPrimaryKeyLong(); //create cached variables

            await base.OnActivateAsync();
        }

        public Task<bool> IsValid() { return Task.FromResult(State.Exists); }
        public bool _IsValid() { return State.Exists; }

        public virtual IObjectImpl ToRef() { return this.AsReference<IObjectImpl>(); }

        public virtual Task<string> VirtualCall() { return Task.FromResult("Virtual call from object"); }
        public Task<string> ObjectCall() { return Task.FromResult("Call from object"); }

        public async Task Save() { if (State.Exists) await WriteStateAsync(); }

        public Task<ObjectGUID> GetGUID() { return Task.FromResult(oGUID); }
        public Task<PackedGUID> GetPackedGUID() { return Task.FromResult(pGUID); }

        public Task CreateUpdateFields(int sz)
        {
            State.UpdateFields = new UpdateField[sz];
            return TaskDone.Done;
        }

        public Task SetObjectType(ObjectType type)
        {
            State.ObjType = type;
            CreateUpdateFieldsByType(type);
            return TaskDone.Done;
        }

        public Task CreateUpdateFieldsByType(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Player: CreateUpdateFields((int)EUnitFields.PLAYER_END); break;
                case ObjectType.Creature: CreateUpdateFields((int)EUnitFields.UNIT_END); break;

                default: throw new Exception("Cannot to create Update Fields by unknown object type");
            }

            return TaskDone.Done;
        }

        #region UpdateFields

        public byte _GetByte(int field, int index) { return State.UpdateFields[field].GetByte(index); }
        public UInt32 _GetUInt32(int field) { return State.UpdateFields[field].GetUInt32(); }
        public Int32 _GetInt32(int field) { return State.UpdateFields[field].GetInt32(); }
        public UInt64 _GetUInt64(int field)
        {
            var low = _GetUInt32(field);
            var high = _GetUInt32(field + 1);

            var ret = (UInt64)low;
            ret |= (UInt64)high << 32;
            return ret;
        }
        public float _GetFloat(UInt32 field) { return State.UpdateFields[field].GetFloat(); }
        public void _SetByte(int field, int index, byte val) { State.UpdateFields[field].Set(index, val); }
        public void _SetUInt32(int field, UInt32 val) { State.UpdateFields[field].Set(val); }
        public void _SetInt32(int field, int val) { State.UpdateFields[field].Set(val); }
        public void _SetFloat(int field, float val) { State.UpdateFields[field].Set(val); }

        public void _SetGUID(int field, ObjectGUID val) { SetUInt64(field, val.ToUInt64()); }
        public void _SetUInt64(int field, UInt64 val)
        {
            UInt32 high = (UInt32)(val >> 32);
            UInt32 low = (UInt32)val;
            SetUInt32(field, low);
            SetUInt32(field + 1, high);
        }

        //Tasks for external people
        public Task<byte> GetByte(int field, int index) { return Task.FromResult(_GetByte(field, index)); }
        public Task<UInt32> GetUInt32(int field) { return Task.FromResult(_GetUInt32(field)); }
        public Task<float> GetFloat(UInt32 field) { return Task.FromResult(_GetFloat(field)); }
        public Task SetByte(int field, int index, byte val) { _SetByte(field, index, val); return TaskDone.Done; }
        public Task SetUInt32(int field, UInt32 val) { _SetUInt32(field, val); return TaskDone.Done; }
        public Task SetInt32(int field, int val) { _SetInt32(field, val); return TaskDone.Done; }
        public Task SetFloat(int field, float val) { _SetFloat(field, val); return TaskDone.Done; }
        public Task SetUInt64(int field, UInt64 val) { SetUInt64(field, val); return TaskDone.Done; }
        public Task SetGUID(int field, ObjectGUID val) { _SetGUID(field, val); return TaskDone.Done; }

        #endregion

        #region Updatefield Getters and Setters

        public int Type
        {
            get { return _GetInt32((int)EObjectFields.OBJECT_FIELD_TYPE); }
            set { _SetInt32((int)EObjectFields.OBJECT_FIELD_TYPE, value); }
        }

        public UInt64 GUID
        {
            get { return _GetUInt64((int)EObjectFields.OBJECT_FIELD_GUID); }
            set { _SetUInt64((int)EObjectFields.OBJECT_FIELD_GUID, value); oGUID = new ObjectGUID(value); pGUID = new PackedGUID(value); }
        }

       #endregion

        public async Task SetMap(IMap map)
        {
            State.MapID = await map.GetMapID();
            State.InstanceID = await map.GetInstanceID();

            var asref = ToRef();
            if (asref is IPlayer)
            {
                IPlayer plrthis = asref as IPlayer;
                await plrthis.BuildInitialUpdate();
            }
        }

        public virtual Task<bool> IsCellActivator() { return Task.FromResult(false); }

        public Task<PacketOut> BuildCreateUpdateFor(IPlayer plr)
        {
            ObjectUpdateType updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT;

            ObjectUpdateFlags updateFlags = State.UpdateFlags;

            if (plr.GetPrimaryKeyLong() == this.GetPrimaryKeyLong())
                updateFlags |= ObjectUpdateFlags.UPDATEFLAG_SELF;

            if ((updateFlags & ObjectUpdateFlags.UPDATEFLAG_STATIONARY_POSITION) != 0)
            {
                if (_IsPlayer())
                    updateType = ObjectUpdateType.UPDATETYPE_CREATE_OBJECT2;
            }

            PacketOut p = new PacketOut();
            p.Write((byte)updateType);
            p.Write(pGUID);
            p.Write((byte)Type);
            _BuildMovementUpdate(updateType, updateFlags, ref p);
            _BuildValuesUpdate(updateType, updateFlags, ref p, plr);
            return Task.FromResult(p);
        }

        void _BuildMovementUpdate(ObjectUpdateType type, ObjectUpdateFlags flags, ref PacketOut pkt)
        {
            var asUnit = ToRef() as IUnitImpl;

            pkt.Write((UInt16)flags);

            if ((flags & ObjectUpdateFlags.UPDATEFLAG_LIVING) != 0)
            {
                UInt32 moveflags = 0;
                UInt16 moveflagsex = 0;

                pkt.Write(moveflags);
                pkt.Write(moveflagsex);
                pkt.Write(Time.GetMSTime());
                pkt.Write(State.PositionX);
                pkt.Write(State.PositionY);
                pkt.Write(State.PositionZ);
                pkt.Write(State.Orientation);

                pkt.Write((UInt32)0); //fall time

                pkt.Write((float)1.0); //walk speed
                pkt.Write((float)1.0); //run speed
                pkt.Write((float)1.0); //runback speed
                pkt.Write((float)1.0); //swim speed
                pkt.Write((float)1.0); //swimback speed
                pkt.Write((float)1.0); //flight speed
                pkt.Write((float)1.0); //flightback speed
                pkt.Write((float)1.0); //turn rate
                pkt.Write((float)1.0); //pitch rate
            }
            else
            {
                if ((flags & ObjectUpdateFlags.UPDATEFLAG_POSITION) != 0)
                {
                    pkt.Write((byte)0); //packed guid of transport
                    pkt.Write(State.PositionX);
                    pkt.Write(State.PositionY);
                    pkt.Write(State.PositionZ);

                    pkt.Write((float)0); //transport offset x
                    pkt.Write((float)0); //transport offset y
                    pkt.Write((float)0); //transport offset z

                    pkt.Write(State.Orientation);
                }
                else
                {
                    if ((flags & ObjectUpdateFlags.UPDATEFLAG_STATIONARY_POSITION) != 0)
                    {
                        pkt.Write(State.PositionX);
                        pkt.Write(State.PositionY);
                        pkt.Write(State.PositionZ);
                        pkt.Write(State.Orientation);
                    }
                }
            }

            if ((flags & ObjectUpdateFlags.UPDATEFLAG_UNKNOWN) != 0)
            {
                pkt.Write((UInt32)0);
            }

            if ((flags & ObjectUpdateFlags.UPDATEFLAG_LOWGUID) != 0)
            {
                //lowguid of some items 2F or 8 for players
                TypeID myType = (TypeID)Type;

                switch (myType)
                {
                    case TypeID.TYPEID_OBJECT:
                    case TypeID.TYPEID_ITEM:
                    case TypeID.TYPEID_CONTAINER:
                    case TypeID.TYPEID_GAMEOBJECT:
                    case TypeID.TYPEID_DYNAMICOBJECT:
                    case TypeID.TYPEID_CORPSE:
                        {
                            pkt.Write((UInt32)GUID);
                        }
                        break;
                    case TypeID.TYPEID_UNIT: pkt.Write((UInt32)0xB); break;
                    case TypeID.TYPEID_PLAYER:
                        {
                            if ((flags & ObjectUpdateFlags.UPDATEFLAG_SELF) != 0)
                                pkt.Write((UInt32)0x2F);
                            else
                                pkt.Write((UInt32)0x8);
                        }
                        break;
                    default: pkt.Write((UInt32)0); break;
                }
            }

            if ((flags & ObjectUpdateFlags.UPDATEFLAG_HAS_TARGET) != 0)
            {
                pkt.Write((byte)0); //packed guid of target
            }

            if ((flags & ObjectUpdateFlags.UPDATEFLAG_TRANSPORT) != 0)
                pkt.Write(Time.GetMSTime()); //fk knows

            if ((flags & ObjectUpdateFlags.UPDATEFLAG_VEHICLE) != 0)
            {
                pkt.Write((UInt32)0); //vehicle id
                pkt.Write((float)0); //orientation
            }

            if ((flags & ObjectUpdateFlags.UPDATEFLAG_ROTATION) != 0)
            {
                //this is the packed quart :(
                pkt.Write((UInt64)0);
            }
        }

        void _BuildValuesUpdate(ObjectUpdateType type, ObjectUpdateFlags flags, ref PacketOut pkt, IPlayer plr)
        {
            UpdateMask mask = new UpdateMask(State.UpdateFields.Length);
            PacketOut tmp = new PacketOut();


            UpdatefieldFlags visibilityFlags = UpdatefieldFlags.UF_FLAG_PUBLIC;
            if (plr.GetPrimaryKeyLong() == this.GetPrimaryKeyLong())
                visibilityFlags |= UpdatefieldFlags.UF_FLAG_PRIVATE;

            var fieldFlags = GetFlagVisibility();

            /*for (var i = 0; i < State.UpdateFields.Length; ++i)
            {
               // if ((fieldFlags[i] & visibilityFlags) == 0)
                //    continue;
                tmp.Write(State.UpdateFields[i].data);
                mask.SetBit(i);
            }*/

            pkt.Write((byte)mask.MaxBlockCount);
            for (var i = 0; i < mask.MaxBlockCount; ++i)
                pkt.Write(mask.Blocks[i]);

            pkt.Write(tmp);
        }

        UpdatefieldFlags[] GetFlagVisibility()
        {
            TypeID myType = (TypeID)Type;

            switch (myType)
            {
                case TypeID.TYPEID_CORPSE: return UpdateVisibility.CorpseUpdateFieldFlags;
                case TypeID.TYPEID_DYNAMICOBJECT: return UpdateVisibility.DynamicObjectUpdateFieldFlags;
                case TypeID.TYPEID_CONTAINER:
                case TypeID.TYPEID_ITEM: return UpdateVisibility.ItemUpdateFieldFlags;
                case TypeID.TYPEID_UNIT: return UpdateVisibility.UnitUpdateFieldFlags;
                case TypeID.TYPEID_PLAYER: return UpdateVisibility.UnitUpdateFieldFlags;
                default: return null;
            }
        }

        #region Type virtuals
        public virtual bool _IsPlayer() { return false; }
        public virtual bool _IsUnit() { return false; }
        public virtual bool _IsCreature() { return false; }


        public virtual Task<bool> IsPlayer() { return Task.FromResult(_IsPlayer()); }
        public virtual Task<bool> IsUnit() { return Task.FromResult(_IsUnit()); }
        public virtual Task<bool> IsCreature() { return Task.FromResult(_IsCreature()); }
        #endregion
    }

    [Reentrant]
[StorageProvider(ProviderName = "Default")]
public class BaseObject<T> : Grain<T>, IBaseObjectImpl
    where T : class, IGrainState
    {

    }

}
