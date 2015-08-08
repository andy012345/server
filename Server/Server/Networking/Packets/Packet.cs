using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Networking
{
    public enum PacketProcessResult
    {
        Processed,
        RequiresData,
        Error,
    }

    public class PacketProcessor
    {
        public MemoryStream packetData;
        public Reader.PacketReader packetReader;
        public int dataNeeded = 0;
        public ServerSocket sock = null;

        public PacketProcessor() { Reset(); }

        void Reset() { packetData = new MemoryStream(); packetReader = new Reader.PacketReader(packetData); dataNeeded = DefaultDataNeeded(); }

        public virtual int DefaultDataNeeded() { return 0; }

        public void SetSocket(ServerSocket s) { sock = s; }

        public void ReadHandler(byte[] data, int dataIndex, int dataSize)
        {
            int copyAmount = 0;

            var res = OnReceive(data, dataIndex, dataSize, out copyAmount);

            if (res == PacketProcessResult.Error) //Unknown data on a stream that doesn't have a reported size
            {
                sock.Dispose();
                return;
            }

            if (res == PacketProcessResult.Processed)
            {
                packetData = new MemoryStream();
                packetReader = new Reader.PacketReader(packetData);
                dataNeeded = DefaultDataNeeded();
            }
            dataIndex += copyAmount;

            if (dataIndex < dataSize)
            {
                //we have more data to process
                ReadHandler(data, dataIndex, dataSize);
            }
        }

        PacketProcessResult OnReceive(byte[] data, int dataIndex, int dataSize, out int copyAmount)
        {
            copyAmount = 0;
            int dataLeft = dataNeeded - (int)packetData.Length;

            if (dataLeft >= 1)
            {
                if (dataLeft <= (dataSize - dataIndex)) //we have received all we need to continue processing, WOO!
                {
                    packetData.Write(data, dataIndex, dataLeft);
                    copyAmount = dataLeft;
                    //pass to our handler
                    return HandleProcess();
                }
                else
                {
                    //copy what we can :(
                    packetData.Write(data, dataIndex, (dataSize - dataIndex));
                    copyAmount = (dataSize - dataIndex);
                    return PacketProcessResult.RequiresData;
                }
            }
            return HandleProcess();
        }

        public virtual PacketProcessResult ProcessData() { return PacketProcessResult.Processed; }
        public virtual void OnConnect() { }

        PacketProcessResult HandleProcess()
        {
            var oldPosition = packetData.Position;
            packetData.Position = 0;

            var retval = ProcessData();

            packetData.Position = oldPosition;

            return retval;
        }
    }
}
