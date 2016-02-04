﻿using System;
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
        private static ICommandPerserService _commandPerserService;
        private CommandJsonEntity _commandJson { get; set; }
        private Device Device { get; set; }
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
        public CommandJsonManager(CommandJsonEntity commandJson)
        {
            InitializeParameters(commandJson);

            InitializeList();
        }

        private void InitializeParameters(CommandJsonEntity commandJson)
        {
            IDataContextAsync context = new SmartHomeDataContext();
            _unitOfWorkAsync = new UnitOfWork(context);
            _commandPerserService = new CommandParserService(_unitOfWorkAsync, commandJson.EmailAddress);
            _commandJson = commandJson;
            CommandArray = _commandJson.Command.Replace("[", string.Empty).Replace("]", string.Empty).Split(',');
            Length = CommandArray.Length;
        }

        private void InitializeList()
        {
            DeviceStatusList = new List<DeviceStatusEntity>();
            ChannelStatusList = new List<ChannelStatusEntity>();
        }

        #endregion

        #region Methods

        public void LogCommand(bool isProcessed,string reason)
        {
            _commandJson.IsProcessed = isProcessed;
            _commandJson.ProcessFailReason = reason;
            _commandPerserService.LogCommand(_commandJson);
        }
        public void Parse()
        {
            Device = _commandPerserService.FindDevice(_commandJson.DeviceUUID);

            if (Device == null)
            {
                LogCommand(false,"Device (DeviceHash = "+_commandJson.DeviceUUID+" ) not found.");
                return;
            }
            else
            {
                //Initiator = GetInitiator();

                CommandId = GetCommandId();
                

                switch (CommandId)
                {
                    case CommandId.DevicePingRequest:
                        LogCommand(true,"");
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
                        throw new ArgumentOutOfRangeException();
                }

                SaveOrUpDateStatus();
            }

        }

        private CommandId GetCommandId()
        {
            return (CommandId)Enum.ToObject(typeof(CommandId), GetValue(CommandArray[1]));
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
            if (Device.DeviceType != DeviceType.SMART_SWITCH_6G) return;
            GetDeviceStatus(StatusType.SmartSwitchThermalShutdown);

            ParseCurrentLoadStatusCommand(CommandArray.Length > 30 ? 28 : 8);
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
                    //AddDeviceStatusToList(StatusType.DimmingFeedback, GetValue(CommandArray[i]));
                }
                else if (i % 7 == 0)
                {
                    AddChannelStatusToList(GetValue(CommandArray[i - 3]), StatusType.IndicatorOnOffFeedback, GetValue(CommandArray[i]));
                    if(GetValue(CommandArray[i - 3]) == 0)
                    {
                        AddDeviceStatusToList(StatusType.SmartSwitchIndicator, GetValue(CommandArray[i]));
                    }
                }
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
                DId = _commandJson.DeviceID.ToString(),
                StatusType = (int)type,
                Value = value
            };

            DeviceStatusList.Add(deviceStatus);
        }


        private void OnOffFeedbackCommandParse()
        {
            if (Device.DeviceType == DeviceType.SMART_SWITCH_6G)
                GetChannelStatus(StatusType.OnOffFeedback);

            else if (Device.DeviceType == DeviceType.SMART_RAINBOW_12)
                GetDeviceStatus(StatusType.OnOffFeedback);
        }

        private void DimmingFeedbackCommandParse()
        {
            if (Device.DeviceType == DeviceType.SMART_SWITCH_6G)
                GetChannelStatus(StatusType.DimmingFeedback);
            else if (Device.DeviceType == DeviceType.SMART_RAINBOW_12)
                GetDeviceStatus(StatusType.DimmingFeedback);
        }

        private void DimmingFeedbackEnableDisableCommandParse()
        {
            if (Device.DeviceType == DeviceType.SMART_SWITCH_6G)
                GetChannelStatus(StatusType.DimmingEnableDisableFeedback);
        }

        private void  LoadTypeSelectCommandParse()
        {
            if (Device.DeviceType == DeviceType.SMART_SWITCH_6G)
            {
                AddChannelValue(StatusType.LoadTypeSelectFeedback);
            }
            //GetChannelStatus(StatusType.LoadTypeSelectFeedback);
        }

        private void AddChannelValue(StatusType status)
        {
            Channel channel = _commandPerserService.FindChannel(Device.DeviceId, GetChannelNoOfCommunicationProtocol());
            channel.LoadType = (LoadType?) GetValueOfCommunicationProtocol();
            channel.LoadName = "";

            _commandPerserService.UpdateChannel(channel);
        }

        private void ThermalShutDownCommandParse()
        {
            GetDeviceStatus(StatusType.ThermalShutDownResponse);
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
                if (channelValue.ChannelNo == 0)
                {
                    UpdateAllChannelStatus(device, channelValue);
                }
                else
                {
                    SaveSingleChannelStatus(channelValue, device);
                }
            }
        }

        private static void UpdateAllChannelStatus(Device device, ChannelStatusEntity channelValue)
        {
            List<ChannelStatus> list = _commandPerserService.GetAllChannelStatus(device.Id);
            foreach (ChannelStatus channelStatus in list)
            {
                UpdateChannelStatus(channelStatus, channelValue);
            }
        }

        private void SaveSingleChannelStatus(ChannelStatusEntity channelValue, Device device)
        {
            var channel = _commandPerserService.FindChannel(device.Id, channelValue.ChannelNo);

            if (channel != null)
            {
                AddOrUpdateChannelStatus(channel, channelValue);
            }
            else
            {
                LogCommand(false, "Channel (DeviceHash = " + _commandJson.DeviceUUID + " and Channel No ( "+ channelValue.ChannelNo+" ) ) not found.");
            }
        }

        private void AddOrUpdateChannelStatus(Channel channel, ChannelStatusEntity channelValue)
        {
            var status = _commandPerserService.FindChannelStatus(channel.ChannelId, channelValue.ChannelNo);

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
                DId = _commandJson.DeviceID,
                CId = channel.ChannelId,
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
                DeviceStatus deviceStatus = _commandPerserService.FindDeviceStatus(entity.DeviceId, ds.StatusType);

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
            var deviceStatus = new DeviceStatus();
            deviceStatus.DId = entity.DeviceId;
            deviceStatus.StatusType = (StatusType) ds.StatusType;
            deviceStatus.Status = ds.Value;

            _commandPerserService.AddDeviceStatus(deviceStatus);
        }

        private static void UpdateDevice(DeviceStatus deviceStatus, DeviceStatusEntity ds)
        {
            deviceStatus.Status = ds.Value;
            _commandPerserService.UpdateDeviceStatus(deviceStatus);
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
            ChannelStatusEntity channelStatus = new ChannelStatusEntity
            {
                Status = (int)status,
                ChannelNo = GetChannelNoOfCommunicationProtocol(),
                Value = GetValueOfCommunicationProtocol().ToString()
            };
            ChannelStatusList.Add(channelStatus);
        }

        

        #endregion
    }
}
