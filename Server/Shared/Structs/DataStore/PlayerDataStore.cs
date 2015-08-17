using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class PlayerCreateInfo
    {
        public UInt32 race = 0;
        public UInt32 @class = 0;
        public UInt32 map = 0;
        public UInt32 zone = 0;
        public float position_x = 0;
        public float position_y = 0;
        public float position_z = 0;
        public float orientation = 0;
    }

    public class PlayerCreateInfoAction
    {
        public UInt32 race = 0;
        public UInt32 @class = 0;
        public UInt32 button = 0;
        public UInt32 action = 0;
        public UInt32 type = 0;
    }

    public class PlayerCreateInfoItem
    {
        public UInt32 race = 0;
        public UInt32 @class = 0;
        public UInt32 itemid = 0;
        public Int32 amount = -1;
    }

    public class PlayerCreateInfoSkills
    {
        public UInt32 raceMask = 0;
        public UInt32 classMask = 0;
        public UInt32 skill = 0;
        public UInt32 rank = 0;
        public string comment = "";
    }

    public class PlayerCreateInfoSpellCustom
    {
        public UInt32 raceMask = 0;
        public UInt32 classMask = 0;
        public UInt32 Spell = 0;
        public string Note = "";

    }

    public class PlayerClassLevelStats
    {
        public UInt32 @class = 0;
        public UInt32 level = 0;
        public UInt32 basehp = 0;
        public UInt32 basemana = 0;
    }

    public class PlayerXPForLevel
    {
        public UInt32 Levle = 0;
        public UInt32 Experience = 0;
    }

    public class PlayerLevelStats
    {
        public UInt32 race = 0;
        public UInt32 @class = 0;
        public UInt32 level = 0;
        public UInt32 str = 0;
        public UInt32 agi = 0;
        public UInt32 sta = 0;
        public UInt32 inte = 0;
        public UInt32 spi = 0;

    }
}
