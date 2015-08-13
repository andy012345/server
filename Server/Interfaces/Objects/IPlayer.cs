using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;
using System;
using Server;
using Shared;

namespace Server
{
    public interface IPlayerByNameIndex : IGrainWithStringKey
    {
        Task<bool> SetPlayer(IPlayer plr);
        Task<ObjectGUID> GetPlayerGUID();
        Task<IPlayer> GetPlayer();
        Task Save();
    }

    public interface IPlayer : IPlayerImpl { }
   
    public interface IPlayerImpl : IUnitImpl
    {
       // Task<string> VirtualCall();
        Task<string> PlayerCall();
        Task Create(CMSG_CHAR_CREATE creationData);
    }

}
