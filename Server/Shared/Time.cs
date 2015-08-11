using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Time
    {
        private const long TICKS_SINCE_1970 = 621355968000000000; // .NET ticks for 1970

        public static uint GetUnixTime(DateTime time)
        {
            return (uint)((time.Ticks - TICKS_SINCE_1970) / 10000000L);
        }

        public static uint GetUnixTime() { return GetUnixTime(DateTime.UtcNow); }
    }

}