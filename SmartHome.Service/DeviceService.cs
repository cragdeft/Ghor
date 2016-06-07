using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Service.Pattern;
using SmartHome.Entity;
using SmartHome.Model.Models;
using SmartHome.Repository.Repositories;
using SmartHome.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{
    public class DeviceService : Service<SmartDevice>, IDeviceService
    {
        private readonly IRepositoryAsync<SmartDevice> _repository;

        public DeviceService(IRepositoryAsync<SmartDevice> repository) : base(repository)
        {
            _repository = repository;
        }

        #region AddOrUpdateGraphRange


        public IEnumerable<SmartDevice> AddOrUpdateGraphRange(IEnumerable<SmartDevice> model)
        {
            List<SmartDevice> deviceModel = new List<SmartDevice>();
            deviceModel = FillDeviceInformations(model, deviceModel);
            base.InsertOrUpdateGraphRange(deviceModel);
            return deviceModel;
        }


        public List<SmartDevice> FillDeviceInformations(IEnumerable<SmartDevice> model, List<SmartDevice> deviceModel)
        {
            foreach (var item in model)
            {
                //check already exist or not.
                IEnumerable<SmartDevice> temp = IsDeviceExists(item.AppsDeviceId, item.DeviceHash);
                if (temp.Count() == 0)
                {
                    //new item
                    deviceModel.Add(item);
                    continue;
                }
                else
                {
                    foreach (var existingItem in temp.ToList())
                    {
                        //modify version                    
                        FillExistingDeviceInfo(item, existingItem);

                        if (item.DeviceStatus != null && item.DeviceStatus.Count > 0)
                        {
                            AddOrEditExistingDeviceStatus(item, existingItem);
                            AddOrEditExistingChannels(item, existingItem);
                        }

                    }
                }
            }

            return deviceModel;
        }


        private void AddOrEditExistingChannels(SmartDevice item, SmartDevice existingItem)
        {
            foreach (var nextChannel in ((SmartSwitch)item).Channels)
            {
                var tempExistingChannel = ((SmartSwitch)existingItem).Channels.Where(p => p.AppsChannelId == nextChannel.AppsChannelId).FirstOrDefault();
                if (tempExistingChannel != null)
                {
                    //modify
                    FillExistingChannelInfo(nextChannel, tempExistingChannel);
                }
                else
                {
                    //add
                    ((SmartSwitch)existingItem).Channels.Add(nextChannel);
                }
            }
        }

        private void FillExistingChannelInfo(Channel nextChannel, Channel tempExistingChannel)
        {
            tempExistingChannel.ObjectState = ObjectState.Modified;
            tempExistingChannel.AppsChannelId = nextChannel.AppsChannelId;
            tempExistingChannel.AppsDeviceTableId = nextChannel.AppsDeviceTableId;
            tempExistingChannel.ChannelNo = nextChannel.ChannelNo;
            tempExistingChannel.LoadName = nextChannel.LoadName;
            tempExistingChannel.LoadType = nextChannel.LoadType;
            tempExistingChannel.AuditField = new AuditFields();

            AddOrEditExistingChannelStatus(nextChannel, tempExistingChannel);

        }

        private void AddOrEditExistingChannelStatus(Channel nextChannel, Channel tempExistingChannel)
        {

            //channel status
            foreach (var nextChannelStatus in nextChannel.ChannelStatuses)
            {
                var tempExistingChannelStatus = tempExistingChannel.ChannelStatuses.Where(p => p.AppsChannelStatusId == nextChannelStatus.AppsChannelStatusId).FirstOrDefault();
                if (tempExistingChannelStatus != null)
                {
                    //modify
                    FillExistingChannelStatusInfo(nextChannelStatus, tempExistingChannelStatus);
                }
                else
                {
                    //add
                    tempExistingChannel.ChannelStatuses.Add(nextChannelStatus);
                }
            }
        }

        private void FillExistingChannelStatusInfo(ChannelStatus nextChannelStatus, ChannelStatus tempExistingChannelStatus)
        {
            tempExistingChannelStatus.ObjectState = ObjectState.Modified;
            tempExistingChannelStatus.AppsChannelStatusId = nextChannelStatus.AppsChannelStatusId;
            tempExistingChannelStatus.AppsChannelId = nextChannelStatus.AppsChannelId;
            //tempExistingChannelStatus.DId = nextChannelStatus.DId;
            //tempExistingChannelStatus.ChannelNo = nextChannelStatus.ChannelNo;
            tempExistingChannelStatus.StatusType = nextChannelStatus.StatusType;
            tempExistingChannelStatus.StatusValue = nextChannelStatus.StatusValue;
            tempExistingChannelStatus.AuditField = new AuditFields();
        }

        private void AddOrEditExistingDeviceStatus(SmartDevice item, SmartDevice existingItem)
        {
            foreach (var nextDStatus in item.DeviceStatus)
            {
                var tempExistingDStatus = existingItem.DeviceStatus.Where(p => p.AppsDeviceStatusId == nextDStatus.AppsDeviceStatusId).FirstOrDefault();
                if (tempExistingDStatus != null)
                {
                    //modify
                    FillExistingVDetailInfo(nextDStatus, tempExistingDStatus);
                }
                else
                {
                    //add
                    existingItem.DeviceStatus.Add(nextDStatus);
                }
            }
        }

        private void FillExistingVDetailInfo(DeviceStatus nextDStatus, DeviceStatus tempExistingDStatus)
        {
            tempExistingDStatus.ObjectState = ObjectState.Modified;
            tempExistingDStatus.AppsDeviceStatusId = nextDStatus.AppsDeviceStatusId;
            tempExistingDStatus.AppsDeviceId = nextDStatus.AppsDeviceId;
            tempExistingDStatus.StatusType = nextDStatus.StatusType;
            tempExistingDStatus.Status = nextDStatus.Status;
            tempExistingDStatus.AuditField = new AuditFields();
        }

        private void FillExistingDeviceInfo(SmartDevice item, SmartDevice existingItem)
        {
            existingItem.ObjectState = ObjectState.Modified;

            existingItem.AppsDeviceId = item.AppsDeviceId;
            existingItem.AppsBleId = item.AppsBleId;
            existingItem.DeviceName = item.DeviceName;
            existingItem.DeviceHash = item.DeviceHash;
            existingItem.FirmwareVersion = item.FirmwareVersion;
            existingItem.IsDeleted = item.IsDeleted;
            existingItem.Watt = item.Watt;
            //existingItem.Mac = item.Mac;
            existingItem.DeviceType = item.DeviceType;
            existingItem.AuditField = new AuditFields();

        }

        private IEnumerable<SmartDevice> IsDeviceExists(int key, string deviceHash)
        {
            return _repository.Query(e => e.AppsDeviceId == key && e.DeviceHash == deviceHash).Include(x => x.DeviceStatus).Include(x => ((SmartSwitch)x).Channels.Select(y => y.ChannelStatuses)).Select();
        }


        #endregion

        public IEnumerable<DeviceInfoEntity> GetsDeviceAllInfo()
        {
            // add business logic here
            // return _repository.GetsAllDevice();

            int parentSequence = 0;
            List<DeviceInfoEntity> dInfoEntity = new List<DeviceInfoEntity>();
            var device = _repository.Query().Include(x => x.DeviceStatus).Include(x => ((SmartSwitch)x).Channels.Select(y => y.ChannelStatuses)).Select().ToList();

            foreach (SmartDevice nextDeviceInfo in device)
            {
                //DId,DeviceName,DeviceHash,DType
                DeviceInfoEntity deviceInfo = new DeviceInfoEntity();
                deviceInfo.DisplayName = "DId--" + nextDeviceInfo.AppsBleId + ", DeviceName--" + nextDeviceInfo.DeviceName + ", DeviceHash--" + nextDeviceInfo.DeviceHash + ", DType--" + nextDeviceInfo.DeviceType.ToString();
                deviceInfo.ParentId = 0;
                deviceInfo.SequenceId = dInfoEntity.Count() + 1;
                parentSequence = deviceInfo.SequenceId;
                dInfoEntity.Add(deviceInfo);

                foreach (DeviceStatus nextDStatus in nextDeviceInfo.DeviceStatus)
                {
                    deviceInfo = new DeviceInfoEntity();
                    deviceInfo.DisplayName = "StatusType--" + nextDStatus.StatusType.ToString() + ", Value--" + nextDStatus.Value.ToString();
                    deviceInfo.SequenceId = dInfoEntity.Count() + 1;
                    deviceInfo.ParentId = parentSequence;
                    dInfoEntity.Add(deviceInfo);

                }

                foreach (Channel nextChannelInfo in ((SmartSwitch)nextDeviceInfo).Channels)
                {
                    DeviceInfoEntity deviceCInfo = new DeviceInfoEntity();
                    deviceCInfo.DisplayName = " ChannelNo--" + nextChannelInfo.ChannelNo.ToString() + ", LoadName--" + nextChannelInfo.LoadName + ", LoadType--" + nextChannelInfo.LoadType.ToString();
                    deviceCInfo.SequenceId = dInfoEntity.Count() + 1;
                    deviceCInfo.ParentId = parentSequence;
                    dInfoEntity.Add(deviceCInfo);

                    foreach (ChannelStatus nextCStatus in nextChannelInfo.ChannelStatuses)
                    {
                        deviceInfo = new DeviceInfoEntity();
                        deviceInfo.DisplayName = " Status--" + nextCStatus.StatusType.ToString() + ", Value--" + nextCStatus.StatusValue.ToString();//nextChannelInfo.ChannelNo.ToString();
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
