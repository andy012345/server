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
        PacketProcessed,
        PacketRequiresMoreData,
    }

    public class PacketProcessor
    {
        public MemoryStream packetData = new MemoryStream();
        public int dataNeeded = 0;

        public void ReadHandler(byte[] data, int dataIndex, int dataSize)
        {
            int copyAmount = 0;

            var res = OnReceive(data, dataIndex, dataSize, out copyAmount);

            if (res == PacketProcessResult.PacketProcessed)
                packetData = new MemoryStream();
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
                    return PacketProcessResult.PacketRequiresMoreData;
                }
            }
            return HandleProcess();
        }

        public virtual PacketProcessResult ProcessData() { return PacketProcessResult.PacketProcessed; }
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
