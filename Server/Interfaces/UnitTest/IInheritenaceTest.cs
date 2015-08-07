using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;
using System;
using Server;

namespace Server
{
    public interface IPlayerTestImpl : IPlayerTest { }
    public interface IUnitTestImpl : IUnitTest { }
    public interface IObjectTestImpl : IObjectTest { }

    public interface IPlayerTest : IUnitTest
    {
       // Task<string> VirtualCall();
        Task<string> PlayerCall();
    }

    public interface IUnitTest : IObjectTest
    {
        //  Task<string> VirtualCall();
        Task<string> UnitCall();
    }

    public interface IObjectTest : IGrainWithIntegerKey
    {
        Task<string> VirtualCall();
        Task<string> ObjectCall();
    }
}
