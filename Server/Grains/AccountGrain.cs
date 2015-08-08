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
using Shared;

namespace Server
{
    public class AccountData
    {
        public byte[][] Data = new byte[8][];
        public UInt32[] TimeStamp = new UInt32[8];
        public UInt32[] Size = new UInt32[8]; //Decompressed size of data
    }

    public interface AccountState : IGrainState
    {
        AccountFlags Flags { get; set; }

        string Password { get; set; }
        string PasswordPlain { get; set; }

        //Account Data, I think this is for the UI config or something. Whatever.
        AccountData Data { get; set; }
    }

    public enum AccountFlags
    {
        AccountFlagNone = 0,
        AccountNotValid = 1,
    }

    [Reentrant]
    [StorageProvider(ProviderName = "Default")]
    public class AccountGrain : Grain<AccountState>, IAccountGrain
    {
        List<ISession> ActiveSessions = new List<ISession>();
        ISession AuthSession;
        ISession RealmSession;

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            if (State.Password == null || State.Password.Length == 0)
                State.Flags |= AccountFlags.AccountNotValid;

            if (State.Data != null)
                State.Data = new AccountData();
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

        public Task<bool> IsValid()
        {
            return Task.FromResult(IntIsValid());
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
            State.PasswordPlain = password;


            SaveAccount();

            return TaskDone.Done;
        }

        public Task<string> GetPassword() { return Task.FromResult(State.Password); }
        public Task<string> GetPasswordPlain() { return Task.FromResult(State.PasswordPlain); }

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

        public async Task AddSession(ISession s)
        {
            var type = await s.GetSessionType();

            switch (type)
            {
                case SessionType.AuthSession:
                    {
                        if (AuthSession != s)
                        {
                            if (AuthSession != null)
                                await RemoveSession(AuthSession, true);
                            if (RealmSession != null)
                                await RemoveSession(RealmSession, true);
                        }
                        AuthSession = s;
                    } break;
                case SessionType.RealmSession:
                    {
                        if (RealmSession != null && s != RealmSession)
                            await RemoveSession(RealmSession, true);
                        RealmSession = s;
                    } break;
            }

            ActiveSessions.Add(s);
        }

        public async Task RemoveSession(ISession s, bool disconnect = false)
        {
            var type = await s.GetSessionType();
            switch (type)
            {
                case SessionType.AuthSession:
                    {
                        AuthSession = null;
                    }
                    break;
                case SessionType.RealmSession:
                    {
                        RealmSession = null;
                    }
                    break;
            }

            ActiveSessions.Remove(s);

            if (disconnect)
                await s.Disconnect();
        }

        public Task<ISession> GetAuthSession() { return Task.FromResult(AuthSession); }
        public Task<ISession> GetRealmSession() { return Task.FromResult(RealmSession); }

        public async Task SendPacketRealm(Packet p)
        {
            if (RealmSession == null)
                return;
            await RealmSession.SendPacket(p);
        }
        public async Task SendPacketAuth(Packet p)
        {
            if (AuthSession == null)
                return;
            await AuthSession.SendPacket(p);
        }

        public Task UpdateAccountData(UInt32 id, UInt32 time, UInt32 size, byte[] data)
        {
            if (id >= 8)
                throw new ArgumentException("id cannot be >= 8");

            State.Data.TimeStamp[id] = time;
            State.Data.Size[id] = size;
            State.Data.Data[id] = data;
            return TaskDone.Done;
        }

        public async Task SendAccountData(UInt32 id)
        {
            if (id >= 8)
                throw new ArgumentException("SendAccountData attempted to use out of range index");

            Packet p = new Packet(RealmOp.SMSG_UPDATE_ACCOUNT_DATA);
            var guid = (UInt64)0; //TODO: replace with active character if they are there!

            p.Write(guid);
            p.Write(id);
            p.Write(State.Data.TimeStamp[id]);
            p.Write(State.Data.Size[id]);
            p.Write(State.Data.Data[id]);

            await SendPacketRealm(p);
        }

        public async Task SendAccountDataTimes(UInt32 mask)
        {
            Packet p = new Packet(RealmOp.SMSG_ACCOUNT_DATA_TIMES);
            p.Write(Time.GetUnixTime());
            p.Write((byte)1);
            p.Write(mask);

            for (int i = 0; i < 8; ++i)
            {
                if ((mask & (1 << i)) != 0)
                {
                    p.Write(State.Data.TimeStamp[i]);
                }
            }

            await SendPacketRealm(p);
        }

        public async Task SendCharEnum()
        {
            Packet p = new Packet(RealmOp.SMSG_CHAR_ENUM);

            p.Write((byte)0); //todo :)

            await SendPacketRealm(p);
        }
    }
}
