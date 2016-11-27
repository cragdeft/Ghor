
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
    SmartSwitch6g = 1,
    [EnumMember]
    SmartRainbow12 = 2,
    [EnumMember]
    CurtainV = 3,
    [EnumMember]
    CurtainH = 4,
    [EnumMember]
    Camera = 5,
    [EnumMember]
    SmartRouter = 6
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


  public enum RGBColorStatusType
  {
    color1 = 1,
    color2 = 2,
    color3 = 3,
    color4 = 4

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

  public enum ConfigurationType
  {
    All,
    NewRoom,
    NewDevice,
    NewChannel,
    NewUser,
    DeviceRoomUpdate,
    DeleteChannel,
    DeleteDevice,
    DeleteRoom,
    NewRouterInfo,
    DeleteUser,
    NewRouter,
    TestMQTT
  }

  public enum MessageReceivedFrom
  {
    [EnumMember]
    MQTT = 1,
    [EnumMember]
    Api = 2,
    [EnumMember]
    RegisterUser = 3,
    [EnumMember]
    GetUserInfo = 4,
    [EnumMember]
    IsUserExist = 5,
    [EnumMember]
    ChangePassword = 6,
    [EnumMember]
    ConfigurationProcessFromApi = 7,
    [EnumMember]
    PasswordRecovery = 8,
    [EnumMember]
    GetFirmwareUpdate = 9,
    [EnumMember]
    NewRoom = 10,
    [EnumMember]
    DeleteRoom = 11,
    [EnumMember]
    NewUser = 12,
    [EnumMember]
    NewDevice = 13,
    [EnumMember]
    DeleteDevice = 14,
    [EnumMember]
    NewChannel = 15,
    [EnumMember]
    DeleteChannel = 16,
    [EnumMember]
    DeviceRoomUpdate = 17,
    [EnumMember]
    NewRouter = 18,
    [EnumMember]
    UpdateRoom = 19,
    [EnumMember]
    NewRoomUser = 20,
    [EnumMember]
    UpdateDevice = 21,
    [EnumMember]
    UpdateUser = 22,
    [EnumMember]
    DeleteUser = 23,
    [EnumMember]
    UpdateChannel = 24,
    [EnumMember]
    UpdateRouter = 25,
    [EnumMember]
    NewHome = 26,
    [EnumMember]
    UpdateHome = 27,
    [EnumMember]
    NewCameraConfigInfo = 28,
    [EnumMember]
    UpdateCameraConfigInfo = 29,

  }
}
