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

    public interface IAccountGrain : IGrainWithStringKey
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


        Task SendPacketRealm(Packet p);
        Task SendPacketAuth(Packet p);

        Task SendAccountDataTimes(UInt32 mask);
        Task UpdateAccountData(UInt32 id, UInt32 time, UInt32 size, byte[] data);
        Task SendAccountData(UInt32 id);

        Task SendCharEnum();
    }
}
