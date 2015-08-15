using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class PlayerCreateData
    {
        public CMSG_CHAR_CREATE CreateData;
        public int RealmID = 0;
        public string AccountName = "";
    }
}
