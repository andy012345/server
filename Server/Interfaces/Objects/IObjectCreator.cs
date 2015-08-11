using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;
using System;
using Server;
using Shared;

namespace Server
{
    public interface IObjectCreator : IGrainWithIntegerKey
    {
        Task<ObjectGUID> GenerateGUID(ObjectType objectType);
    }
}
