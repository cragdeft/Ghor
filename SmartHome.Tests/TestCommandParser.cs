using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Json;
using SmartHome.Model.Enums;
using SmartHome.Model.Models;
using SmartHome.MQTT.Client;
using SmartHome.Service;
using SmartHome.Service.Interfaces;

namespace SmartHome.Tests
{
    [TestClass]
    public class TestCommandJsonManager
    {
        public CommandJsonEntity JsonObject { get; set; }
        public Device Device { get; set; }

        public List<Device> DeviceList { get; set; }

        public Mock<ICommandPerserService> CommandPerserService { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            DeviceList = new List<Device>();
            CommandPerserService = new Mock<ICommandPerserService>();
            
        }

        [TestMethod]
        public void TestOnOffFeedback_ShouldParseCommand()
        {
            var jsonString =
              "{ \"response\": true, \"device_version\": \"00\", \"email\": \"hvuv@vuu.com\", \"device_uuid\": 2094027172,\"device_id\": 32769, \"mac_id\": \"mac\",\"command_byte\": \"[1, 2, 1, 1, 1, 255, 255, -5]\",\"command_id\": 1}";

            //Arrange
            var mockCommandJsonManager = Arrange(jsonString);
            //act
            mockCommandJsonManager.Object.Parse();
            // Assert
            Assert.AreEqual(JsonObject.DeviceUUId.ToString(), mockCommandJsonManager.Object.Device.DeviceHash);
            Assert.AreEqual(CommandId.DeviceOnOffFeedback, mockCommandJsonManager.Object.CommandId);
            Assert.AreEqual(DeviceType.SMART_SWITCH_6G, mockCommandJsonManager.Object.Device.DeviceType);
            Assert.AreEqual(StatusType.OnOffFeedback, (StatusType)mockCommandJsonManager.Object.ChannelStatusList[0].Status);
            Assert.AreEqual("1", mockCommandJsonManager.Object.ChannelStatusList[0].Value);
        }
        [TestMethod]
        public void TestThermalShutDown_ShouldParseCommand()
        {
            var jsonString =
              "{ \"response\": true, \"device_version\": \"00\", \"email\": \"hvuv@vuu.com\", \"device_uuid\": 2094027172,\"device_id\": 32769, \"mac_id\": \"mac\",\"command_byte\": \"[1, 56, 0, 1, 1, 255, 255, -5]\",\"command_id\": 1}";

            //Arrange
            var mockCommandJsonManager = Arrange(jsonString);
            //act
            mockCommandJsonManager.Object.Parse();
            // Assert
            Assert.AreEqual(JsonObject.DeviceUUId.ToString(), mockCommandJsonManager.Object.Device.DeviceHash);
            Assert.AreEqual(CommandId.SmartSwitchThermalShutdownNotificationFeedback, mockCommandJsonManager.Object.CommandId);
            Assert.AreEqual(DeviceType.SMART_SWITCH_6G, mockCommandJsonManager.Object.Device.DeviceType);
            Assert.AreEqual(StatusType.ThermalShutDownResponse, (StatusType)mockCommandJsonManager.Object.DeviceStatusList[0].StatusType);
            Assert.AreEqual(1, mockCommandJsonManager.Object.DeviceStatusList[0].Value);
        }
        [TestMethod]
        public void TestDimmingFeedback_ShouldParseCommand()
        {
            var jsonString =
               "{ \"response\": true, \"device_version\": \"00\", \"email\": \"hvuv@vuu.com\", \"device_uuid\": 2094027172,\"device_id\": 32769, \"mac_id\": \"mac\",\"command_byte\": \"[1, 52, 0, 1, 1, 255, 255, -5]\",\"command_id\": 1}";

            //Arrange
            var mockCommandJsonManager = Arrange(jsonString);
            //act
            mockCommandJsonManager.Object.Parse();
            // Assert
            Assert.AreEqual(JsonObject.DeviceUUId.ToString(), mockCommandJsonManager.Object.Device.DeviceHash);
            Assert.AreEqual(CommandId.SmartSwitchDimmingFeedback, mockCommandJsonManager.Object.CommandId);
            Assert.AreEqual(DeviceType.SMART_SWITCH_6G, mockCommandJsonManager.Object.Device.DeviceType);
        }
        [TestMethod]
        public void TestDemmingEnableDisableFeedback_ShouldParseCommand()
        {
            var jsonString =
               "{ \"response\": true, \"device_version\": \"00\", \"email\": \"hvuv@vuu.com\", \"device_uuid\": 2094027172,\"device_id\": 32769, \"mac_id\": \"mac\",\"command_byte\": \"[1, 64, 0, 1, 1, 255, 255, -5]\",\"command_id\": 1}";

            //Arrange
            var mockCommandJsonManager = Arrange(jsonString);
            //act
            mockCommandJsonManager.Object.Parse();
            // Assert
            Assert.AreEqual(JsonObject.DeviceUUId.ToString(), mockCommandJsonManager.Object.Device.DeviceHash);
            Assert.AreEqual(CommandId.SmartSwitchHardwareDimmingFeedback, mockCommandJsonManager.Object.CommandId);
            Assert.AreEqual(DeviceType.SMART_SWITCH_6G, mockCommandJsonManager.Object.Device.DeviceType);
        }
        [TestMethod]
        public void LoadTypeFeedback_ShouldParseCommand()
        {
            var jsonString =
              "{ \"response\": true, \"device_version\": \"00\", \"email\": \"hvuv@vuu.com\", \"device_uuid\": 2094027172,\"device_id\": 32769, \"mac_id\": \"mac\",\"command_byte\": \"[1, 58, 0, 1, 1, 255, 255, -5]\",\"command_id\": 1}";

            //Arrange
            var mockCommandJsonManager = Arrange(jsonString);
            //act
            mockCommandJsonManager.Object.Parse();
            // Assert
            Assert.AreEqual(JsonObject.DeviceUUId.ToString(), mockCommandJsonManager.Object.Device.DeviceHash);
            Assert.AreEqual(CommandId.SmartSwitchLoadTypeSelectFeedback, mockCommandJsonManager.Object.CommandId);
            Assert.AreEqual(DeviceType.SMART_SWITCH_6G, mockCommandJsonManager.Object.Device.DeviceType);
        }
        [TestMethod]
        public void CurrentLoadStatus32ByteFeedback_ShouldParseCommand()
        {
            var jsonString =
              "{ \"response\": true, \"device_version\": \"00\", \"email\": \"hvuv@vuu.com\", \"device_uuid\": 2094027172,\"device_id\": 32769, \"mac_id\": \"mac\",\"command_byte\": \"[1, 6, 0, 1, 1, 255, 255, -5]\",\"command_id\": 1}";

            //Arrange
            var mockCommandJsonManager = Arrange(jsonString);
            //act
            mockCommandJsonManager.Object.Parse();
            // Assert
            Assert.AreEqual(JsonObject.DeviceUUId.ToString(), mockCommandJsonManager.Object.Device.DeviceHash);
            Assert.AreEqual(CommandId.DeviceCurrentLoadStatusFeedback, mockCommandJsonManager.Object.CommandId);
            Assert.AreEqual(DeviceType.SMART_SWITCH_6G, mockCommandJsonManager.Object.Device.DeviceType);
        }
        [TestMethod]
        public void CurrentLoadStatus8ByteFeedback_ShouldParseCommand()
        {
            var jsonString =
              "{ \"response\": true, \"device_version\": \"00\", \"email\": \"hvuv@vuu.com\", \"device_uuid\": 2094027172,\"device_id\": 32769, \"mac_id\": \"mac\",\"command_byte\": \"[1, 6, 0, 1, 1, 255, 255, -5]\",\"command_id\": 1}";

            //Arrange
            var mockCommandJsonManager = Arrange(jsonString);
            //act
            mockCommandJsonManager.Object.Parse();
            // Assert
            Assert.AreEqual(JsonObject.DeviceUUId.ToString(), mockCommandJsonManager.Object.Device.DeviceHash);
            Assert.AreEqual(CommandId.DeviceCurrentLoadStatusFeedback, mockCommandJsonManager.Object.CommandId);
            Assert.AreEqual(DeviceType.SMART_SWITCH_6G, mockCommandJsonManager.Object.Device.DeviceType);
        }

