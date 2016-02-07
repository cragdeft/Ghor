using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Model.Models;
using SmartHome.Service.Interfaces;
//using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{
    public class ConfigurationParserManagerService : IConfigurationParserManagerService
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<Version> _versionRepository;
        private readonly IRepositoryAsync<Device> _deviceRepository;
        private string _email;
        #endregion

        public ConfigurationParserManagerService(IUnitOfWorkAsync unitOfWorkAsync)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _versionRepository = _unitOfWorkAsync.RepositoryAsync<Version>();
            _deviceRepository = _unitOfWorkAsync.RepositoryAsync<Device>();
        }


        #region version AddOrUpdateGraphRange
        public IEnumerable<Version> AddOrUpdateVersionGraphRange(IEnumerable<Version> model)
        {
            List<Version> versionModel = new List<Version>();
            versionModel = FillVersionInformations(model, versionModel);
            _versionRepository.InsertOrUpdateGraphRange(versionModel);
            return versionModel;
        }


        public List<Version> FillVersionInformations(IEnumerable<Version> model, List<Version> versionModel)
        {
            foreach (var item in model)
            {
                //check already exist or not.
                IEnumerable<Version> temp = IsVersionExists(item.Id, item.Mac);
                if (temp.Count() == 0)
                {
                    //new item
                    versionModel.Add(item);
                    continue;
                }
                else
                {
                    //existing item               
                    // versionModel = temp;
                    foreach (var existingItem in temp.ToList())
                    {
                        //modify version                    
                        FillExistingVersionInfo(item, existingItem);

                        if (item.VersionDetails != null && item.VersionDetails.Count > 0)
                        {
                            AddOrEditExistingVDetailItems(item, existingItem);
                        }

                    }
                }
            }

            return versionModel;
        }



        private void AddOrEditExistingVDetailItems(Version item, Version existingItem)
        {
            foreach (var nextVDetail in item.VersionDetails)
            {
                var tempExistingVDetail = existingItem.VersionDetails.Where(p => p.Id == nextVDetail.Id).FirstOrDefault();
                if (tempExistingVDetail != null)
                {
                    //modify
                    FillExistingVDetailInfo(nextVDetail, tempExistingVDetail);
                }
                else
                {
                    //add
                    existingItem.VersionDetails.Add(nextVDetail);
                }
            }
        }

        private void FillExistingVDetailInfo(VersionDetail nextVDetail, VersionDetail tempExistingVDetail)
        {
            tempExistingVDetail.ObjectState = ObjectState.Modified;
            tempExistingVDetail.Id = nextVDetail.Id;
            tempExistingVDetail.VId = nextVDetail.VId;
            tempExistingVDetail.HardwareVersion = nextVDetail.HardwareVersion;
            tempExistingVDetail.DeviceType = nextVDetail.DeviceType;
            tempExistingVDetail.AuditField = new AuditFields();
        }

        private void FillExistingVersionInfo(Version item, Version existingItem)
        {
            existingItem.AppName = item.AppName;
            existingItem.AppVersion = item.AppVersion;
            existingItem.AuditField = new AuditFields();
            existingItem.AuthCode = item.AuthCode;
            existingItem.Mac = item.Mac;
            existingItem.Id = item.Id;
            existingItem.ObjectState = ObjectState.Modified;
        }

        private IEnumerable<Version> IsVersionExists(string key, string Mac)
        {
            return _versionRepository.Query(e => e.Id == key && e.Mac == Mac).Include(x => x.VersionDetails).Select();
        }



        #endregion


        #region device AddOrUpdateGraphRange


        public IEnumerable<Device> AddOrUpdateDeviceGraphRange(IEnumerable<Device> model)
        {
            List<Device> deviceModel = new List<Device>();
            deviceModel = FillDeviceInformations(model, deviceModel);
            _deviceRepository.InsertOrUpdateGraphRange(deviceModel);
            return deviceModel;
        }


        public List<Device> FillDeviceInformations(IEnumerable<Device> model, List<Device> deviceModel)
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
            return _deviceRepository.Query(e => e.Id == key && e.DeviceHash == deviceHash).Include(x => x.DeviceStatus).Include(x => x.Channels.Select(y => y.ChannelStatuses)).Select();
        }


        #endregion
    }
}
