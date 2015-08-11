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
        Task OnLogonChallenge(AuthLogonChallenge challenge);
        Task OnLogonProof(AuthLogonProof proof);
        Task OnRealmList();
        Task SetSessionType(SessionType type);
        Task<SessionType> GetSessionType();
        Task Disconnect();
        Task<BigInteger> GetSessionKey();
        Task HandleAuthSession(CMSG_AUTH_SESSION auth, UInt32 ServerSeed);
        Task OnSocketDisconnect();
        Task GetSessionKeyFromAuthAccount(string AccountName);
        Task SetRealmInfo(RealmSettings settings);
        Task SendPacket(PacketOut p);

        Task HandleReadyForAccountDataTimes();
        Task HandleUpdateAccountData(UInt32 type, UInt32 time, UInt32 size, byte[] data);
        Task HandleRequestAccountData(UInt32 type);

        Task HandleCharEnum();
        Task HandleCharCreate(CMSG_CHAR_CREATE create);
    }
}
