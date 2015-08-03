using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;
using System;
using Server;

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
        Task AddQuestComplete(UInt32 questid);

        Task<bool> QuestCompleted(UInt32 questid);
        Task SetPassword(string password);
        Task<String> GetPassword();
        Task<bool> IsValid();
    }
}
