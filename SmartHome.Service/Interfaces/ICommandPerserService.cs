﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Pattern;
using SmartHome.Entity;
using SmartHome.Model.Models;

namespace SmartHome.Service.Interfaces
{
    public interface ICommandPerserService
    {
        DeviceStatus UpdateDeviceStatus(DeviceStatus deviceStatus);

        DeviceStatus AddDeviceStatus(DeviceStatus deviceStatus);

        bool UpdateChannelStatus(ChannelStatus channelStatus);

        ChannelStatus AddChannelStatus(ChannelStatus channelStatus);

        Device FindDevice(int id);

        Device AdddDevice(Device device);

        DeviceStatus FindDeviceStatus(int deviceid, int Id);

        ChannelStatus FindChannelStatus(int deviceid, int Id);
        Channel FindChannel(int id);
    }
}