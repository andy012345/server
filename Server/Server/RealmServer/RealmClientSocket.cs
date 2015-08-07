using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Orleans.Streams;
using Shared;


namespace Server.RealmServer
{
    public class RealmClientSocket : Server.Networking.ServerSocket
    {
        RealmClientSocket() { }
        public RealmClientSocket(AddressFamily addressFamily, SocketType sockType, ProtocolType protoType) : base(addressFamily, sockType, protoType) { }
        public RealmClientSocket(Socket s) : base(s) { }

    }
}
