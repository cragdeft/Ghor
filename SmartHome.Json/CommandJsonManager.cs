using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        private static ICommandPerserService _commandPerserService;
        private CommandJsonEntity _commandJson { get; set; }
        private Device Device { get; set; }

        private string[] CommandArray { get; set; }
        #endregion

        #region Public
        public List<DeviceStatusEntity> DeviceStatusList { get; set; }
        public List<ChannelStatusEntity> ChannelStatusList { get; set; }
        public int Length { get; set; }
        public byte Initiator { get; set; }
        public CommandId CommandId { get; set; }
        
        #endregion

        #endregion

        #region Constructor


        public CommandJsonManager(CommandJsonEntity commandJson)
        {
            InitializeParameters(commandJson);

            InitializeList();
        }

        public CommandJsonManager(CommandJsonEntity commandJson, ICommandPerserService commandPerserService)
        {
            _commandPerserService = commandPerserService;
            _commandJson = commandJson;
            SetupCommandArrayAndLength();
            
            InitializeList();

        }

        private void SetupCommandArrayAndLength()
        {
            CommandArray = _commandJson.Command.Replace("[", string.Empty).Replace("]", string.Empty).Split(',');
            Length = CommandArray.Length;
        }

        private void InitializeParameters(CommandJsonEntity commandJson)
        {
            IDataContextAsync context = new SmartHomeDataContext();
            _unitOfWorkAsync = new UnitOfWork(context);
            _commandPerserService = new CommandParserService(_unitOfWorkAsync, commandJson.EmailAddress);
            _commandJson = commandJson;
            SetupCommandArrayAndLength();
            
        }

        private void InitializeList()
        {
            DeviceStatusList = new List<DeviceStatusEntity>();
            ChannelStatusList = new List<ChannelStatusEntity>();
        }

        #endregion

        #region Methods

        public void LogCommand(bool isProcessed, string reason)
        {
            _commandJson.IsProcessed = isProcessed;
            _commandJson.ProcessFailReason = reason;
            _commandPerserService.LogCommand(_commandJson);
        }
        public void Parse()
        {
            Device = FindDevice();

            if (Device == null)
            {
                LogCommand(false, "Device not found.");
                return;
            }
            else
            {
                ParseInitiatorAndSetVersionValue();

                CommandId = GetCommandId();


                switch (CommandId)
                {
                    case CommandId.DevicePingRequest:
                        LogCommand(true, "");
                        return;
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
                        break;
                    case CommandId.SmartSwitchLoadTypeSelectFeedback:
                        LoadTypeSelectCommandParse();
                        break;
                    case CommandId.SmartSwitchHardwareDimmingFeedback:
                        DimmingFeedbackEnableDisableCommandParse();
                        break;
                    default:
                        return;
                }

                SaveOrUpDateStatus();
            }
        }

        public virtual Device FindDevice()
        {
            return _commandPerserService.FindDevice(_commandJson.DeviceUUId);
        }

        private CommandId GetCommandId()
        {
            _commandJson.CommandId = GetValue(CommandArray[1]);
            return (CommandId)Enum.ToObject(typeof(CommandId), _commandJson.CommandId);
        }

        private void ParseInitiatorAndSetVersionValue()
        {
            GetInitiator();

            bool[] directionBoolArray = new bool[5];
            bool[] versionBoolArray = new bool[3];
            BitArray cds0 = new BitArray(BitConverter.GetBytes(Initiator).ToArray());

            IterateByte(cds0, ref directionBoolArray, ref versionBoolArray);

            _commandJson.DeviceVersion = GetIntFromBitArray(new BitArray(versionBoolArray)).ToString();

        }

        private void GetInitiator()
        {
            Initiator = Convert.ToByte(CommandArray[0]);
        }

        private static void IterateByte(BitArray cds0, ref bool[] directionBoolArray, ref bool[] versionBoolArray)
        {
            for (int i = 0; i < 8; i++)
            {
                if (i <= 4)
                    directionBoolArray[i] = cds0[i];
                else
                    versionBoolArray[i - 5] = cds0[i];
            }
        }

        private int GetIntFromBitArray(BitArray bitArray)
        {

            if (bitArray.Length > 32)
                throw new ArgumentException("Argument length shall be at most 32 bits.");

            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];

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
            if (Device.DeviceType != DeviceType.SMART_SWITCH_6G) return;

            if (Length == 32)
            {
                GetDeviceStatusFromNumber3Bit(StatusType.SmartSwitchThermalShutdown);
                GetDeviceStatusFromNumber7Bit(StatusType.IndicatorOnOffFeedback);
                ParseCurrentLoadStatusCommand(8);
            }
            else
            {
                ParseCurrentLoadStatusCommand(4);
            }

        }

        private void ParseCurrentLoadStatusCommand(int startingIndex)
        {
            for (int i = startingIndex; i < Length; i += 4)
            {
                AddChannelStatusToList(GetValue(CommandArray[i]), StatusType.OnOffFeedback, GetValue(CommandArray[i + 1]));

                AddChannelStatusToList(GetValue(CommandArray[i]), StatusType.DimmingFeedback, GetValue(CommandArray[i + 2]));

                //AddChannelStatusToList(GetValue(CommandArray[i - 1]), StatusType.IndicatorOnOffFeedback, GetValue(CommandArray[i + 2]));
            }
        }

        private void AddChannelStatusToList(int channelNo, StatusType type, int value)
        {
            ChannelStatusEntity channelStatus = new ChannelStatusEntity
            {
                ChannelNo = channelNo,
                Status = (int)type,
                Value = value.ToString()
            };
            ChannelStatusList.Add(channelStatus);

        }

        private void AddDeviceStatusToList(StatusType type, int value)
        {
            DeviceStatusEntity deviceStatus = new DeviceStatusEntity
            {
                StatusType = (int)type,
                Value = value
            };

            DeviceStatusList.Add(deviceStatus);
        }


        private void OnOffFeedbackCommandParse()
        {
            if (Device.DeviceType == DeviceType.SMART_SWITCH_6G)
            {
                if (GetChannelNoOfCommunicationProtocol() == 0)
                    GetDeviceStatusFromNumber3Bit(StatusType.OnOffFeedback);
                else
                    GetChannelStatus(StatusType.OnOffFeedback);
            }


            else if (Device.DeviceType == DeviceType.SMART_RAINBOW_12)
                GetDeviceStatusFromNumber3Bit(StatusType.OnOffFeedback);
        }

        private void DimmingFeedbackCommandParse()
        {
            if (Device.DeviceType == DeviceType.SMART_SWITCH_6G)
                GetChannelStatus(StatusType.DimmingFeedback);
            else if (Device.DeviceType == DeviceType.SMART_RAINBOW_12)
                GetDeviceStatusFromNumber3Bit(StatusType.DimmingFeedback);
        }

        private void DimmingFeedbackEnableDisableCommandParse()
        {
            if (Device.DeviceType == DeviceType.SMART_SWITCH_6G)
                GetChannelStatus(StatusType.DimmingEnableDisableFeedback);
        }

        private void LoadTypeSelectCommandParse()
        {
            if (Device.DeviceType == DeviceType.SMART_SWITCH_6G)
            {
                AddChannelValue(StatusType.LoadTypeSelectFeedback);
            }
            //GetChannelStatus(StatusType.LoadTypeSelectFeedback);
        }

        private void AddChannelValue(StatusType status)
        {
            Channel channel = Device.Channels.FirstOrDefault(x => x.ChannelNo == GetChannelNoOfCommunicationProtocol());
            if (channel != null)
            {
                channel.LoadType = (LoadType?)Get3RdBitValueOfCommunicationProtocol();

                _commandPerserService.UpdateChannel(channel);
            }
            else
                LogCommand(false, "Channel DeviceId = " + Device.DeviceId + " not found.");
        }

        private void ThermalShutDownCommandParse()
        {
            GetDeviceStatusFromNumber3Bit(StatusType.ThermalShutDownResponse);
        }

        private void SaveOrUpDateStatus()
        {
            SaveDeviceStatus(Device);

            SaveChannelStatus(Device);
        }

        private void SaveChannelStatus(Device device)
        {
            foreach (var channelValue in ChannelStatusList)
            {
                if (channelValue.ChannelNo > 0)
                {
                    SaveSingleChannelStatus(channelValue, device);
                    //UpdateAllChannelStatus(device, channelValue);
                }
            }
        }

        private void SaveSingleChannelStatus(ChannelStatusEntity channelValue, Device device)
        {
            var channel = Device.Channels.FirstOrDefault(x => x.ChannelNo == GetChannelNoOfCommunicationProtocol());

            if (channel != null)
            {
                AddOrUpdateChannelStatus(channel, channelValue);
            }
            else
            {
                LogCommand(false, "Channel (DeviceHash = " + _commandJson.DeviceUUId + " and Channel No ( " + channelValue.ChannelNo + " ) ) not found.");
            }
        }

        private void AddOrUpdateChannelStatus(Channel channel, ChannelStatusEntity channelValue)
        {
            var status = channel.ChannelStatuses.FirstOrDefault(x => x.Status == (ChannelStatusType) channelValue.Status);

            if (status != null)
            {
                UpdateChannelStatus(status, channelValue);
            }
            else
            {
                AddChannelStatus(channel, channelValue);
            }
        }

        private void AddChannelStatus(Channel channel, ChannelStatusEntity channelValue)
        {
            var status = new ChannelStatus
            {
                Channel = channel,
                Status =  (ChannelStatusType) channelValue.Status,
                ChannelNo = channelValue.ChannelNo,
                Value = Convert.ToInt32(channelValue.Value)
            };
            _commandPerserService.AddChannelStatus(status);
        }

        private static void UpdateChannelStatus(ChannelStatus status, ChannelStatusEntity channelValue)
        {
            status.Value = Convert.ToInt32(channelValue.Value);
            _commandPerserService.UpdateChannelStatus(status);
        }

        private void SaveDeviceStatus(Device entity)
        {
            foreach (var ds in DeviceStatusList)
            {
                DeviceStatus deviceStatus =  Device.DeviceStatus.FirstOrDefault(x => x.StatusType == (StatusType) ds.StatusType);

                if (deviceStatus != null)
                {
                    UpdateDevice(deviceStatus, ds);
                }
                else
                {
                    AddDeviceStatus(entity, ds);
                }
            }
        }

        private static void AddDeviceStatus(Device entity, DeviceStatusEntity ds)
        {
            var deviceStatus = new DeviceStatus
            {
                Device = entity,
                StatusType = (StatusType) ds.StatusType,
                Value = ds.Value
            };

            _commandPerserService.AddDeviceStatus(deviceStatus);
        }

        private void UpdateDevice(DeviceStatus deviceStatus, DeviceStatusEntity ds)
        {
            deviceStatus.StatusType = (StatusType) ds.StatusType;
            deviceStatus.Value = ds.Value;
            _commandPerserService.UpdateDeviceStatus(deviceStatus);
        }

        private int GetChannelNoOfCommunicationProtocol()
        {
            return Convert.ToInt32(GetValue(CommandArray[2]));
        }

        private int Get3RdBitValueOfCommunicationProtocol()
        {
            return Convert.ToInt32(GetValue(CommandArray[3]));
        }

        private int Get7ThBitValueOfCommunicationProtocol()
        {
            return Convert.ToInt32(GetValue(CommandArray[3]));
        }

        private void GetDeviceStatusFromNumber3Bit(StatusType status)
        {
            AddDeviceStatusToList(status, Get3RdBitValueOfCommunicationProtocol());

        }

        private void GetDeviceStatusFromNumber7Bit(StatusType status)
        {
            AddDeviceStatusToList(status, Get7ThBitValueOfCommunicationProtocol());
        }

        private void GetChannelStatus(StatusType status)
        {
            ChannelStatusEntity channelStatus = new ChannelStatusEntity
            {
                Status = (int)status,
                ChannelNo = GetChannelNoOfCommunicationProtocol(),
                Value = Get3RdBitValueOfCommunicationProtocol().ToString()
            };
            ChannelStatusList.Add(channelStatus);
        }

        public static T JsonDesrialized<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        #endregion
    }
}
