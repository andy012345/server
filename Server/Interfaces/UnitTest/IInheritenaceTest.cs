using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;
using System;
using Server;

namespace Server
{
    public interface IPlayer : IPlayerImpl { }
    public interface IUnit : IUnitImpl { }
    public interface IObject : IObjectImpl { }

    public interface IPlayerImpl : IUnitImpl
    {
       // Task<string> VirtualCall();
        Task<string> PlayerCall();
    }

    public interface IUnitImpl : IObjectImpl
    {
        //  Task<string> VirtualCall();
        Task<string> UnitCall();
    }

    public interface IObjectImpl : IGrainWithIntegerKey
    {
        Task<string> VirtualCall();
        Task<string> ObjectCall();
    }
}
