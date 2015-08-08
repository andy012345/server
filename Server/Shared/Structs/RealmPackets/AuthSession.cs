using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public struct CMSG_AUTH_SESSION
    {
        public UInt32 Build;
        public UInt32 Seed;
        public BigInteger Digest;
        public string Account;
        public byte[] AddonData;
    }

    public class AuthSessionResponse
    {
        public bool Errored = true;
        public BigInteger SessionKey;

        public AuthSessionResponse() { }
        public AuthSessionResponse(bool error) { Errored = error; }
        public AuthSessionResponse(bool error, BigInteger key) { Errored = error; SessionKey = key; }
    }
}
