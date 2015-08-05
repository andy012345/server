using System;
using System.Collections.Generic;
using System.Linq;
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
}
