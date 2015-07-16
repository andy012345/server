using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using Orleans.Storage;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Server
{
    public interface AccountData : IGrainState
    {
        string password { get; set; }
        float test_float { get; set; }
        HashSet<UInt32> completed_quests_example_test { get; set; }
        AccountFlags flags { get; set; }
    }

    public enum AccountFlags
    {
        AccountFlagNone = 0,
        AccountNotValid = 1,
    }

    [Reentrant]
    [StorageProvider(ProviderName = "AccountStore")]
    public class AccountGrain : Grain<AccountData>, IAccountGrain
    {
        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            if (State.password.Length == 0)
                State.flags |= AccountFlags.AccountNotValid;
        }

        public Task Destroy() { DeactivateOnIdle(); return TaskDone.Done; }

        public Task<AccountCreateResponse> CreateAccount(string password, float test_float)
        {
            return Task.FromResult(AccountCreateResponse.AccountCreateOk);
        }

        public Task<AccountAuthResponse> Authenticate(string password)
        {
            if ((State.flags & AccountFlags.AccountNotValid) != 0)
                return Task.FromResult(AccountAuthResponse.AccountAuthNotValid);

            if (password != State.password)
                return Task.FromResult(AccountAuthResponse.AccountAuthNoMatch);

            return Task.FromResult(AccountAuthResponse.AccountAuthOk);
        }

        public Task AddQuestComplete(UInt32 questid)
        {
            State.completed_quests_example_test.Add(questid);
            WriteStateAsync();
            return TaskDone.Done;
        }

        public Task<bool> QuestCompleted(UInt32 questid)
        {
            return Task.FromResult(State.completed_quests_example_test.Contains(questid));
        }

    }
}
