
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
        NoLoad = 0,
        [EnumMember]
        NonDimmableBulb = 1,
        [EnumMember]
        DimmableBulb = 2,
        [EnumMember]
        Fan = 3,
        Tubelight = 4,
        Cfl = 5
    }
    [DataContract]
    public enum DeviceType
    {
        [EnumMember]
        SmartSwitch6g = 0,
        [EnumMember]
        SmartRainbow12 = 1,
        [EnumMember]
        Curtain = 2,
        [EnumMember]
        Camera = 3
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
        SmartSwitchChannelAll = 0,
        SmartSwitchChannel1 = 1,
        SmartSwitchChannel2 = 2,
        SmartSwitchChannel3 = 3,
        SmartSwitchChannel4 = 4,
        SmartSwitchChannel5 = 5,
        SmartSwitchChannel6 = 6

    }

    public enum CommandId
    {


        DeviceOnOffRequest = 1,
        DeviceOnOffFeedback = 2,
        DeviceCurrentLoadStatusRequest = 5,
        DeviceCurrentLoadStatusFeedback = 6,
        DevicePingRequest = 25,
        SmartSwitchIndicatorPowerRequest = 53, //calls constructor with value 53
        SmartSwitchIndicatorPowerFeedback = 54,
        SmartSwitchThermalShutdownNotificationRequest = 55,
        SmartSwitchThermalShutdownNotificationFeedback = 56,

        SmartSwitchLoadTypeSelectRequest = 57,
        SmartSwitchLoadTypeSelectFeedback = 58,

        SmartSwitchDimmingRequest = 51,
        SmartSwitchDimmingFeedback = 52,
        SmartSwitchHardwareDimmingRequest = 63,
        SmartSwitchHardwareDimmingFeedback = 64,
        DeviceHardResetRequest = 17,
        DeviceHardResetFeedback = 18,
        //DevicePower = 200,
        SmartRainbowPower = 97,
        //DeviceRgbwStatus = 201,
        SmartRainbowRgbwStatus = 98,
        //DeviceRgbwSet = 202
        SmartRainbowRgbw = 99

    }

    public enum StatusType
    {
        DeviceSwitchable = 1,
        SmartSwitchIndicator = 53,
        DimmingFeedback = 52,
        OnOffFeedback = 2,
        CurrentLoadStatus = 6,
        DeviceActive = 5,
        IndicatorOnOffFeedback = 54,
        SmartSwitchThermalShutdown = 55,
        ThermalShutDownResponse = 56,
        LoadTypeSelectFeedback = 58,
        DimmingEnableDisableFeedback = 64,
        RgbwStatus = 98,
        RgbwCustomColor = 201

    }

    public enum ChannelStatusType
    {
        Switchable = 1,
        Dimmable = 2,
        HardwareDimSwitchable = 3

    }



    public enum CommandType
    {
        Command,
        Configuration,
        Feedback
    }
}
