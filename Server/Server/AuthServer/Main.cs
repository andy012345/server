using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Server.AuthServer
{
    public class Main
    {
        public static Networking.LogonPacketHandler LogonPacketHandler = new Networking.LogonPacketHandler();

        public static void Run()
        {
            //just some test setup for now
            LogonPacketHandler.Init();

            Networking.ServerSocket sock = new Networking.ServerSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.SetProcessor(new Networking.LogonPacketProcessor(sock));
            sock.Bind(9001);
            sock.Listen(50);
            sock.Accept();
        }
    }
}
