using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Orleans.Streams;
using Shared;
using System.Collections;

namespace Server
{
    public interface SessionData : IGrainState
    {
        byte[] SessionKey { get; set; }
        IAccountGrain Account { get; set; } //can we reference interfaces to actors? the codegen seems fine, to test
    }

    [Reentrant]
    [StorageProvider(ProviderName = "Default")]
    public partial class Session : Grain<SessionData>, ISession
    {
        Shared.BigInteger N;
        Shared.BigInteger g;
        Shared.BigInteger s;
        Shared.BigInteger v;
        Shared.BigInteger b;
        Shared.BigInteger B;
        Shared.BigInteger rs;

        Shared.BigInteger SessionKey;
        bool Authed = false;
        
        IAccountGrain Account = null;
        IAsyncStream<byte[]> packetStream = null;
        IAsyncStream<SocketCommand> commandStream = null;

        SessionType SessionType = SessionType.Unknown;
        int RealmID = 0;

        public object Assert { get; private set; }

        public override Task OnActivateAsync()
        {
            N = new Shared.BigInteger("B79B3E2A87823CAB8F5EBFBF8EB10108535006298B5BADBD5B53E1895E644B89", 16);
            g = new Shared.BigInteger(7);
            s = new Shared.BigInteger(new Random(), 256);

            var streamProvider = base.GetStreamProvider("PacketStream");

            if (streamProvider == null)
                throw new Exception("Session failed to initialise stream: GetStreamProvider failed");

            packetStream = streamProvider.GetStream<byte[]>(this.GetPrimaryKey(), "SessionPacketStream");
            commandStream = streamProvider.GetStream<SocketCommand>(this.GetPrimaryKey(), "SessionCommandStream");

            if (packetStream == null)
                throw new Exception("Session failed to initialise stream: GetStream failed");
            if (commandStream == null)
                throw new Exception("Session failed to initialise stream: GetStream failed");

            DelayDeactivation(TimeSpan.FromDays(1)); //don't automatically gobble me up please!

            return TaskDone.Done;
        }

        public Task<BigInteger> GetSessionKey() { return Task.FromResult(SessionKey); }

        bool IsAuthedRealmSession() { if (Authed && SessionType == SessionType.RealmSession) return true; return false; }
        
        public Task OnSocketDisconnect()
        {
            //DeactivateOnIdle();
            DelayDeactivation(TimeSpan.FromSeconds(1)); //let gc have this back
            return TaskDone.Done;
        }

        public async Task SendPacketAsync(Packet p)
        {
            p.Finalise();
            await packetStream.OnNextAsync(p.strm.ToArray());
        }
        public Task SendPacket(Packet p)
        {
            p.Finalise();
            packetStream.OnNextAsync(p.strm.ToArray());
            return TaskDone.Done;
        }

        public Task SetSessionType(SessionType type) { SessionType = type; return TaskDone.Done; }
        public Task<SessionType> GetSessionType() { return Task.FromResult(SessionType); }

        public Task Disconnect()
        {
            //and lets disconnect anyone on the other end!
            if (commandStream != null)
                commandStream.OnNextAsync(SocketCommand.DisconnectClient);

            DelayDeactivation(TimeSpan.FromSeconds(1)); //let gc have this back
            //DeactivateOnIdle();
            return TaskDone.Done;
        }

        public async Task HandleAuthSession(CMSG_AUTH_SESSION auth, UInt32 ServerSeed)
        {
            if (SessionKey == 0)
                await GetSessionKeyFromAuthAccount(auth.Account);

            if (SessionKey == 0)
            {
                await SendAuthResponse(LoginErrorCode.AUTH_UNKNOWN_ACCOUNT);
                return;
            }

            byte[] t = new byte[4];
            var accountbytes = Encoding.UTF8.GetBytes(Account.GetPrimaryKeyString());
            var clientseed = BitConverter.GetBytes(auth.Seed);
            var serverseed = BitConverter.GetBytes(ServerSeed);
            var sess_key = SessionKey;
            var sesskeybytes = sess_key.GetBytes();

            BigInteger ServerProof = BigInt.Hash(accountbytes, t, clientseed, serverseed, sesskeybytes);
            BigInteger ClientProof = new BigInteger(auth.Digest);

            if (ClientProof != ServerProof)
            {
                await SendAuthResponse(LoginErrorCode.AUTH_UNKNOWN_ACCOUNT);
                return;
            }

            Authed = true;
            await Account.AddSession(this); //set realm session (and disconnect any others)

            //TODO: add queues
            Packet p = new Packet(RealmOp.SMSG_AUTH_RESPONSE);

            p.Write((byte)LoginErrorCode.AUTH_OK);
            p.Write((int)0);
            p.Write((byte)0);
            p.Write(0);
            p.Write((byte)0); //expansionLevel

            await SendPacket(p);
        }

        public Task SendAuthResponse(LoginErrorCode code)
        {
            Packet p = new Packet(RealmOp.SMSG_AUTH_RESPONSE);
            p.Write((byte)code);
            SendPacket(p);
            return TaskDone.Done;
        }

        public async Task GetSessionKeyFromAuthAccount(string AccountName)
        {
            Account = GrainFactory.GetGrain<IAccountGrain>(AccountName.ToUpper());

            if (Account == null)
                return;

            //get the auth session
            ISession auth_session = await Account.GetAuthSession();

            if (auth_session == null)
                return;

            SessionKey = await auth_session.GetSessionKey();
        }

        public Task SetRealmInfo(RealmSettings settings)
        {
            SessionType = SessionType.RealmSession;
            RealmID = settings.RealmID;
            return TaskDone.Done;
        }
    }
}
