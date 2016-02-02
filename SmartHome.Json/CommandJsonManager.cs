using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.ModelDataContext;
using SmartHome.Model.Models;
using SmartHome.Service;
using SmartHome.Service.Interfaces;

namespace SmartHome.Json
{
    public class CommandJsonManager
    {
        #region Property

        #region Private
        private static IUnitOfWorkAsync _unitOfWorkAsync;
        private static IDeviceService _deviceService;
        private static ICommandPerserService _commandPerserService;
        private CommandJson _commandJson { get; set; }
        #endregion

        #region Public
        public List<DeviceStatusEntity> DeviceStatusList { get; set; }
        public List<ChannelStatusEntity> ChannelStatusList { get; set; }
        public int Length { get; set; }
        public int Initiator { get; set; }

        public CommandId CommandId { get; set; }
        #endregion

        #endregion

        #region Constructor
        public CommandJsonManager(CommandJson commandJson)
        {
            IDataContextAsync context = new SmartHomeDataContext();
            _unitOfWorkAsync = new UnitOfWork(context);
            _commandPerserService = new CommandParserService(_unitOfWorkAsync,commandJson.EmailAddress);

            DeviceStatusList = new List<DeviceStatusEntity>();
            ChannelStatusList = new List<ChannelStatusEntity>();
            _commandJson = commandJson;
        }
        #endregion

        #region Methods
        public void Parse()
        {
            #region Parse

            var values = _commandJson.Command.Replace("[{", string.Empty).Replace("}]", string.Empty).Split(',');
            int loopLength;

            for (int index = 0; index < values.Length; index++)
            {
                int value = Convert.ToInt32(values[index]);

                if (value < 0)
                {
                    value += 255;
                }


                if (index == 0)
                {
                    Initiator = Convert.ToInt32(values[index]);
                }

                else if (index == 1)
                {
                    CommandId = (CommandId)Enum.ToObject(typeof(CommandId), Convert.ToInt32(values[index]));
                }
                else if (index == 2)
                {

                    Length = Convert.ToInt32(values[index]);
                }
                else if (index == 3)
                {
                    DeviceStatusEntity deviceStatus = new DeviceStatusEntity();
                    deviceStatus.StatusType = (int)StatusType.SmartSwitchThermalShutdown;
                    deviceStatus.Value = value;
                    deviceStatus.DId = _commandJson.DeviceID.ToString();
                    DeviceStatusList.Add(deviceStatus);

                    break;
                }

            }

            if (Length == 32)
                loopLength = 28;
            else
                loopLength = 8;

            ChannelStatusEntity channelStatus = new ChannelStatusEntity(); ;
            for (int i = 4; i < loopLength; i++)
            {
                int value = Convert.ToInt32(values[i]);

                if (value < 0)
                {
                    value += 255;
                }

                if (i % 4 == 0)
                {

                    channelStatus = new ChannelStatusEntity();
                    channelStatus.CId = (int)((ChannelId)Enum.ToObject(typeof(ChannelId), value));

                }
                else if (i % 5 == 0)
                {
                    channelStatus.Status = (int)StatusType.DeviceActive;
                    channelStatus.Value = value.ToString();
                }
                else if (i % 6 == 0)
                {
                    channelStatus.Status = (int)StatusType.DeviceActive;
                    channelStatus.Value = value.ToString();
                }
                else if (i % 7 == 0)
                {
                    channelStatus.Status = (int)StatusType.DeviceActive;
                    channelStatus.Value = value.ToString();

                    DeviceStatusEntity deviceStatus = new DeviceStatusEntity();
                    deviceStatus.StatusType = (int)StatusType.SmartSwitchIndicator;
                    deviceStatus.Value = Convert.ToInt32(values[i]);
                    deviceStatus.DId = _commandJson.DeviceID.ToString();
                    DeviceStatusList.Add(deviceStatus);
                    ChannelStatusList.Add(channelStatus);
                }


            }
            #endregion

        }

        public void SaveOrUpDateStatus()
        {
            foreach (var ds in DeviceStatusList)
            {
                Device entity = _commandPerserService.FindDevice(Convert.ToInt32((_commandJson.DeviceID)));

                if (entity != null)
                {
                    DeviceStatus deviceStatus = _commandPerserService.FindDeviceStatus(entity.DeviceId, entity.Id);

                    if (deviceStatus != null)
                    {
                        deviceStatus.StatusType = ds.StatusType;
                        deviceStatus.Status = ds.Value;
                        _commandPerserService.UpdateDeviceStatus(deviceStatus);
                    }
                    else
                    {
                        deviceStatus = new DeviceStatus();
                        deviceStatus.DId = entity.DeviceId;
                        deviceStatus.StatusType = ds.StatusType;
                        deviceStatus.Status = ds.Value;

                        _commandPerserService.AddDeviceStatus(deviceStatus);
                    }
                }
                else
                {
                    entity = new Device();
                    entity.DeviceHash = _commandJson.DeviceUUID.ToString();
                    entity.Mac = _commandJson.MacID;
                    entity.Id = Convert.ToInt32((_commandJson.DeviceID));
                    entity.DeviceVersion = _commandJson.DeviceVersion;

                    entity = _commandPerserService.AdddDevice(entity);

                    DeviceStatus deviceStatus = new DeviceStatus();
                    deviceStatus.DId = entity.DeviceId;
                    deviceStatus.StatusType = ds.StatusType;
                    deviceStatus.Status = ds.Value;

                    _commandPerserService.AddDeviceStatus(deviceStatus);
                }
            }
        }


        //CommandParser fp = new CommandParser(commandArray);

        //    Device entity = _commandPerserService.FindDevice(Convert.ToInt32((commandJson.DeviceID)));


        //    Channel channel = _commandPerserService.FindChannel(Convert.ToInt32((commandJson.DeviceID)));
        //    foreach (var channelValue in fp.ChannelValueList)
        //    {
        //        //update channelstatus
        //    }


        //}

        #endregion
    }
}
