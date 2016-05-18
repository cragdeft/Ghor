﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private IUnitOfWorkAsync _unitOfWorkAsync;
        private ICommandPerserService _commandPerserService;
        private CommandJsonEntity _commandJson { get; set; }
        public SmartDevice SmartDevice { get; set; }

        private string[] CommandArray { get; set; }
        #endregion

        #region Public
        public List<DeviceStatusEntity> DeviceStatusList { get; set; }
        public List<ChannelStatusEntity> ChannelStatusList { get; set; }
        public int Length { get; set; }
        public byte Initiator { get; set; }
        public CommandId CommandId { get; set; }
        public LoadType LoadType { get; set; }

        #endregion

        #endregion

        #region Constructor

     

        public CommandJsonManager()
        {
        }


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
            //CommandArray = _commandJson.Command.Skip(2).ToString().Replace("[", string.Empty).Replace("]", string.Empty).Split(',');
            CommandArray = _commandJson.Command.ToString().Replace("[", string.Empty).Replace("]", string.Empty).Split(',').Skip(2).ToArray();
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

        #region Public Methods
        public void LogCommand(bool isProcessed, string reason)
        {
            _commandJson.IsProcessed = isProcessed;
            _commandJson.ProcessFailReason = reason;
            _commandPerserService.LogCommand(_commandJson);
        }
        public void Parse()
        {
            SmartDevice = FindDevice();

            if (SmartDevice == null)
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
                    case CommandId.SmartRainbowPower:
                        SmartRainbowPowerCommandParse();
                        break;
                    case CommandId.SmartRainbowRgbw:
                        SmartRainbowRgbwSetCommandParse();
                        break;
                    default:
                        return;
                }

                SaveOrUpDateStatus();
            }
        }

        public virtual SmartDevice FindDevice()
        {
            return _commandPerserService.FindDevice(_commandJson.DeviceUUId);
        }

        public static T JsonDesrialized<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public CommandJsonEntity ConvertToCommandJsonObject(string jsonString, CommandType commandType)
        {
            CommandJsonEntity jsonObject = CommandJsonManager.JsonDesrialized<CommandJsonEntity>(jsonString);
            jsonObject.CommandType = commandType;
            return jsonObject;
        }

        public void CommandLog(CommandJsonEntity jsonObject)
        {
            CommandJsonManager commandJsonManager = new CommandJsonManager(jsonObject);
            commandJsonManager.LogCommand(true, "");
        }

        #endregion

        #region Private Methods
        private CommandId GetCommandId()
        {
            _commandJson.CommandId = (CommandId)GetValue(CommandArray[1]);
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

        private void IterateByte(BitArray cds0, ref bool[] directionBoolArray, ref bool[] versionBoolArray)
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

        public void CurrentLoadStatusParse()
        {
            if (SmartDevice.DeviceType != DeviceType.SmartSwitch6g) return;

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

        private void AddDeviceStatusToList(StatusType type, string value)
        {
            DeviceStatusEntity deviceStatus = new DeviceStatusEntity
            {
                StatusType = (int)type,
                Value = value
            };

            DeviceStatusList.Add(deviceStatus);
        }


        public void OnOffFeedbackCommandParse()
        {
            if (SmartDevice.DeviceType == DeviceType.SmartSwitch6g)
            {
                if (GetChannelNoOfCommunicationProtocol() == 0)
                    GetDeviceStatusFromNumber3Bit(StatusType.OnOffFeedback);
                else
                    GetChannelStatus(StatusType.OnOffFeedback);
            }


            else if (SmartDevice.DeviceType == DeviceType.SmartRainbow12)
                GetDeviceStatusFromNumber3Bit(StatusType.OnOffFeedback);
        }

        public void DimmingFeedbackCommandParse()
        {
            if (SmartDevice.DeviceType == DeviceType.SmartSwitch6g)
                GetChannelStatus(StatusType.DimmingFeedback);
            else if (SmartDevice.DeviceType == DeviceType.SmartRainbow12)
                GetDeviceStatusFromNumber3Bit(StatusType.DimmingFeedback);
        }

        public void DimmingFeedbackEnableDisableCommandParse()
        {
            if (SmartDevice.DeviceType == DeviceType.SmartSwitch6g)
                GetChannelStatus(StatusType.DimmingEnableDisableFeedback);
        }

        public void LoadTypeSelectCommandParse()
        {
            if (SmartDevice.DeviceType == DeviceType.SmartSwitch6g)
            {
                AddChannelValue(StatusType.LoadTypeSelectFeedback);
            }
            //GetChannelStatus(StatusType.LoadTypeSelectFeedback);
        }

        private void SmartRainbowRgbwSetCommandParse()
        {
            if (SmartDevice.DeviceType == DeviceType.SmartRainbow12)
            {
                AddDeviceStatusToList(StatusType.RgbwStatus, GetRgbwStatus());
            }
        }

        private string GetRgbwStatus()
        {
            string deviceStatusValue = null;
            for (int i = 2; i < 7; i++)
                deviceStatusValue += GetValue(CommandArray[i]).ToString() + "|";
            deviceStatusValue = deviceStatusValue?.Remove(deviceStatusValue.Length - 1);
            return deviceStatusValue;

        }

        private void SmartRainbowPowerCommandParse()
        {
            if (SmartDevice.DeviceType == DeviceType.SmartRainbow12)
                GetDeviceStatusFromNumber2Bit(StatusType.RgbwStatus);
        }

        private void AddChannelValue(StatusType status)
        {
            Channel channel = ((SmartSwitch)SmartDevice).Channels.FirstOrDefault(x => x.ChannelNo == GetChannelNoOfCommunicationProtocol());
            if (channel != null)
            {
                channel.LoadType = (LoadType)Get3RdBitValueOfCommunicationProtocol();
                LoadType = (LoadType)channel.LoadType;
                _commandPerserService.UpdateChannel(channel);
            }
            else
                LogCommand(false, "Channel DeviceId = " + SmartDevice.DeviceId + " not found.");
        }

        private void ThermalShutDownCommandParse()
        {
            GetDeviceStatusFromNumber3Bit(StatusType.ThermalShutDownResponse);
        }

        public void SaveOrUpDateStatus()
        {
            SaveDeviceStatus(SmartDevice);

            SaveChannelStatus(SmartDevice);
        }

        private void SaveChannelStatus(SmartDevice device)
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

        private void SaveSingleChannelStatus(ChannelStatusEntity channelValue, SmartDevice device)
        {
            var channel = ((SmartSwitch)SmartDevice).Channels.FirstOrDefault(x => x.ChannelNo == GetChannelNoOfCommunicationProtocol());

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
            var status = channel.ChannelStatuses.FirstOrDefault(x => x.Status == (ChannelStatusType)channelValue.Status);

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
                Status = (ChannelStatusType)channelValue.Status,
                //ChannelNo = channelValue.ChannelNo,
                Value = Convert.ToInt32(channelValue.Value)
            };
            _commandPerserService.AddChannelStatus(status);
        }

        private void UpdateChannelStatus(ChannelStatus status, ChannelStatusEntity channelValue)
        {
            status.Value = Convert.ToInt32(channelValue.Value);
            _commandPerserService.UpdateChannelStatus(status);
        }

        private void SaveDeviceStatus(SmartDevice entity)
        {
            foreach (var ds in DeviceStatusList)
            {
                DeviceStatus deviceStatus = SmartDevice.DeviceStatus.FirstOrDefault(x => x.StatusType == (StatusType)ds.StatusType);

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

        private void AddDeviceStatus(SmartDevice entity, DeviceStatusEntity ds)
        {
            var deviceStatus = new DeviceStatus
            {
                SmartDevice = entity,
                StatusType = (StatusType)ds.StatusType,
                Value = ds.Value
            };

            _commandPerserService.AddDeviceStatus(deviceStatus);
        }

        private void UpdateDevice(DeviceStatus deviceStatus, DeviceStatusEntity ds)
        {
            deviceStatus.StatusType = (StatusType)ds.StatusType;
            deviceStatus.Value = ds.Value;
            _commandPerserService.UpdateDeviceStatus(deviceStatus);
        }

        private int GetChannelNoOfCommunicationProtocol()
        {
            return Convert.ToInt32(GetValue(CommandArray[2]));
        }

        private int Get2NdBitValueOfCommunicationProtocol()
        {
            return Convert.ToInt32(GetValue(CommandArray[2]));
        }
        private int Get3RdBitValueOfCommunicationProtocol()
        {
            return Convert.ToInt32(GetValue(CommandArray[3]));
        }

        private int Get7ThBitValueOfCommunicationProtocol()
        {
            return Convert.ToInt32(GetValue(CommandArray[7]));
        }

        private void GetDeviceStatusFromNumber3Bit(StatusType status)
        {
            AddDeviceStatusToList(status, Get3RdBitValueOfCommunicationProtocol().ToString());
        }

        private void GetDeviceStatusFromNumber2Bit(StatusType status)
        {
            AddDeviceStatusToList(status, Get2NdBitValueOfCommunicationProtocol().ToString());
        }

        private void GetDeviceStatusFromNumber7Bit(StatusType status)
        {
            AddDeviceStatusToList(status, Get7ThBitValueOfCommunicationProtocol().ToString());
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
        #endregion

        #endregion
    }
}
