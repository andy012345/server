using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Networking
{
    class ServerSocket : IDisposable
    {
        Socket sock = null;
        SocketPermission permissions = null;
        bool disposed = false;

        PacketProcessor processor = null;

        public ServerSocket(AddressFamily addressFamily, SocketType sockType, ProtocolType protoType)
        {
            /*

            These should always be for the server:
            addressFamily = InterNetwork
            sockType = Stream
            protoType = Tcp

            */
            sock = new Socket(addressFamily, sockType, protoType);
        }

        public ServerSocket(Socket s) { sock = s; }

        public void Send(byte[] buffer, int bufferSize)
        {
            SocketAsyncEventArgs ev = new SocketAsyncEventArgs();
            ev.SetBuffer(buffer, 0, bufferSize);
            ev.Completed += AsyncSendEvent;

            if (!sock.SendAsync(ev))
                OnSend(ev);
        }

        void AsyncSendEvent(object sender, SocketAsyncEventArgs e)
        {
            OnSend(e);
        }
        
        void OnSend(SocketAsyncEventArgs e)
        {

        }

        public void Read(int bufferSize = 8192, byte[] reusebuffer = null)
        {    
            SocketAsyncEventArgs ev = new SocketAsyncEventArgs();
            byte[] buf = reusebuffer;
            if (buf == null)
                buf = new byte[bufferSize];
            ev.SetBuffer(buf, 0, bufferSize);
            ev.Completed += AsyncReadEvent;

            if (!sock.ReceiveAsync(ev))
                OnReceive(ev);
        }

        void AsyncReadEvent(object sender, SocketAsyncEventArgs e)
        {
            OnReceive(e);
        }

        void OnReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred == 0 || e.SocketError != SocketError.Success) //disconnected
            {
                Dispose();
                return;
            }

            if (processor != null)
                processor.ReadHandler(e.Buffer, 0, e.BytesTransferred);

            Read(e.Buffer.Length, e.Buffer); //reuse buffers
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
            sock.Dispose();
            sock = null;
        }

        public void Bind(UInt16 port)
        {
            SetupPermissions();
            sock.Bind(new IPEndPoint(IPAddress.Any, port));
        }

        public void Listen(int backlog)
        {
            sock.Listen(backlog);
        }

        public void Accept()
        {
            SocketAsyncEventArgs ev = new SocketAsyncEventArgs();
            ev.Completed += AcceptAsyncEvent;

            if (!sock.AcceptAsync(ev))
                OnAccept(ev);
        }

        void AcceptAsyncEvent(object sender, SocketAsyncEventArgs e)
        {
            OnAccept(e);
        }

        void OnAccept(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
                return;

            var newsocket = e.AcceptSocket;

            if (newsocket != null)
            {
                ServerSocket sck = new ServerSocket(newsocket);
                //inherit my packet processor
                sck.SetProcessor(processor);
                sck.Read();
            }

            Accept();
        }

        public void SetupPermissions()
        {
            permissions = new SocketPermission(NetworkAccess.Accept,
                   TransportType.Tcp, "", SocketPermission.AllPorts);
        }

        public void SetProcessor(PacketProcessor p) { processor = p; }
    }
}