        #region Private Methods
        private Mock<CommandJsonManager> Arrange(string jsonString)
        {
            InitializeDevice();
            InitializeJsonObject(jsonString);
            CommandPerserService.Setup(x => x.FindDevice(JsonObject.DeviceUUId))
                .Returns(FindDevice(JsonObject.DeviceUUId.ToString()));

            var mockCommandJsonManager = new Mock<CommandJsonManager>(JsonObject, CommandPerserService.Object);
            mockCommandJsonManager.Setup(x => x.FindDevice()).Returns(Device);
            return mockCommandJsonManager;
        }

        private Device FindDevice(string deviceHash)
        {
            return DeviceList.FirstOrDefault(x => x.DeviceHash == deviceHash);
        }

        private void InitializeDevice()
        {
            GetDemoDevice();
            var demoChannel = GetDemoChannel();
            var demoChannelList = new List<Channel>() { demoChannel };
            Device.Channels = demoChannelList;

            var demoChannelStatusList = GetDemoChannelStatusList(demoChannel);

            demoChannel.ChannelStatuses = demoChannelStatusList;

            var demoDeviceStatusList = GetDemoDeviceStatusList();
            Device.DeviceStatus = demoDeviceStatusList;

            DeviceList.Add(Device);
        }

        private List<DeviceStatus> GetDemoDeviceStatusList()
        {
            return new List<DeviceStatus>()
            {
                new DeviceStatus()
                {
                    Value = 1,
                    AuditField = null,
                    Device = Device,
                    Status = 1,
                    StatusType = StatusType.CurrentLoadStatus,
                    DeviceStatusId = 1
                }
            };
        }

        private static List<ChannelStatus> GetDemoChannelStatusList(Channel demoChannel)
        {
            return new List<ChannelStatus>()
            {
                new ChannelStatus()
                {
                    Channel = demoChannel,
                    AuditField = null,
                    ChannelNo = 1,
                    ChannelStatusId = 1,
                    Status = ChannelStatusType.Dimmable,
                    Value = 1
                }
            };
        }

        private Channel GetDemoChannel()
        {
            return new Channel()
            {
                Device = Device,
                AuditField = null,
                ChannelId = 1,
                ChannelNo = 1,
                ChannelStatuses = null,
                LoadName = "2",
                LoadType = null
            };
        }

        private void GetDemoDevice()
        {
            Device = new Device()
            {
                AuditField = null,
                DeviceHash = "2094027172",
                DeviceName = "demo",
                DeviceType = 0,
                IsDeleted = false,
                Mac = "mac",
                Watt = null,
                DeviceId = 1,
                DeviceVersion = "0",
                DeviceStatus = null,
                Channels = null
            };
        }

        private void InitializeJsonObject(string jsonString)
        {
            JsonObject = CommandJsonManager.JsonDesrialized<CommandJsonEntity>(jsonString);
            JsonObject.CommandType = CommandType.Feedback;
        } 
        #endregion
    }
}   
