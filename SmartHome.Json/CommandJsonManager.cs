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
using SmartHome.MQTT.Client;
using SmartHome.Service;
using SmartHome.Service.Interfaces;

namespace SmartHome.Json
{
    public class CommandJsonManager
    {
        private static IUnitOfWorkAsync _unitOfWorkAsync;
        private static IDeviceService _deviceService;
        private static ICommandPerserService _commandPerserService;

        public CommandJsonManager(CommandJson commandJson)
        {
            IDataContextAsync context = new SmartHomeDataContext();
            _unitOfWorkAsync = new UnitOfWork(context);
            _commandPerserService = new CommandParserService(_unitOfWorkAsync);

            var values = commandJson.Command.Replace("[{", string.Empty).Replace("}]", string.Empty).Split(',');

            byte[] commandArray = new byte[values.Length];

            for (int index = 0; index < values.Length; index++)
            {
                commandArray[index] = Convert.ToByte(values[index]);
            }
            CommandParser fp = new CommandParser(commandArray);

            Device entity = _commandPerserService.FindDevice(Convert.ToInt32((commandJson.DeviceID)));
            if (entity != null)
            {
                DeviceStatus deviceStatus = _commandPerserService.FindDeviceStatus(entity.DeviceId, entity.Id);

                if (deviceStatus != null)
                {
                    UpdateDeviceThermalShutDownStatus(deviceStatus, fp);
                }
                else
                {
                    deviceStatus = new DeviceStatus();
                    deviceStatus.DId = entity.Id;
                    UpdateDeviceThermalShutDownStatus(deviceStatus, fp);
                }

            }
            else
            {

                entity = new Device();
                entity.DeviceHash = commandJson.DeviceUUID;
                entity.Mac = commandJson.MacID;
                entity.Id = Convert.ToInt32((commandJson.DeviceID));
                entity.DeviceVersion = commandJson.DeviceVersion;

                entity = _commandPerserService.AdddDevice(entity);

                DeviceStatus deviceStatus = new DeviceStatus();
                deviceStatus.DId = entity.Id;
                AddDeviceThermalShutDownStatus(deviceStatus, fp);
            }

            Channel channel = _commandPerserService.FindChannel(Convert.ToInt32((commandJson.DeviceID)));
            foreach (var channelValue in fp.ChannelValueList)
            {
                //update channelstatus

            }
        }

        private  void UpdateDeviceThermalShutDownStatus(DeviceStatus deviceStatus, CommandParser fp)
        {
            deviceStatus.StatusType = (int)StatusType.SmartSwitchThermalShutdown;
            deviceStatus.Status = fp.ThermalShutDownValue;
            _commandPerserService.UpdateDeviceStatus(deviceStatus);

            deviceStatus.StatusType = (int)StatusType.SmartSwitchIndicator;
            deviceStatus.Status = fp.ChannelValueList[0].IndicatorValue;
            _commandPerserService.UpdateDeviceStatus(deviceStatus);
        }

        private  void AddDeviceThermalShutDownStatus(DeviceStatus deviceStatus, CommandParser fp)
        {
            deviceStatus.StatusType = (int)StatusType.SmartSwitchThermalShutdown;
            deviceStatus.Status = fp.ThermalShutDownValue;
            _commandPerserService.AddDeviceStatus(deviceStatus);

            deviceStatus.StatusType = (int)StatusType.SmartSwitchIndicator;
            deviceStatus.Status = fp.ChannelValueList[0].IndicatorValue;
            _commandPerserService.AddDeviceStatus(deviceStatus);
        }
    }
}
