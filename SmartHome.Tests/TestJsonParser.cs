﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartHome.Entity;
using SmartHome.Json;

namespace SmartHome.Tests
{
    [TestClass]
    public class TestJsonParser
    {
        [TestMethod]
        public void ParseJson()
        {

            string json = @"{""VersionDetails"":[{""VersionId"":1,""Id"":1,""IsSynced"":0,""DeviceType"":1,""HardwareVersion"":""00""},{""VersionId"":1,""Id"":2,""IsSynced"":0,""DeviceType"":2,""HardwareVersion"":""00""},{""VersionId"":1,""Id"":3,""IsSynced"":0,""DeviceType"":4,""HardwareVersion"":""00""},{""VersionId"":1,""Id"":4,""IsSynced"":0,""DeviceType"":3,""HardwareVersion"":""00""},{""VersionId"":1,""Id"":5,""IsSynced"":0,""DeviceType"":6,""HardwareVersion"":""00""}],""Home"":[{""Phone"":null,""IsInternet"":1,""PassPhrase"":""5260ed3f3c403ec7_A33FYT"",""IsDefault"":1,""IsSynced"":0,""Country"":null,""ZipCode"":0,""City"":null,""Name"":""MyHome"",""IsActive"":1,""Address2"":null,""Address1"":null,""Id"":1,""Zone"":null,""TimeZone"":6,""MeshMode"":0,""Block"":null}],""Channel"":[{""LoadName"":""Fan"",""DeviceTableId"":1,""ChannelNo"":1,""Id"":1,""IsSynced"":0,""LoadType"":3,""LoadWatt"":0},{""LoadName"":""Dimlight"",""DeviceTableId"":1,""ChannelNo"":2,""Id"":2,""IsSynced"":0,""LoadType"":2,""LoadWatt"":0},{""LoadName"":""Tube Light"",""DeviceTableId"":1,""ChannelNo"":3,""Id"":3,""IsSynced"":0,""LoadType"":4,""LoadWatt"":0},{""LoadName"":""CFL"",""DeviceTableId"":1,""ChannelNo"":4,""Id"":4,""IsSynced"":0,""LoadType"":5,""LoadWatt"":0},{""LoadName"":""Tube Light"",""DeviceTableId"":1,""ChannelNo"":5,""Id"":5,""IsSynced"":0,""LoadType"":4,""LoadWatt"":0},{""LoadName"":""CFL"",""DeviceTableId"":1,""ChannelNo"":6,""Id"":6,""IsSynced"":0,""LoadType"":5,""LoadWatt"":0},{""LoadName"":""Fan"",""DeviceTableId"":2,""ChannelNo"":1,""Id"":7,""IsSynced"":0,""LoadType"":3,""LoadWatt"":0},{""LoadName"":""Dimlight"",""DeviceTableId"":2,""ChannelNo"":2,""Id"":8,""IsSynced"":0,""LoadType"":2,""LoadWatt"":0},{""LoadName"":""Tube Light"",""DeviceTableId"":2,""ChannelNo"":3,""Id"":9,""IsSynced"":0,""LoadType"":4,""LoadWatt"":0},{""LoadName"":""CFL"",""DeviceTableId"":2,""ChannelNo"":4,""Id"":10,""IsSynced"":0,""LoadType"":5,""LoadWatt"":0},{""LoadName"":""Dimlight"",""DeviceTableId"":2,""ChannelNo"":5,""Id"":11,""IsSynced"":0,""LoadType"":2,""LoadWatt"":0},{""LoadName"":""Light"",""DeviceTableId"":2,""ChannelNo"":6,""Id"":12,""IsSynced"":0,""LoadType"":1,""LoadWatt"":0}],""Device"":[{""DeviceId"":32769,""Room"":1,""DeviceName"":""SMSW6G 1620029686"",""DeviceHash"":1620029686,""Watt"":0,""IsDeleted"":0,""Id"":1,""IsSynced"":0,""Version"":""00"",""DeviceType"":1},{""DeviceId"":32770,""Room"":1,""DeviceName"":""SMSW6G 1531230936"",""DeviceHash"":1531230936,""Watt"":0,""IsDeleted"":0,""Id"":2,""IsSynced"":0,""Version"":""00"",""DeviceType"":1},{""DeviceId"":32770,""Room"":2,""DeviceName"":""SMRWTR 1177190358"",""DeviceHash"":1177190358,""Watt"":0,""IsDeleted"":1,""Id"":3,""IsSynced"":0,""Version"":""00"",""DeviceType"":6},{""DeviceId"":32770,""Room"":2,""DeviceName"":""SMRWTR 356106934"",""DeviceHash"":356106934,""Watt"":0,""IsDeleted"":0,""Id"":4,""IsSynced"":0,""Version"":""00"",""DeviceType"":6}],""UserRoomLink"":[{""IsSynced"":0,""User"":2,""Room"":1,""Id"":1}],""NextAssociatedDeviceId"":[{""IsSynced"":0,""NextDeviceId"":32770}],""RouterInfo"":[{""Id"":1,""Home"":1,""Ssid"":""openWrtApl128"",""IsSynced"":false,""LocalBrokerPassword"":""kanok"",""LocalBrokerUsername"":""kanok""}],""RgbwStatus"":[],""ChannelStatus"":[{""Id"":1,""IsSynced"":0,""ChannelTableId"":1,""StatusValue"":2,""StatusType"":1},{""Id"":2,""IsSynced"":0,""ChannelTableId"":1,""StatusValue"":0,""StatusType"":3},{""Id"":3,""IsSynced"":0,""ChannelTableId"":1,""StatusValue"":0,""StatusType"":2},{""Id"":4,""IsSynced"":0,""ChannelTableId"":2,""StatusValue"":2,""StatusType"":1},{""Id"":5,""IsSynced"":0,""ChannelTableId"":2,""StatusValue"":0,""StatusType"":3},{""Id"":6,""IsSynced"":0,""ChannelTableId"":2,""StatusValue"":0,""StatusType"":2},{""Id"":7,""IsSynced"":0,""ChannelTableId"":3,""StatusValue"":2,""StatusType"":1},{""Id"":8,""IsSynced"":0,""ChannelTableId"":3,""StatusValue"":0,""StatusType"":3},{""Id"":9,""IsSynced"":0,""ChannelTableId"":3,""StatusValue"":0,""StatusType"":2},{""Id"":10,""IsSynced"":0,""ChannelTableId"":4,""StatusValue"":1,""StatusType"":1},{""Id"":11,""IsSynced"":0,""ChannelTableId"":4,""StatusValue"":0,""StatusType"":3},{""Id"":12,""IsSynced"":0,""ChannelTableId"":4,""StatusValue"":0,""StatusType"":2},{""Id"":13,""IsSynced"":0,""ChannelTableId"":5,""StatusValue"":1,""StatusType"":1},{""Id"":14,""IsSynced"":0,""ChannelTableId"":5,""StatusValue"":0,""StatusType"":3},{""Id"":15,""IsSynced"":0,""ChannelTableId"":5,""StatusValue"":0,""StatusType"":2},{""Id"":16,""IsSynced"":0,""ChannelTableId"":6,""StatusValue"":1,""StatusType"":1},{""Id"":17,""IsSynced"":0,""ChannelTableId"":6,""StatusValue"":0,""StatusType"":3},{""Id"":18,""IsSynced"":0,""ChannelTableId"":6,""StatusValue"":0,""StatusType"":2},{""Id"":19,""IsSynced"":0,""ChannelTableId"":7,""StatusValue"":0,""StatusType"":1},{""Id"":20,""IsSynced"":0,""ChannelTableId"":7,""StatusValue"":0,""StatusType"":3},{""Id"":21,""IsSynced"":0,""ChannelTableId"":7,""StatusValue"":100,""StatusType"":2},{""Id"":22,""IsSynced"":0,""ChannelTableId"":8,""StatusValue"":1,""StatusType"":1},{""Id"":23,""IsSynced"":0,""ChannelTableId"":8,""StatusValue"":0,""StatusType"":3},{""Id"":24,""IsSynced"":0,""ChannelTableId"":8,""StatusValue"":100,""StatusType"":2},{""Id"":25,""IsSynced"":0,""ChannelTableId"":9,""StatusValue"":1,""StatusType"":1},{""Id"":26,""IsSynced"":0,""ChannelTableId"":9,""StatusValue"":0,""StatusType"":3},{""Id"":27,""IsSynced"":0,""ChannelTableId"":9,""StatusValue"":100,""StatusType"":2},{""Id"":28,""IsSynced"":0,""ChannelTableId"":10,""StatusValue"":0,""StatusType"":1},{""Id"":29,""IsSynced"":0,""ChannelTableId"":10,""StatusValue"":0,""StatusType"":3},{""Id"":30,""IsSynced"":0,""ChannelTableId"":10,""StatusValue"":100,""StatusType"":2},{""Id"":31,""IsSynced"":0,""ChannelTableId"":11,""StatusValue"":0,""StatusType"":1},{""Id"":32,""IsSynced"":0,""ChannelTableId"":11,""StatusValue"":0,""StatusType"":3},{""Id"":33,""IsSynced"":0,""ChannelTableId"":11,""StatusValue"":100,""StatusType"":2},{""Id"":34,""IsSynced"":0,""ChannelTableId"":12,""StatusValue"":0,""StatusType"":1},{""Id"":35,""IsSynced"":0,""ChannelTableId"":12,""StatusValue"":0,""StatusType"":3},{""Id"":36,""IsSynced"":0,""ChannelTableId"":12,""StatusValue"":100,""StatusType"":2}],""Room"":[{""Name"":""MyRoom"",""RoomNumber"":0,""Id"":1,""Home"":1,""IsSynced"":0,""IsActive"":1},{""Name"":""room1"",""RoomNumber"":0,""Id"":2,""Home"":1,""IsSynced"":0,""IsActive"":1}],""DatabaseVersion"":2,""UserHomeLink"":[{""User"":1,""Id"":1,""Home"":1,""IsSynced"":0,""IsAdmin"":0},{""User"":2,""Id"":2,""Home"":1,""IsSynced"":0,""IsAdmin"":0}],""UserInfo"":[{""Email"":""as@as.com"",""LoginStatus"":1,""Password"":""46tUX\/XbJOPCnTLtU283wg=="",""RegStatus"":1,""UserName"":""dfs dfs"",""Id"":1,""IsSynced"":0,""Country"":""Bangladesh"",""MobileNumber"":""+8801672525252"",""Sex"":""Male""},{""Email"":""shuaib@gmail.com"",""LoginStatus"":0,""Password"":""dJOreK9VX5ZEdu8j8sqDVg=="",""RegStatus"":0,""UserName"":""Hs Nd"",""Id"":2,""IsSynced"":0,""Country"":""Bangladesh"",""MobileNumber"":""+88 01761735742"",""Sex"":""Male""}],""Version"":[{""AppName"":""SmartHome"",""Id"":1,""IsSynced"":0,""PassPhrase"":"""",""AuthCode"":""0123456789ABCDEF"",""AppVersion"":""2.0.0""}],""DeviceStatus"":[{""DeviceTableId"":1,""Id"":1,""IsSynced"":0,""StatusValue"":1,""StatusType"":53},{""DeviceTableId"":1,""Id"":2,""IsSynced"":0,""StatusValue"":0,""StatusType"":5},{""DeviceTableId"":2,""Id"":3,""IsSynced"":0,""StatusValue"":1,""StatusType"":53},{""DeviceTableId"":2,""Id"":4,""IsSynced"":0,""StatusValue"":1,""StatusType"":5},{""DeviceTableId"":3,""Id"":5,""IsSynced"":0,""StatusValue"":0,""StatusType"":5},{""DeviceTableId"":4,""Id"":6,""IsSynced"":0,""StatusValue"":1,""StatusType"":5}]}";
            HomeJsonEntity entity = JsonParser.JsonDesrialized<HomeJsonEntity>(json);
            Assert.AreEqual(entity.RouterInfo[0].Ssid, "openWrtApl128");
        }
    }
}
