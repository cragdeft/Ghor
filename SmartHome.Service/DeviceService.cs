using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Service.Pattern;
using SmartHome.Entity;
using SmartHome.Model.Models;
using SmartHome.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{
    public class DeviceService : Service<Device>, IDeviceService
    {
        private readonly IRepositoryAsync<Device> _repository;

        public DeviceService(IRepositoryAsync<Device> repository) : base(repository)
        {
            _repository = repository;
        }

        public IEnumerable<Device> AddOrUpdateGraphRange(IEnumerable<Device> model)
        {
           
            List<Device> deviceModel = new List<Device>();
            deviceModel = FillDeviceInformations(model, deviceModel);
            base.InsertOrUpdateGraphRange(deviceModel);
            return deviceModel;
        }

        private List<Device> FillDeviceInformations(IEnumerable<Device> model, List<Device> deviceModel)
        {
            foreach (var item in model)
            {
                //check already exist or not.
                IEnumerable<Device> temp = IsDeviceExists(item.Id, item.DeviceHash);
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


        private void AddOrEditExistingChannels(Device item, Device existingItem)
        {
            foreach (var nextChannel in item.Channels)
            {
                var tempExistingChannel = existingItem.Channels.Where(p => p.Id == nextChannel.Id).FirstOrDefault();
                if (tempExistingChannel != null)
                {
                    //modify
                    FillExistingChannelInfo(nextChannel, tempExistingChannel);
                }
                else
                {
                    //add
                    existingItem.Channels.Add(nextChannel);
                }
            }
        }

        private void FillExistingChannelInfo(Channel nextChannel, Channel tempExistingChannel)
        {
            tempExistingChannel.ObjectState = ObjectState.Modified;
            tempExistingChannel.Id = nextChannel.Id;
            tempExistingChannel.DId = nextChannel.DId;
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
                var tempExistingChannelStatus = tempExistingChannel.ChannelStatuses.Where(p => p.Id == nextChannelStatus.Id).FirstOrDefault();
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
            tempExistingChannelStatus.Id = nextChannelStatus.Id;
            tempExistingChannelStatus.CId = nextChannelStatus.CId;
            tempExistingChannelStatus.DId = nextChannelStatus.DId;
            tempExistingChannelStatus.ChannelNo = nextChannelStatus.ChannelNo;
            tempExistingChannelStatus.Status = nextChannelStatus.Status;
            tempExistingChannelStatus.Value = nextChannelStatus.Value;
            tempExistingChannelStatus.AuditField = new AuditFields();
        }

        private void AddOrEditExistingDeviceStatus(Device item, Device existingItem)
        {
            foreach (var nextDStatus in item.DeviceStatus)
            {
                var tempExistingDStatus = existingItem.DeviceStatus.Where(p => p.Id == nextDStatus.Id).FirstOrDefault();
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
            tempExistingDStatus.Id = nextDStatus.Id;
            tempExistingDStatus.DId = nextDStatus.DId;
            tempExistingDStatus.StatusType = nextDStatus.StatusType;
            tempExistingDStatus.Status = nextDStatus.Status;
            tempExistingDStatus.AuditField = new AuditFields();
        }

        private void FillExistingDeviceInfo(Device item, Device existingItem)
        {
            existingItem.ObjectState = ObjectState.Modified;

            existingItem.Id = item.Id;
            existingItem.DId = item.DId;
            existingItem.DeviceName = item.DeviceName;
            existingItem.DeviceHash = item.DeviceHash;
            existingItem.DeviceVersion = item.DeviceVersion;
            existingItem.IsDeleted = item.IsDeleted;
            existingItem.Watt = item.Watt;
            existingItem.Mac = item.Mac;
            existingItem.DeviceType = item.DeviceType;
            existingItem.AuditField = new AuditFields();

        }

        private IEnumerable<Device> IsDeviceExists(int key, string deviceHash)
        {
            return base.Query(e => e.Id == key && e.DeviceHash == deviceHash).Include(x => x.DeviceStatus).Include(x => x.Channels.Select(y=>y.ChannelStatuses)).Select();
        }
    }
}
