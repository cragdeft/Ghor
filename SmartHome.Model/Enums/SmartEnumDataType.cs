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

    public enum ChannelId
    {
        SmartSwitchChannelAll,  
        SmartSwitchChannel1,
        SmartSwitchChannel2,
        SmartSwitchChannel3,
        SmartSwitchChannel4,
        SmartSwitchChannel5,
        SmartSwitchChannel6

    }

    public enum CommandId
    {


        DeviceOnOffRequestCommandId = 1,
        DeviceOnOffFeedbackCommandId = 2,
        DeviceCurrentLoadStatusRequestCommandId = 5,
        DeviceCurrentLoadStatusFeedbackCommandId = 6,
        DevicePingRequestCommandId = 25,
        SmartSwitchIndicatorPowerRequestCommandId = 53, //calls constructor with value 53
        SmartSwitchIndicatorPowerFeedbackCommandId = 54,
        SmartSwitchThermalShutdownNotificationRequestCommandId = 55,
        SmartSwitchThermalShutdownNotificationFeedbackCommandId = 56,

        SmartSwitchLoadTypeSelectRequestCommandId = 57,
        SmartSwitchLoadTypeSelectFeedbackCommandId = 58,

        SmartSwitchDimmingRequestCommandId = 51,
        SmartSwitchDimmingFeedbackCommandId = 52,
        SmartSwitchHardwareDimmingRequestCommandId = 63,
        SmartSwitchHardwareDimmingFeedbackCommandId = 64,
        DeviceHardResetRequestCommandId = 17,
        DeviceHardResetFeedbackCommandId = 18,
        DevicePowerCommandId = 200,
        DeviceRgbwStatusCommandId = 201,
        DeviceRgbwSetCommandId = 202

    }

    public enum StatusType
    {
        DeviceSwitchable = 1,
        SmartSwitchIndicator = 53,  
        DeviceActive = 5,
        SmartSwitchThermalShutdown = 55,
        RgbwStatus = 200,
        RgbwCustomColor = 201

    }

    public enum TopicType
    {
        Command,
        Configuration
    }
}
