using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;
using Server;

namespace Server
{
    public interface ISession : IGrainWithGuidKey
    {
        Task<Shared.Packet> OnLogonChallenge(string AccountName);
    }
}
