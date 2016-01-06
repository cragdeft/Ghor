using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.SmartHome.Enums
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
        [Description("SMSW6G")]
        SMART_SWITCH_6G = 58,
        [EnumMember]
        [Description("SMRB12")]
        SMART_RAINBOW_12 = 58,
        [EnumMember]
        CURTAIN = 2,
        [EnumMember]
        CAMERA = 3
    }
}
