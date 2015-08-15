using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class SessionStream
    {
        public IAsyncStream<byte[]> PacketStream = null;
        public IAsyncStream<SocketCommand> CommandStream = null;
    }

}
