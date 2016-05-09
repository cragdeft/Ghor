using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartHome.Entity;
using SmartHome.Json;
using SmartHome.Model.Enums;
using SmartHome.Model.Models;
using SmartHome.Service.Interfaces;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SmartHome.Tests.Steps
{
    [Binding]
    public class ParseCommandSteps
    {
        public CommandJsonEntity JsonObject { get; set; }

        public List<SmartDevice> DeviceList { get; set; }

        public Mock<ICommandPerserService> CommandPerserService { get; set; }
        public Mock<CommandJsonManager> _CommandJsonManager { get; set; }
        
        [Given]
        public void Given_I_have_entered_following_property(Table table)
        {
            foreach (string jsonString in table.Rows.Select(row => row["value"]))
            {
                DeviceList = new List<SmartDevice>();
                CommandPerserService = new Mock<ICommandPerserService>();
                ScenarioContext.Current.Set<string>(jsonString);
            }
        }


        [Given]
        public void Given_I_created_a_json_with_that_string()
        {
            var jsonString = ScenarioContext.Current.Get<string>();
            _CommandJsonManager = Arrange(jsonString.ToString());

            //ScenarioContext.Current.Set<CommandJsonManager>(CommandJsonManager.Object, "SaveResult");
        }
        
        [When]
        public void When_I_parse()
        {
            _CommandJsonManager.Object.Parse();
        }

        [Then]
        public void Then_I_will_check_onoff_status()
        {
            Assert.AreEqual("1", _CommandJsonManager.Object.ChannelStatusList[0].Value);
        }

        [Then]
        public void Then_I_will_check_thermalshutdown_status()
        {
            Assert.AreEqual("1", _CommandJsonManager.Object.DeviceStatusList[0].Value);
        }

        [Then]
        public void Then_I_will_check_dimmingEnableDisable_status()
        {
            Assert.AreEqual(StatusType.DimmingEnableDisableFeedback, (StatusType)_CommandJsonManager.Object.ChannelStatusList[0].Status);
            Assert.AreEqual("1", _CommandJsonManager.Object.ChannelStatusList[0].Value);
        }

        [Then]
        public void Then_I_will_check_LoadType_status()
        {
            Assert.AreEqual(DeviceType.SmartSwitch6g, _CommandJsonManager.Object.SmartDevice.DeviceType);
            Assert.AreEqual(LoadType.NoLoad, _CommandJsonManager.Object.LoadType);
        }

        [Then]
        public void Then_I_will_check_CurrentLoadStatusThreeTwoByte_status()
        {
            Assert.AreEqual(StatusType.SmartSwitchThermalShutdown, (StatusType)_CommandJsonManager.Object.DeviceStatusList[0].StatusType);
            Assert.AreEqual("1", _CommandJsonManager.Object.DeviceStatusList[0].Value);

            Assert.AreEqual(StatusType.IndicatorOnOffFeedback, (StatusType)_CommandJsonManager.Object.DeviceStatusList[1].StatusType);
            Assert.AreEqual("2", _CommandJsonManager.Object.DeviceStatusList[1].Value);

            //channel 1
            Assert.AreEqual(StatusType.OnOffFeedback, (StatusType)_CommandJsonManager.Object.ChannelStatusList[0].Status);
            Assert.AreEqual("2", _CommandJsonManager.Object.ChannelStatusList[0].Value);

            Assert.AreEqual(StatusType.DimmingFeedback, (StatusType)_CommandJsonManager.Object.ChannelStatusList[1].Status);
            Assert.AreEqual("2", _CommandJsonManager.Object.ChannelStatusList[1].Value);

            //channel 2
            Assert.AreEqual(StatusType.OnOffFeedback, (StatusType)_CommandJsonManager.Object.ChannelStatusList[2].Status);
            Assert.AreEqual("2", _CommandJsonManager.Object.ChannelStatusList[2].Value);

            Assert.AreEqual(StatusType.DimmingFeedback, (StatusType)_CommandJsonManager.Object.ChannelStatusList[3].Status);
            Assert.AreEqual("2", _CommandJsonManager.Object.ChannelStatusList[3].Value);

            //channel 3
            Assert.AreEqual(StatusType.OnOffFeedback, (StatusType)_CommandJsonManager.Object.ChannelStatusList[4].Status);
            Assert.AreEqual("2", _CommandJsonManager.Object.ChannelStatusList[4].Value);

            Assert.AreEqual(StatusType.DimmingFeedback, (StatusType)_CommandJsonManager.Object.ChannelStatusList[5].Status);
            Assert.AreEqual("2", _CommandJsonManager.Object.ChannelStatusList[5].Value);

            //channel 4
            Assert.AreEqual(StatusType.OnOffFeedback, (StatusType)_CommandJsonManager.Object.ChannelStatusList[6].Status);
            Assert.AreEqual("2", _CommandJsonManager.Object.ChannelStatusList[6].Value);

            Assert.AreEqual(StatusType.DimmingFeedback, (StatusType)_CommandJsonManager.Object.ChannelStatusList[7].Status);
            Assert.AreEqual("2", _CommandJsonManager.Object.ChannelStatusList[7].Value);

            //chanel 5
            Assert.AreEqual(StatusType.OnOffFeedback, (StatusType)_CommandJsonManager.Object.ChannelStatusList[8].Status);
            Assert.AreEqual("2", _CommandJsonManager.Object.ChannelStatusList[8].Value);

            Assert.AreEqual(StatusType.DimmingFeedback, (StatusType)_CommandJsonManager.Object.ChannelStatusList[9].Status);
            Assert.AreEqual("2", _CommandJsonManager.Object.ChannelStatusList[9].Value);

            //channel 6
            Assert.AreEqual(StatusType.OnOffFeedback, (StatusType)_CommandJsonManager.Object.ChannelStatusList[10].Status);
            Assert.AreEqual("2", _CommandJsonManager.Object.ChannelStatusList[10].Value);

            Assert.AreEqual(StatusType.DimmingFeedback, (StatusType)_CommandJsonManager.Object.ChannelStatusList[11].Status);
            Assert.AreEqual("2", _CommandJsonManager.Object.ChannelStatusList[11].Value);
        }

        [Then]

        public void Then_I_will_check_CurrentLoadStatusEightByte_status()
        {
            Assert.AreEqual(StatusType.OnOffFeedback, (StatusType)_CommandJsonManager.Object.ChannelStatusList[0].Status);
            Assert.AreEqual("2", _CommandJsonManager.Object.ChannelStatusList[0].Value);

            Assert.AreEqual(StatusType.DimmingFeedback, (StatusType)_CommandJsonManager.Object.ChannelStatusList[1].Status);
            Assert.AreEqual("2", _CommandJsonManager.Object.ChannelStatusList[1].Value);
        }


        [Then]
        public void Then_I_will_check_DimmingFeedback_status()
        {
            Assert.AreEqual(StatusType.DimmingFeedback, (StatusType)_CommandJsonManager.Object.ChannelStatusList[0].Status);
            Assert.AreEqual("1", _CommandJsonManager.Object.ChannelStatusList[0].Value);
        }

        [Then]
        public void Then_I_will_check_RgbwSetFeedback_status()
        {
            Assert.AreEqual(StatusType.RgbwStatus, (StatusType)_CommandJsonManager.Object.DeviceStatusList[0].StatusType);
            Assert.AreEqual("1|1|1|0|1", _CommandJsonManager.Object.DeviceStatusList[0].Value);
        }

        [Then]
        public void Then_I_will_check_RainbowOnOffFeedback_status()
        {
            Assert.AreEqual(StatusType.RgbwStatus, (StatusType)_CommandJsonManager.Object.DeviceStatusList[0].StatusType);
            Assert.AreEqual("1", _CommandJsonManager.Object.DeviceStatusList[0].Value);
        }


        #region Private Methods
        private Mock<CommandJsonManager> Arrange(string jsonString)
        {
            InitializeDevice();
            InitializeJsonObject(jsonString);
            CommandPerserService.Setup(x => x.FindDevice(JsonObject.DeviceUUId))
                .Returns(FindDevice(JsonObject.DeviceUUId.ToString()));

            var mockCommandJsonManager = new Mock<CommandJsonManager>(JsonObject, CommandPerserService.Object);
            mockCommandJsonManager.Setup(x => x.FindDevice()).Returns(CommandPerserService.Object.FindDevice(JsonObject.DeviceUUId));
            return mockCommandJsonManager;
        }

        private SmartDevice FindDevice(string deviceHash)
        {
            return DeviceList.FirstOrDefault(x => x.DeviceHash == deviceHash);
        }

        private void InitializeDevice()
        {
            AddDemoDevice();

            foreach (var device in DeviceList)
            {
                var demoChannel = GetDemoChannel(device);
                var demoChannelList = new List<Channel>() { demoChannel };
                ((SmartSwitch)device).Channels = demoChannelList;

                var demoChannelStatusList = GetDemoChannelStatusList(demoChannel);

                demoChannel.ChannelStatuses = demoChannelStatusList;

                var demoDeviceStatusList = GetDemoDeviceStatusList(device);
                device.DeviceStatus = demoDeviceStatusList;
            }
        }

        private List<DeviceStatus> GetDemoDeviceStatusList(SmartDevice device)
        {
            return new List<DeviceStatus>()
            {
                new DeviceStatus()
                {
                    Value = "1",
                    AuditField = null,
                    SmartDevice = device,
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

        private Channel GetDemoChannel(SmartDevice device)
        {
            return new Channel()
            {
               // SmartDevice = device,
                AuditField = null,
                ChannelId = 1,
                ChannelNo = 1,
                ChannelStatuses = null,
                LoadName = "2",
                LoadType = LoadType.NoLoad
            };
        }

        private void AddDemoDevice()
        {
            var device = new SmartDevice()
            {
                AuditField = null,
                DeviceHash = "2094027172",
                DeviceName = "demo",
                DeviceType = 0,
                IsDeleted = false,
                Mac = "mac",
                Watt = null,
                DeviceId = 32769,
                DeviceVersion = "0",
                DeviceStatus = null
               
            };

            DeviceList.Add(device);

            device = new SmartDevice()
            {
                AuditField = null,
                DeviceHash = "2094027173",
                DeviceName = "demo",
                DeviceType = (DeviceType) 1,
                IsDeleted = false,
                Mac = "mac",
                Watt = null,
                DeviceId = 32767,
                DeviceVersion = "0",
                DeviceStatus = null
               
            };

            DeviceList.Add(device);
        }

        private void InitializeJsonObject(string jsonString)
        {
            JsonObject = CommandJsonManager.JsonDesrialized<CommandJsonEntity>(jsonString);
            JsonObject.CommandType = CommandType.Feedback;
        }
        #endregion
    }
}
