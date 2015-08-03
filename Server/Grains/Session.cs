using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public interface SessionData : IGrainState
    {
        byte[] SessionKey { get; set; }
        IAccountGrain Account { get; set; } //can we reference interfaces to actors? the codegen seems fine, to test
    }

    [Reentrant]
    [StorageProvider(ProviderName = "Default")]
    class Session : Grain<SessionData>, ISession
    {
        BigInteger N;
        BigInteger g;
        BigInteger s;
        BigInteger v;
        BigInteger b;
        BigInteger B;
        BigInteger rs;

        BigInteger sessionKey;

        IAccountGrain Account = null;

        public override async Task OnActivateAsync()
        {
            N = BigInteger.Parse("894B645E89E1535BBDAD5B8B290650530801B18EBFBF5E8FAB3C82872A3E9BB7", NumberStyles.HexNumber);
            g = new BigInteger(7);
            s = Shared.BigInt.FromRand(32);
        }

        public async Task<Shared.Packet> OnLogonChallenge(string AccountName)
        {
            IAccountGrain act = AccountGrainFactory.GetGrain(AccountName);

            //if (!(await act.IsValid()))
           //     return; //todo: add errors?
            
            SHA1Managed sh = new SHA1Managed();
            MemoryStream strm = new MemoryStream();

            byte[] sbytes = s.ToByteArray();

            await strm.WriteAsync(sbytes, 0, sbytes.Length);

            string password = await act.GetPassword();
            BigInteger pass = BigInteger.Parse(password, NumberStyles.HexNumber);
            byte[] pbytes = pass.ToByteArray();

            await strm.WriteAsync(pbytes, 0, pbytes.Length);

            var xbytes = sh.ComputeHash(strm.GetBuffer());


            BigInteger x = Shared.BigInt.FromBytesUnsigned(xbytes);
            v = BigInteger.ModPow(g, x, N);

            b = Shared.BigInt.FromRand(20);

            BigInteger gmod = BigInteger.ModPow(g, b, N);
            B = ((v * 3) + gmod) % N;

            BigInteger unk = Shared.BigInt.FromRand(16); //I'm sure this is used for matrix proofing (2 factor auth)

            Shared.Packet rtn = new Shared.Packet();
            rtn.w.Write((byte)0); //AUTH_LOGON_CHALLENGE
            rtn.w.Write((byte)0); //success
            rtn.w.Write((byte)0); //unknown
            rtn.w.WriteBigInt(B, 32);
            rtn.w.WriteBigIntLength(g, 1);
            rtn.w.WriteBigIntLength(N, 32);
            rtn.w.WriteBigInt(s, 32);
            rtn.w.WriteBigInt(unk, 16);
            rtn.w.Write((byte)0); //security flag

            return rtn;
        }
    }
}
