using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;
using Server;
using Shared;

namespace Server
{
    public interface ISession : IGrainWithGuidKey
    {
        Task OnLogonChallenge(string AccountName);
        Task OnLogonProof(AuthLogonProof proof);
        Task OnRealmList();
        Task SetSessionType(SessionType type);
        Task<SessionType> GetSessionType();
        Task Disconnect();
    }
}
