using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public interface SessionData : IGrainState
    {
        byte[] SessionKey { get; set; }
        IAccountGrain Account { get; set; } //can we reference interfaces to actors? the codegen seems fine, to test
    }

    [Reentrant]
    [StorageProvider(ProviderName = "Default")]
    class Session : Grain<SessionData>, ISession
    {
    }
}
