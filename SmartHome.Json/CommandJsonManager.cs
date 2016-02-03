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

        private string[] CommandArray { get; set; }
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
            InitializeParameters(commandJson);

            InitializeList();
        }

        private void InitializeParameters(CommandJson commandJson)
        {
            IDataContextAsync context = new SmartHomeDataContext();
            _unitOfWorkAsync = new UnitOfWork(context);
            _commandPerserService = new CommandParserService(_unitOfWorkAsync, commandJson.EmailAddress);
            _commandJson = commandJson;
            CommandArray = _commandJson.Command.Replace("[{", string.Empty).Replace("}]", string.Empty).Split(',');
            Length = CommandArray.Length;
        }

        private void InitializeList()
        {
            DeviceStatusList = new List<DeviceStatusEntity>();
            ChannelStatusList = new List<ChannelStatusEntity>();
        }

        #endregion

        #region Methods

        public void Parse()
        {

            Initiator = GetInitiator();

            CommandId = GetCommandId();

            switch (CommandId)
            {
                case CommandId.DeviceOnOffFeedback:
                    OnOffFeedbackCommandParse();
                    break;
                case CommandId.DeviceCurrentLoadStatusFeedback:
                    CurrentLoadStatusParse();
                    break;
                case CommandId.SmartSwitchDimmingFeedback:
                    DimmingFeedbackCommandParse();
                    break;
                case CommandId.SmartSwitchThermalShutdownNotificationFeedback:
                    ThermalShutDownCommandParse();
                    SaveOrUpDateStatus();
                    break;
                case CommandId.SmartSwitchLoadTypeSelectFeedback:
                    LoadTypeSelectCommandParse();
                    break;
                case CommandId.SmartSwitchHardwareDimmingFeedback:
                    DimmingFeedbackEnableDisableCommandParse();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SaveOrUpDateStatus();

        }

        private CommandId GetCommandId()
                {
            return (CommandId) Enum.ToObject(typeof (CommandId), GetValue(CommandArray[1]));
        }

        private int GetInitiator()
        {
            return Convert.ToInt32(GetValue(CommandArray[0]));
        }

        private int GetValue(string arg)
        {
            int value = Convert.ToInt32(arg);

            if (value < 0)
                    value += 255;

            return value;
                }

        private void CurrentLoadStatusParse()
        {
            GetDeviceStatus(StatusType.SmartSwitchThermalShutdown);

            if (CommandArray.Length > 30)
                {
                ParseCurrentLoadStatusCommand(28);
                }
            else
            {
                ParseCurrentLoadStatusCommand(8);
            }
        }

        private void ParseCurrentLoadStatusCommand(int length)
                {

            for (int i = 4; i < length; i++)
            {
                if (i % 5 == 0)
                {
                    AddChannelStatusToList(GetValue(CommandArray[i - 1]), StatusType.OnOffFeedback, GetValue(CommandArray[i]));
                }
                else if (i % 6 == 0)
                {
                    AddChannelStatusToList(GetValue(CommandArray[i - 2]), StatusType.DimmingFeedback, GetValue(CommandArray[i]));
                    AddDeviceStatusToList(StatusType.DimmingFeedback, GetValue(CommandArray[i]));
                }
                else if (i % 7 == 0)
                {
                    AddChannelStatusToList(GetValue(CommandArray[i - 3]), StatusType.IndicatorOnOffFeedback, GetValue(CommandArray[i]));
                    AddDeviceStatusToList(StatusType.SmartSwitchIndicator, GetValue(CommandArray[i]));
                }


                }
        }

        private void AddChannelStatusToList(int channelNo,StatusType type,int value)
                {
            ChannelStatusEntity channelStatus = new ChannelStatusEntity
            {
                ChannelNo = channelNo,
                Status = (int) type,
                Value = value.ToString()
            };

            ChannelStatusList.Add(channelStatus);

                }

        private void AddDeviceStatusToList( StatusType type, int value)
        {
            DeviceStatusEntity deviceStatus = new DeviceStatusEntity
            {
                DId = _commandJson.DeviceID.ToString(),
                StatusType = (int)type,
                Value = value
            };

            DeviceStatusList.Add(deviceStatus);

            }


        private void OnOffFeedbackCommandParse()
            {
            GetChannelStatus(StatusType.OnOffFeedback);
        }

        private void DimmingFeedbackCommandParse()
                {
            GetChannelStatus(StatusType.DimmingFeedback);

                }

        private void DimmingFeedbackEnableDisableCommandParse()
                {
            GetChannelStatus(StatusType.DimmingEnableDisableFeedback);

                }

        private void LoadTypeSelectCommandParse()
                {
            GetChannelStatus(StatusType.LoadTypeSelectFeedback);
                }

        private void ThermalShutDownCommandParse()
                {
            GetDeviceStatus(StatusType.ThermalShutDownResponse);
                }

        private void SaveOrUpDateStatus()
                {

            var device = _commandPerserService.FindDevice(Convert.ToInt32((_commandJson.DeviceID)));

            if (device == null)
                device = AddNewDevice();


            SaveDeviceStatus(device);


            SaveChannelStatus();
                }

        private void SaveChannelStatus()
        {
            foreach (var channelValue in ChannelStatusList)
            {
                var channel = _commandPerserService.FindChannel(_commandJson.DeviceID,channelValue.ChannelNo);

                if (channel != null)
                {

            }
            }
        }

        private void SaveDeviceStatus(Device entity)
        {
            foreach (var ds in DeviceStatusList)
            {
                DeviceStatus deviceStatus = _commandPerserService.FindDeviceStatus(entity.DeviceId, ds.StatusType);

                if (deviceStatus != null)
                {
                    deviceStatus.StatusType =(StatusType) ds.StatusType;
                    deviceStatus.Status = ds.Value;
                    _commandPerserService.UpdateDeviceStatus(deviceStatus);
                }
                else
                {
                    deviceStatus = new DeviceStatus();
                    deviceStatus.DId = entity.DeviceId;
                    deviceStatus.StatusType = (StatusType)ds.StatusType;
                    deviceStatus.Status = ds.Value;

                    _commandPerserService.AddDeviceStatus(deviceStatus);
                }
            }
        }

        private static void AddDeviceStatus(Device entity, DeviceStatusEntity ds)
                {
            var deviceStatus = new DeviceStatus();
                    deviceStatus.DId = entity.DeviceId;
                    deviceStatus.StatusType = (StatusType)ds.StatusType;
                    deviceStatus.Status = ds.Value;

                    _commandPerserService.AddDeviceStatus(deviceStatus);
                }

        private static void UpdateDevice(DeviceStatus deviceStatus, DeviceStatusEntity ds)
        {
            deviceStatus.Status = ds.Value;
            _commandPerserService.UpdateDeviceStatus(deviceStatus);
        }

        private Device AddNewDevice()
        {
            Device entity = new Device
            {
                DeviceHash = _commandJson.DeviceUUID.ToString(),
                Mac = _commandJson.MacID,
                Id = Convert.ToInt32((_commandJson.DeviceID)),
                DeviceVersion = _commandJson.DeviceVersion
            };

            return _commandPerserService.AdddDevice(entity);
        }

        private int GetChannelNoOfCommunicationProtocol()
        {
            return Convert.ToInt32(GetValue(CommandArray[2]));
        }

        private int GetValueOfCommunicationProtocol()
        {
            return Convert.ToInt32(GetValue(CommandArray[3]));
        }

        private void GetDeviceStatus(StatusType status)
        {
            DeviceStatusEntity deviceStatus = new DeviceStatusEntity();
            deviceStatus.DId = _commandJson.DeviceID.ToString();
            deviceStatus.StatusType = (int)status;
            deviceStatus.Value = GetValueOfCommunicationProtocol();
            DeviceStatusList.Add(deviceStatus);
        }

        private void GetChannelStatus(StatusType status)
        {
            ChannelStatusEntity channelStatus = new ChannelStatusEntity();
            channelStatus.Status = (int)status;
            channelStatus.ChannelNo = GetChannelNoOfCommunicationProtocol();
            channelStatus.Value = GetValueOfCommunicationProtocol().ToString();
            ChannelStatusList.Add(channelStatus);
        }

        #endregion
    }
}
