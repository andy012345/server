using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public enum AuthOp
    { 
        AUTH_LOGON_CHALLENGE = 0x00,
        AUTH_LOGON_PROOF = 0x01,
        AUTH_RECONNECT_CHALLENGE = 0x02,
        AUTH_RECONNECT_PROOF = 0x03,
        REALM_LIST = 0x10,
        XFER_INITIATE = 0x30,
        XFER_DATA = 0x31,
        XFER_ACCEPT = 0x32,
        XFER_RESUME = 0x33,
        XFER_CANCEL = 0x34,
    }

    public enum AuthError
    {
        Success = 0,
        IpBan = 1,
        AccountClosed = 3,
        NoAccount = 4,
        AccountInUse = 6,
        PreorderTimeLimit = 7,
        ServerFull = 8,
        WrongBuildNumber = 9,
        UpdateClient = 10,
        AccountFreezed = 12,
    }
}
