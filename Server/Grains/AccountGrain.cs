using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using Orleans.Storage;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Server
{
    public interface AccountData : IGrainState
    {
        string Password { get; set; }
        float test_float { get; set; }
        HashSet<UInt32> completed_quests_example_test { get; set; }
        AccountFlags Flags { get; set; }
    }

    public enum AccountFlags
    {
        AccountFlagNone = 0,
        AccountNotValid = 1,
    }

    [Reentrant]
    [StorageProvider(ProviderName = "Default")]
    public class AccountGrain : Grain<AccountData>, IAccountGrain
    {
        BigInteger EncryptedPassword;

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            if (State.Password == null || State.Password.Length == 0)
                State.Flags |= AccountFlags.AccountNotValid;
        }

        void SaveAccount()
        {
            if ((State.Flags & AccountFlags.AccountNotValid) == AccountFlags.AccountNotValid)
                return;
            WriteStateAsync();
        }

        bool IntIsValid()
        {
            if ((State.Flags & AccountFlags.AccountNotValid) == AccountFlags.AccountNotValid)
                return false;
            return true;
        }

        public async Task<bool> IsValid()
        {
            if ((State.Flags & AccountFlags.AccountNotValid) == AccountFlags.AccountNotValid)
                return false;
            return true;
        }

        public Task Destroy() { DeactivateOnIdle(); return TaskDone.Done; }

        public Task<AccountCreateResponse> CreateAccount(string password)
        {
            if ((State.Flags & AccountFlags.AccountNotValid) != AccountFlags.AccountNotValid)
                return Task.FromResult(AccountCreateResponse.AccountCreateDataAlreadyExists);

            State.Flags &= ~AccountFlags.AccountNotValid;
            SetPassword(password);

            return Task.FromResult(AccountCreateResponse.AccountCreateOk);
        }

        public Task SetPassword(string password)
        {
            //encrypt wow style
            string account = this.GetPrimaryKeyString();
            string upper_account = account.ToUpper();
            string upper_password = password.ToUpper();

            string password_string = upper_account + ":" + upper_password;

            State.Password = Shared.SHA.HashString(password_string);

            SaveAccount();

            return TaskDone.Done;
        }

        public Task<string> GetPassword() { return Task.FromResult(State.Password); }

        public Task<AccountAuthResponse> Authenticate(string password)
        {
            if ((State.Flags & AccountFlags.AccountNotValid) != 0)
                return Task.FromResult(AccountAuthResponse.AccountAuthNotValid);

            string account = this.GetPrimaryKeyString();
            string upper_account = account.ToUpper();
            string upper_password = password.ToUpper();

            string password_string = upper_account + ":" + upper_password;

            string password_hash = Shared.SHA.HashString(password_string);

            if (password_hash != State.Password)
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
