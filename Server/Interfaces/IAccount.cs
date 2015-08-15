using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;
using System;
using Server;
using Shared;

namespace Server
{
    public enum AccountAuthResponse
    {
        AccountAuthOk = 0,
        AccountAuthNotValid = 1,
        AccountAuthNoMatch = 2,
    }

    public enum AccountCreateResponse
    {
        AccountCreateOk,
        AccountCreateDataAlreadyExists,
    }

    public interface IAccount : IGrainWithStringKey
    {
        Task Destroy();
        Task<AccountAuthResponse> Authenticate(string password);
        Task<AccountCreateResponse> CreateAccount(string password);
        Task SetPassword(string password);
        Task<String> GetPassword();
        Task<String> GetPasswordPlain();
        Task<bool> IsValid();

        Task AddSession(ISession s);
        Task RemoveSession(ISession s, bool disconnect = false);
        Task<ISession> GetAuthSession();
        Task<ISession> GetRealmSession();

        Task SendPacketRealm(PacketOut p);
        Task SendPacketAuth(PacketOut p);

        Task SendAccountDataTimes(UInt32 mask);
        Task UpdateAccountData(UInt32 id, UInt32 time, UInt32 size, byte[] data);
        Task SendAccountData(UInt32 id);

        Task SendCharEnum(int RealmID = 0);
        Task CreatePlayer(CMSG_CHAR_CREATE create);
        Task PlayerLogin(CMSG_PLAYER_LOGIN pkt);
    }
}
