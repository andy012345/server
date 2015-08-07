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

namespace Server.Networking
{
    public class SocketPacketObserver : IAsyncObserver<byte[]>
    {
        ServerSocket sock;
        public SocketPacketObserver(ServerSocket s) { sock = s; }

        public Task OnCompletedAsync()
        {
            throw new NotImplementedException();
        }

        public Task OnErrorAsync(Exception ex)
        {
            throw new NotImplementedException();
        }

        public Task OnNextAsync(byte[] item, StreamSequenceToken token)
        {
            sock.Send(item);
            return TaskDone.Done;
        }
    }
    public class SocketCommandObserver : IAsyncObserver<SocketCommand>
    {
        ServerSocket sock;
        public SocketCommandObserver(ServerSocket s) { sock = s; }

        public Task OnCompletedAsync()
        {
            throw new NotImplementedException();
        }

        public Task OnErrorAsync(Exception ex)
        {
            throw new NotImplementedException();
        }

        public Task OnNextAsync(SocketCommand item, StreamSequenceToken token)
        {
            if (item == SocketCommand.DisconnectClient)
                sock.Dispose();
            return TaskDone.Done;
        }
    }

    public class ServerSocket : IDisposable
    {
        Socket sock = null;
        SocketPermission permissions = null;
        bool disposed = false;

        PacketProcessor processor = null;
        public ISession session = null;
        SocketPacketObserver packetObserver = null;
        StreamSubscriptionHandle<byte[]> packetObserverHandle = null;
        
        SocketCommandObserver commandObserver = null;
        StreamSubscriptionHandle<SocketCommand> commandObserverHandle = null;

        public ServerSocket() { }
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

        public ServerSocket(Socket s)
        {
            sock = s;
        }
        public void CreateSession()
        {
            session = Orleans.GrainClient.GrainFactory.GetGrain<ISession>(Guid.NewGuid()); //create a unique session for this socket

            if (session == null)
                throw new Exception("Socket failed to create orleans session");

            var provider = Orleans.GrainClient.GetStreamProvider("PacketStream");

            if (provider == null)
                throw new Exception("Socket: failed to get PacketStream provider");

            var packetstream = provider.GetStream<byte[]>(session.GetPrimaryKey(), "SessionPacketStream");
            var commandstream = provider.GetStream<SocketCommand>(session.GetPrimaryKey(), "SessionCommandtStream");

            if (packetstream == null)
                throw new Exception("Socket: failed to get packetstream");
            if (commandstream == null)
                throw new Exception("Socket: failed to get commandstream");

            packetObserver = new SocketPacketObserver(this);
            packetObserverHandle = packetstream.SubscribeAsync(packetObserver).Result;
            commandObserver = new SocketCommandObserver(this);
            commandObserverHandle = commandstream.SubscribeAsync(commandObserver).Result;
        }


        public void Send(byte[] buffer) { Send(buffer, buffer.Length); }
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
            if (e.SocketError != SocketError.Success)
                Console.WriteLine("Socket Error {0}", e.SocketError.ToString());
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
            processor = null;
            session = null;

            if (packetObserverHandle != null)
                packetObserverHandle.UnsubscribeAsync().Wait();
            if (commandObserverHandle != null)
                commandObserverHandle.UnsubscribeAsync().Wait();
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
                sck.SetProcessor((PacketProcessor)Activator.CreateInstance(processor.GetType()));
                sck.processor.sock = sck;
                sck.CreateSession();
                sck.Read();
            }

            Accept();
        }

        public void SetupPermissions()
        {
            permissions = new SocketPermission(NetworkAccess.Accept,
                   TransportType.Tcp, "", SocketPermission.AllPorts);
        }

        public void SetProcessor(PacketProcessor p) { processor = p; p.SetSocket(this); }
    }
}
