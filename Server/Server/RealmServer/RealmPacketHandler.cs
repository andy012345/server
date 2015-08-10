using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Net;
using Shared;
using Server.Networking;
using System.Security.Cryptography;

namespace Server.RealmServer
{
    class RealmPacketProcessor : PacketProcessor
    {
        public RealmPacketProcessor() : base() { dataNeeded = DefaultDataNeeded(); } //opcode

        RealmOp opcode;
        int DecryptPointer = 0;
        public UInt32 Seed = 0;
        public int RealmID = 0;

        public override void OnConnect(ServerSocket parent = null)
        {
            if (sock.session == null)
                throw new Exception("Realm connection with no session");
            var realmparent = parent as RealmClientSocket;
            if (realmparent == null)
                throw new Exception("Realm connection with non realm parent!");

            sock.session.SetRealmInfo(realmparent.GetRealmSettings());

            Random rnd = new Random();
            Seed = (UInt32)rnd.Next();

            Packet p = new Packet(RealmOp.SMSG_AUTH_CHALLENGE);
            p.Write((int)1);
            p.Write(Seed);

            p.Write(0xF3539DA3);
            p.Write(0x6E8547B9);
            p.Write(0x9A6AA2F8);
            p.Write(0xA4F170F4);
            p.Write(0xF3539DA3);
            p.Write(0x6E8547B9);
            p.Write(0x9A6AA2F8);
            p.Write(0xA4F170F4);

            SendPacket(p);

        }

        public void SetupCrypto(BigInteger key)
        {
            byte[] ServerDecryptionKey =
            {
                0xC2, 0xB3, 0x72, 0x3C, 0xC6, 0xAE, 0xD9, 0xB5,
                0x34, 0x3C, 0x53, 0xEE, 0x2F, 0x43, 0x67, 0xCE
            };

            byte[] ServerEncryptionKey =
            {
                0xCC, 0x98, 0xAE, 0x04, 0xE8, 0x97, 0xEA, 0xCA,
                0x12, 0xDD, 0xC0, 0x93, 0x42, 0x91, 0x53, 0x57
            };

            HMACSHA1 decryptHMAC = new HMACSHA1(ServerDecryptionKey);
            HMACSHA1 encryptHMAC = new HMACSHA1(ServerEncryptionKey);

            var decryptHash = decryptHMAC.ComputeHash(key.GetBytes());
            var encryptHash = encryptHMAC.ComputeHash(key.GetBytes());

            int dropN = 1024; //1000 before WoTLK, 1024 now
            var buf = new byte[dropN];

            sock.Decrypt = new ARC4(decryptHash);
            sock.Encrypt = new ARC4(encryptHash);

            sock.Decrypt.Process(buf, 0, buf.Length);
            sock.Encrypt.Process(buf, 0, buf.Length);
        }

        public void SendPacket(Packet p)
        {
            p.Finalise();
            var parray = p.strm.ToArray();
            sock.Send(parray, parray.Length);
        }

        public override int DefaultDataNeeded() { return 6; } //shortest is 2 byte size, 4 byte command

        public override PacketProcessResult ProcessData()
        {
            DecryptData(6);
            int sz = packetReader.ReadUInt16BE();

            if ((sz & 0x8000) != 0) //large packet
            {
                dataNeeded = 3; //we need 3 byte size to continue
                if (packetData.Length < dataNeeded) return PacketProcessResult.RequiresData;

                DecryptData(7);
                packetData.Position = 3;
                sz = 0;
                sz |= packetData.GetBuffer()[0] & 0x7F;
                sz <<= 8;
                sz |= packetData.GetBuffer()[1];
                sz <<= 8;
                sz |= packetData.GetBuffer()[2];

                dataNeeded = 3 + sz;
                if (packetData.Length < dataNeeded) return PacketProcessResult.RequiresData;
            }
            else
            {
                dataNeeded = 2 + sz;
                if (packetData.Length < dataNeeded) return PacketProcessResult.RequiresData;
            }

            opcode = (RealmOp)packetReader.ReadUInt32();

            //ok now we need sz + 2 to continue

            return ProcessPacket();
        }

        void DecryptData(int to)
        {
            if (sock.Decrypt == null || DecryptPointer >= to)
                return;

            sock.Decrypt.Process(packetData.GetBuffer(), DecryptPointer, (to - DecryptPointer));
        }

        PacketProcessResult ProcessPacket()
        {
            var handler = RealmServer.Main.PacketHandler.GetHandler(opcode);

            if (handler == null)
            {
                Console.WriteLine("Recieved packet {0} which has no handler", opcode);
                return PacketProcessResult.Processed; //In realm we have known sizes so we mark as processed as multiple packets can come through in one burst, we want to continue reading
            }

            return handler(this);
        }
    }

    public partial class RealmPacketHandler
    {
        Dictionary<UInt32, Func<PacketProcessor, PacketProcessResult>> PacketHandlers = new Dictionary<UInt32, Func<PacketProcessor, PacketProcessResult>>();

        public void Init()
        {
            var type = this.GetType();

            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);

            foreach (var method in methods)
            {
                var attrib = method.GetCustomAttribute<PacketHandlerAttribute>();

                if (attrib == null)
                    continue;

                var handlerDelegate = (Func<PacketProcessor, PacketProcessResult>)Delegate.CreateDelegate(typeof(Func<PacketProcessor, PacketProcessResult>), method);

                if (handlerDelegate == null)
                    continue;

                PacketHandlers.Add(attrib.Id, handlerDelegate);
            }
        }

        public Func<PacketProcessor, PacketProcessResult> GetHandler(RealmOp opcode)
        {
            UInt32 op = (UInt32)opcode;

            Func<PacketProcessor, PacketProcessResult> retval = null;
            PacketHandlers.TryGetValue(op, out retval);
            return retval;
        }
    }

}
