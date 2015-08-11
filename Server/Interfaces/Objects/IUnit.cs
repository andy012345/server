using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;
using System;
using Server;

namespace Server
{
    public interface IUnit : IUnitImpl { }
  
    public interface IUnitImpl : IObjectImpl
    {
        //  Task<string> VirtualCall();
        Task<string> UnitCall();
    }

}
