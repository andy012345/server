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
    public interface ObjectData : IGrainState
    {
        bool Exists { get; set; }
        ObjectType ObjType { get; set; }
        UpdateField[] UpdateFields { get; set; }
    }

    public class ObjectImpl : Object<ObjectData>, IObject
    {
    }
   
    public class Object<T> : BaseObject<T>, IObjectImpl
        where T : class, ObjectData
    {

        public virtual Task<string> VirtualCall() { return Task.FromResult("Virtual call from object"); }
        public Task<string> ObjectCall() { return Task.FromResult("Call from object"); }

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
    }

    [Reentrant]
[StorageProvider(ProviderName = "Default")]
public class BaseObject<T> : Grain<T>, IBaseObjectImpl
    where T : class, IGrainState
    {

    }

}
