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
        public async Task HandleCharEnum()
        {
            if (!IsAuthedRealmSession())
                return;

            await Account.SendCharEnum(RealmID);
        }

        public async Task HandleCharCreate(CMSG_CHAR_CREATE create)
        {
            if (!IsAuthedRealmSession())
                return;
            await Account.CreatePlayer(create);
        }

        public async Task HandlePlayerLogin(CMSG_PLAYER_LOGIN pkt)
        {
            if (!IsAuthedRealmSession())
                return;
            await Account.PlayerLogin(pkt);
        }
    }
}
