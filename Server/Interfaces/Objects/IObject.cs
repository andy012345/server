using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;
using System;
using Server;
using Shared;

namespace Server
{
    public interface IObject : IObjectImpl { }

    public interface IObjectImpl : IBaseObjectImpl
    {
        Task<string> VirtualCall();
        Task<string> ObjectCall();
        Task Save();

        Task<bool> IsValid();

        Task<ObjectGUID> GetGUID();
        Task<PackedGUID> GetPackedGUID();


        //Update FIelds
        Task<byte> GetByte(int field, int index);
        Task<UInt32> GetUInt32(int field);
        Task<float> GetFloat(UInt32 field);
        Task SetByte(int field, int index, byte val);
        Task SetUInt32(int field, UInt32 val);
        Task SetInt32(int field, int val);
        Task SetFloat(int field, float val);
        Task SetUInt64(int field, UInt64 val);
        Task SetGUID(int field, ObjectGUID val);

        //Maps
        Task SetMap(IMap map);
        Task<bool> IsCellActivator();
    }

    public interface IBaseObjectImpl : IGrainWithIntegerKey
    {

    }
}
