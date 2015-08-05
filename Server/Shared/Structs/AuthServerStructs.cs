using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public struct AuthLogonProof
    {
        public byte[] A;
        public byte[] M1;
        public byte[] crchash;
        public byte number_of_keys;
        public byte unk;
    }

    public struct AuthLogonChallenge
    {
        public string client;
        public byte client_major;
        public byte client_minor;
        public byte client_revision;
        public UInt16 client_build;
        public string processor;
        public string os;
        public string locale;
        public int category; //category of client
        public IPAddress ipaddr;
        public string account;
    }
}
