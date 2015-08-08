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
        public async Task HandleReadyForAccountDataTimes()
        {
            if (!IsAuthedRealmSession())
                return;

            await Account.SendAccountDataTimes(0x15);
        }

        public async Task HandleUpdateAccountData(UInt32 type, UInt32 time, UInt32 size, byte[] data)
        {
            if (!IsAuthedRealmSession())
                return;
            await Account.UpdateAccountData(type, time, size, data);
        }

        public async Task HandleRequestAccountData(UInt32 type)
        {
            if (!IsAuthedRealmSession())
                return;
            await Account.SendAccountData(type);
        }
    }
}
