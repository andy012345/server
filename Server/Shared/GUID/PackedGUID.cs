using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class PackedGUID
    {
        public byte mask = 0;
        public byte[] guidbytes = null;

        public PackedGUID() { }
        public PackedGUID(ObjectGUID guid) { Create(guid.ToUInt64()); }
        public PackedGUID(UInt64 guid) { Create(guid); }

        public void Create(UInt64 guid)
        {
            var bytes = BitConverter.GetBytes(guid);

            var packedbytes = new List<byte>();

            for (int i = 0; i < bytes.Length; ++i)
            {
                if (bytes[i] != 0)
                {
                    mask |= (byte)(1 << i);
                    packedbytes.Add(bytes[i]);
                }
            }

            guidbytes = packedbytes.ToArray();
        }
    }    
}
