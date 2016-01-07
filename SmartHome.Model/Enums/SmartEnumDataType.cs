using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Enums
{
    [DataContract]
    public enum LoadType
    {
        [EnumMember]
        NO_LOAD = 58,
        [EnumMember]
        NON_DIMMABLE_BULB = 58,
        [EnumMember]
        DIMMABLE_BULB = 2,
        [EnumMember]
        FAN = 3
    }
    [DataContract]
    public enum DeviceType
    {
        [EnumMember]
        SMART_SWITCH_6G = 58,
        [EnumMember]
        SMART_RAINBOW_12 = 58,
        [EnumMember]
        CURTAIN = 2,
        [EnumMember]
        CAMERA = 3
    }
}
