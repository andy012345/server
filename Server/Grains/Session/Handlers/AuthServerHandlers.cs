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
    public partial class Session
    {
        public async Task OnLogonChallenge(AuthLogonChallenge challenge)
        {
            Account = GrainFactory.GetGrain<IAccountGrain>(challenge.account);

            if (!(await Account.IsValid()))
            {
                await SendAuthError(AuthError.NoAccount);
                return;
            }


            string password = await Account.GetPassword();

            Shared.BigInteger pass = new Shared.BigInteger(password, 16);

            //test

            string passwordPlain = await Account.GetPasswordPlain();
            string SRPHash = challenge.account.ToUpper() + ":" + passwordPlain.ToUpper();
            var SRPHashBytes = Encoding.UTF8.GetBytes(SRPHash);
            var SRPCreds = BigInt.Hash(SRPHashBytes); //The bytes were g

            BigInteger x = BigInt.Hash(s, SRPCreds);

            v = g.ModPow(x, N);

            b = new Shared.BigInteger(new Random(), 160);

            Shared.BigInteger gmod = g.ModPow(b, N);

            if (gmod < 0)
                gmod += N;

            B = ((v * 3) + gmod) % N;

            if (B < 0)
                B += N;

            Shared.BigInteger unk = new Shared.BigInteger(new Random(), 128); //I'm sure this is used for matrix proofing (2 factor auth)

            PacketOut rtn = new PacketOut(AuthOp.AUTH_LOGON_CHALLENGE);
            rtn.Write((byte)AuthError.Success);
            rtn.Write((byte)0); //unknown
            rtn.WriteBigInt(B, 32);
            rtn.WriteBigIntLength(g, 1);
            rtn.WriteBigIntLength(N, 32);
            rtn.WriteBigInt(s, 32);
            rtn.WriteBigInt(unk, 16);
            rtn.Write((byte)0); //security flag
            await SendPacket(rtn); //test
        }

        public async Task OnLogonProof(AuthLogonProof proof)
        {
            BigInteger A = new BigInteger(proof.A);
            BigInteger M1 = new BigInteger(proof.M1);

            if (A == 0 || M1 == 0) //possible hack attempt
            {
                await SendAuthError(AuthError.NoAccount);
                return;
            }

            BigInteger u = BigInt.Hash(A, B);

            BigInteger tmp = v.ModPow(u, N);
            if (tmp < 0)
                tmp += N;
            BigInteger S = A * tmp;
            S = S.ModPow(b, N);
            if (S < 0)
                S += N;

            byte[] t = S.GetBytes();

            //byte[] t = new byte[32];
            byte[] t1 = new byte[16];
            byte[] t2 = new byte[16];
            byte[] vK = new byte[40];

            for (int i = 0; i < 16; ++i)
            {
                t1[i] = t[i * 2];
                t2[i] = t[(i * 2) + 1];
            }

            var t1sha = BigInt.Hash(t1);
            var t2sha = BigInt.Hash(t2);

            var t1shabytes = t1sha.GetBytes(20);
            var t2shabytes = t2sha.GetBytes(20);

            for (int i = 0; i < 20; ++i)
            {
                vK[i * 2] = t1shabytes[i];
                vK[(i * 2) + 1] = t2shabytes[i];
            }

            var t3 = BigInt.Hash(N) ^ BigInt.Hash(g);

            var AccountName = Account.GetPrimaryKeyString().ToUpper();

            var t4bytes = Encoding.UTF8.GetBytes(AccountName);
            var t4 = BigInt.Hash(t4bytes);
            t4bytes = t4.GetBytes();

            SessionKey = new BigInteger(vK);

            var M = BigInt.Hash(t3, t4, s, A, B, SessionKey);

            //do we match the M sent by the client?
            if (M == M1)
            {
                Authed = true;

                // The account code will disconnect any other attached auth or realm sessions when we add an auth session to it
                await Account.AddSession(this);

                BigInteger M2 = BigInt.Hash(A, M, SessionKey);

                PacketOut p = new PacketOut(AuthOp.AUTH_LOGON_PROOF);
                p.Write((byte)AuthError.Success);
                p.WriteBigInt(M2, 20);
                p.Write((int)0);
                p.Write((int)0);
                p.Write((short)0);

                await SendPacket(p);
            }
            else
            {
                await SendAuthError(AuthError.NoAccount);
            }
        }

        public async Task OnRealmList()
        {
            if (!Authed)
                return;

            var realm_manager = GrainFactory.GetGrain<IRealmManager>(0);

            if (realm_manager == null) //something seriously went wrong!
                return;

            var realms = await realm_manager.GetRealms();

            PacketOut p = new PacketOut(AuthOp.REALM_LIST);
            p.Write((UInt16)0); //size

            p.Write((int)0);

            //lets write a test realm!
            p.Write((UInt16)realms.Length); //realmCount


            foreach (var r in realms)
            {
                p.Write((byte)0); //type
                p.Write((byte)0); //status
                p.Write((byte)0); //flags
                p.WriteCString(r.RealmSettings.Name);
                p.WriteCString(r.RealmSettings.Address);
                p.Write(r.GetPopulationStatus());
                p.Write((byte)1); //character count, TODO
                p.Write((byte)r.RealmSettings.Cat);
                p.Write((byte)0); //unknown
            }

            //this should be a loop based on realmcount in future
            //end loop

            p.Write((byte)0x10);
            p.Write((byte)0);

            //rewrite size
            p.strm.Position = 1;
            p.Write((UInt16)(p.strm.Length - 3));
            p.strm.Position = p.strm.Length;

            await SendPacket(p);
        }

        Task SendAuthError(AuthError error)
        {
            PacketOut p = new PacketOut();
            p.w.Write((byte)AuthOp.AUTH_LOGON_CHALLENGE);
            p.w.Write((byte)0);
            p.w.Write((byte)error);
            SendPacket(p);
            return TaskDone.Done;
        }

    }
}
