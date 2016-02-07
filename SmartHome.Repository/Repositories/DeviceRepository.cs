using Repository.Pattern.Infrastructure;
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

        //public static IEnumerable<DeviceInfoEntity> GetsAllDevice(this IRepositoryAsync<Device> repository)
        //{
        //    int parentSequence = 0;
        //    List<DeviceInfoEntity> dInfoEntity = new List<DeviceInfoEntity>();
        //    var device = repository.Query().Include(x => x.DeviceStatus).Include(x => x.Channels.Select(y => y.ChannelStatuses)).Select().ToList();

        //    foreach (Device nextDeviceInfo in device)
        //    {
        //        //DId,DeviceName,DeviceHash,DType
        //        DeviceInfoEntity deviceInfo = new DeviceInfoEntity();
        //        deviceInfo.DisplayName = "DId--" + nextDeviceInfo.DId + ", DeviceName--" + nextDeviceInfo.DeviceName + ", DeviceHash--" + nextDeviceInfo.DeviceHash + ", DType--" + nextDeviceInfo.DeviceType.ToString();
        //        deviceInfo.ParentId = 0;
        //        deviceInfo.SequenceId = dInfoEntity.Count() + 1;
        //        parentSequence = deviceInfo.SequenceId;
        //        dInfoEntity.Add(deviceInfo);

        //        foreach (DeviceStatus nextDStatus in nextDeviceInfo.DeviceStatus)
        //        {
        //            deviceInfo = new DeviceInfoEntity();
        //            deviceInfo.DisplayName = "StatusType--" + nextDStatus.StatusType.ToString() + ", Value--" + nextDStatus.Value.ToString();
        //            deviceInfo.SequenceId = dInfoEntity.Count() + 1;
        //            deviceInfo.ParentId = parentSequence;
        //            dInfoEntity.Add(deviceInfo);

        //        }

        //        foreach (Channel nextChannelInfo in nextDeviceInfo.Channels)
        //        {
        //            DeviceInfoEntity deviceCInfo = new DeviceInfoEntity();
        //            deviceCInfo.DisplayName = " ChannelNo--" + nextChannelInfo.ChannelNo.ToString() + ", LoadName--" + nextChannelInfo.LoadName + ", LoadType--" + nextChannelInfo.LoadType.ToString();
        //            deviceCInfo.SequenceId = dInfoEntity.Count() + 1;
        //            deviceCInfo.ParentId = parentSequence;
        //            dInfoEntity.Add(deviceCInfo);

        //            foreach (ChannelStatus nextCStatus in nextChannelInfo.ChannelStatuses)
        //            {
        //                deviceInfo = new DeviceInfoEntity();
        //                deviceInfo.DisplayName = " Status--" + nextCStatus.Status.ToString() + ", Value--" + nextCStatus.Value.ToString();//nextChannelInfo.ChannelNo.ToString();
        //                deviceInfo.SequenceId = dInfoEntity.Count() + 1;
        //                deviceInfo.ParentId = deviceCInfo.SequenceId;
        //                dInfoEntity.Add(deviceInfo);
        //            }
        //        }


        //    }

        //    return dInfoEntity;
        //}

        //public static List<Device> FillDeviceInformations(this IRepositoryAsync<Device> repository, IEnumerable<Device> model, List<Device> deviceModel)
        //{        
        //    foreach (var item in model)
        //    {
        //        //check already exist or not.
        //        IEnumerable<Device> temp = IsDeviceExists(item.Id, item.DeviceHash, repository);
        //        if (temp.Count() == 0)
        //        {
        //            //new item
        //            deviceModel.Add(item);
        //            continue;
        //        }
        //        else
        //        {
        //            foreach (var existingItem in temp.ToList())
        //            {
        //                //modify version                    
        //                FillExistingDeviceInfo(item, existingItem);

        //                if (item.DeviceStatus != null && item.DeviceStatus.Count > 0)
        //                {
        //                    AddOrEditExistingDeviceStatus(item, existingItem);
        //                    AddOrEditExistingChannels(item, existingItem);
        //                }

        //            }
        //        }
        //    }

        //    return deviceModel;
        //}


        //private static void AddOrEditExistingChannels(Device item, Device existingItem)
        //{
        //    foreach (var nextChannel in item.Channels)
        //    {
        //        var tempExistingChannel = existingItem.Channels.Where(p => p.Id == nextChannel.Id).FirstOrDefault();
        //        if (tempExistingChannel != null)
        //        {
        //            //modify
        //            FillExistingChannelInfo(nextChannel, tempExistingChannel);
        //        }
        //        else
        //        {
        //            //add
        //            existingItem.Channels.Add(nextChannel);
        //        }
        //    }
        //}

        //private static void FillExistingChannelInfo(Channel nextChannel, Channel tempExistingChannel)
        //{
        //    tempExistingChannel.ObjectState = ObjectState.Modified;
        //    tempExistingChannel.Id = nextChannel.Id;
        //    tempExistingChannel.DId = nextChannel.DId;
        //    tempExistingChannel.ChannelNo = nextChannel.ChannelNo;
        //    tempExistingChannel.LoadName = nextChannel.LoadName;
        //    tempExistingChannel.LoadType = nextChannel.LoadType;
        //    tempExistingChannel.AuditField = new AuditFields();

        //    AddOrEditExistingChannelStatus(nextChannel, tempExistingChannel);

        //}

        //private static void AddOrEditExistingChannelStatus(Channel nextChannel, Channel tempExistingChannel)
        //{

        //    //channel status
        //    foreach (var nextChannelStatus in nextChannel.ChannelStatuses)
        //    {
        //        var tempExistingChannelStatus = tempExistingChannel.ChannelStatuses.Where(p => p.Id == nextChannelStatus.Id).FirstOrDefault();
        //        if (tempExistingChannelStatus != null)
        //        {
        //            //modify
        //            FillExistingChannelStatusInfo(nextChannelStatus, tempExistingChannelStatus);
        //        }
        //        else
        //        {
        //            //add
        //            tempExistingChannel.ChannelStatuses.Add(nextChannelStatus);
        //        }
        //    }
        //}

        //private static void FillExistingChannelStatusInfo(ChannelStatus nextChannelStatus, ChannelStatus tempExistingChannelStatus)
        //{
        //    tempExistingChannelStatus.ObjectState = ObjectState.Modified;
        //    tempExistingChannelStatus.Id = nextChannelStatus.Id;
        //    tempExistingChannelStatus.CId = nextChannelStatus.CId;
        //    tempExistingChannelStatus.DId = nextChannelStatus.DId;
        //    tempExistingChannelStatus.ChannelNo = nextChannelStatus.ChannelNo;
        //    tempExistingChannelStatus.Status = nextChannelStatus.Status;
        //    tempExistingChannelStatus.Value = nextChannelStatus.Value;
        //    tempExistingChannelStatus.AuditField = new AuditFields();
        //}

        //private static void AddOrEditExistingDeviceStatus(Device item, Device existingItem)
        //{
        //    foreach (var nextDStatus in item.DeviceStatus)
        //    {
        //        var tempExistingDStatus = existingItem.DeviceStatus.Where(p => p.Id == nextDStatus.Id).FirstOrDefault();
        //        if (tempExistingDStatus != null)
        //        {
        //            //modify
        //            FillExistingVDetailInfo(nextDStatus, tempExistingDStatus);
        //        }
        //        else
        //        {
        //            //add
        //            existingItem.DeviceStatus.Add(nextDStatus);
        //        }
        //    }
        //}

        //private static void FillExistingVDetailInfo(DeviceStatus nextDStatus, DeviceStatus tempExistingDStatus)
        //{
        //    tempExistingDStatus.ObjectState = ObjectState.Modified;
        //    tempExistingDStatus.Id = nextDStatus.Id;
        //    tempExistingDStatus.DId = nextDStatus.DId;
        //    tempExistingDStatus.StatusType = nextDStatus.StatusType;
        //    tempExistingDStatus.Status = nextDStatus.Status;
        //    tempExistingDStatus.AuditField = new AuditFields();
        //}

        //private static void FillExistingDeviceInfo(Device item, Device existingItem)
        //{
        //    existingItem.ObjectState = ObjectState.Modified;

        //    existingItem.Id = item.Id;
        //    existingItem.DId = item.DId;
        //    existingItem.DeviceName = item.DeviceName;
        //    existingItem.DeviceHash = item.DeviceHash;
        //    existingItem.DeviceVersion = item.DeviceVersion;
        //    existingItem.IsDeleted = item.IsDeleted;
        //    existingItem.Watt = item.Watt;
        //    existingItem.Mac = item.Mac;
        //    existingItem.DeviceType = item.DeviceType;
        //    existingItem.AuditField = new AuditFields();

        //}

        //private static IEnumerable<Device> IsDeviceExists(int key, string deviceHash, IRepositoryAsync<Device> repository)
        //{
        //    return repository.Query(e => e.Id == key && e.DeviceHash == deviceHash).Include(x => x.DeviceStatus).Include(x => x.Channels.Select(y => y.ChannelStatuses)).Select();
        //}

    }
}
