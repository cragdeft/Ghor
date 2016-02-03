using Repository.Pattern.Repositories;
using SmartHome.Entity;
using SmartHome.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Repository.Repositories
{

    public static class DeviceRepository
    {

        public static IEnumerable<DeviceInfoEntity> GetsAllDevice(this IRepositoryAsync<Device> repository)
        {
            int parentSequence = 0;
            List<DeviceInfoEntity> dInfoEntity = new List<DeviceInfoEntity>();
            var device = repository.Query().Include(x => x.DeviceStatus).Include(x => x.Channels.Select(y => y.ChannelStatuses)).Select().ToList();

            foreach (Device nextDeviceInfo in device)
            {
                //DId,DeviceName,DeviceHash,DType
                DeviceInfoEntity deviceInfo = new DeviceInfoEntity();
                deviceInfo.DisplayName = "DId--" + nextDeviceInfo.DId + " DeviceName--" + nextDeviceInfo.DeviceName + " DeviceHash--" + nextDeviceInfo.DeviceHash + " DType--" + nextDeviceInfo.DType.ToString();
                deviceInfo.ParentId = 0;
                deviceInfo.SequenceId = dInfoEntity.Count() + 1;
                parentSequence = deviceInfo.SequenceId;
                dInfoEntity.Add(deviceInfo);

                foreach (DeviceStatus nextDStatus in nextDeviceInfo.DeviceStatus)
                {
                    deviceInfo = new DeviceInfoEntity();
                    deviceInfo.DisplayName = "StatusType--" + nextDStatus.StatusType.ToString() + " Value--" + nextDStatus.Value.ToString();
                    deviceInfo.SequenceId = dInfoEntity.Count() + 1;
                    deviceInfo.ParentId = parentSequence;
                    dInfoEntity.Add(deviceInfo);

                }

                foreach (Channel nextChannelInfo in nextDeviceInfo.Channels)
                {
                    DeviceInfoEntity deviceCInfo = new DeviceInfoEntity();
                    deviceCInfo.DisplayName = " ChannelNo--" + nextChannelInfo.ChannelNo.ToString() + " LoadName--" + nextChannelInfo.LoadName + " LoadType--" + nextChannelInfo.LoadType.ToString();
                    deviceCInfo.SequenceId = dInfoEntity.Count() + 1;
                    deviceCInfo.ParentId = parentSequence;
                    dInfoEntity.Add(deviceCInfo);

                    foreach (ChannelStatus nextCStatus in nextChannelInfo.ChannelStatuses)
                    {
                        deviceInfo = new DeviceInfoEntity();
                        deviceInfo.DisplayName = " Status--" + nextCStatus.Status.ToString() + " Value--" + nextCStatus.Value.ToString();//nextChannelInfo.ChannelNo.ToString();
                        deviceInfo.SequenceId = dInfoEntity.Count() + 1;
                        deviceInfo.ParentId = deviceCInfo.SequenceId;
                        dInfoEntity.Add(deviceInfo);
                    }
                }


            }

            return dInfoEntity;
        }
    }
}
