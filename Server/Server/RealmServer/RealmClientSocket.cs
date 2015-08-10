using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Orleans;
using Orleans.Streams;
using Shared;


namespace Server.RealmServer
{
    public class RealmClientSocket : Server.Networking.ServerSocket
    {
        Task PingTask = null;
        CancellationTokenSource PingTaskCancelSource = null;
        RealmSettings settings = null;

        RealmClientSocket() { }
        public RealmClientSocket(AddressFamily addressFamily, SocketType sockType, ProtocolType protoType) : base(addressFamily, sockType, protoType) { }
        public RealmClientSocket(Socket s) : base(s) { }

        public void SetRealmSettings(RealmSettings s) { settings = s; }
        public RealmSettings GetRealmSettings() { return settings; }
        public void PingRunner()
        {
            PingTaskCancelSource = new CancellationTokenSource();
            var cancelToken = PingTaskCancelSource.Token;
            PingTask = Task.Factory.StartNew(async delegate
            {
                var realm_manager = Orleans.GrainClient.GrainFactory.GetGrain<IRealmManager>(0);
                
                while (cancelToken.IsCancellationRequested == false)
                {
                    await realm_manager.PingRealm(settings.ID);
                    await Task.Delay(TimeSpan.FromSeconds(30));
                }
            },  cancelToken);
        }

        public void PingRunnerCancel()
        {
            if (PingTask != null)
            {
                PingTaskCancelSource.Cancel();
                PingTask.Wait();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                PingRunnerCancel();
                
                if (PingTaskCancelSource != null)
                    PingTaskCancelSource.Dispose();

                if (PingTask != null)
                    PingTask.Dispose();
            }
        }
    }
}
