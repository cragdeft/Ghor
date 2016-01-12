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
        NO_LOAD = 0,
        [EnumMember]
        NON_DIMMABLE_BULB = 1,
        [EnumMember]
        DIMMABLE_BULB = 2,
        [EnumMember]
        FAN = 3
    }
    [DataContract]
    public enum DeviceType
    {
        [EnumMember]
        SMART_SWITCH_6G = 0,
        [EnumMember]
        SMART_RAINBOW_12 = 1,
        [EnumMember]
        CURTAIN = 2,
        [EnumMember]
        CAMERA = 3
    }

    public enum MeshModeType
    {
        [EnumMember]
        BLE = 0,
        [EnumMember]
        WIFI = 1
        
    }
}
