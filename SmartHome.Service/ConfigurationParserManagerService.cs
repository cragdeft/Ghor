using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
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
        private readonly IRepositoryAsync<UserInfo> _userInfoRepository;
        private readonly IRepositoryAsync<UserHomeLink> _userHomeRepository;
        private readonly IRepositoryAsync<Version> _versionRepository;
        private readonly IRepositoryAsync<Device> _deviceRepository;
        private string _email;
        #endregion

        public ConfigurationParserManagerService(IUnitOfWorkAsync unitOfWorkAsync)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _userInfoRepository = _unitOfWorkAsync.RepositoryAsync<UserInfo>();
            _userHomeRepository = _unitOfWorkAsync.RepositoryAsync<UserHomeLink>();
            _versionRepository = _unitOfWorkAsync.RepositoryAsync<Version>();
            _deviceRepository = _unitOfWorkAsync.RepositoryAsync<Device>();
        }



        #region Home AddOrUpdateGraphRange
        public IEnumerable<UserHomeLink> AddOrUpdateHomeGraphRange(IEnumerable<UserHomeLink> model)
        {
            List<UserHomeLink> userHomeModel = new List<UserHomeLink>();
            userHomeModel = FillHomeInformations(model, userHomeModel);
            _userHomeRepository.InsertOrUpdateGraphRange(userHomeModel);
            return userHomeModel;
        }

        public List<UserHomeLink> FillHomeInformations(IEnumerable<UserHomeLink> model, List<UserHomeLink> homeModel)
        {
            foreach (var item in model)
            {
                //check already exist or not.
                IEnumerable<UserHomeLink> temp = IsHomeExists(item.HId);
                if (temp.Count() == 0)
                {
                    //new item
                    homeModel.Add(item);
                    continue;
                }
                else
                {
                    //existing item               
                    // versionModel = temp;
                    foreach (var existingItem in temp.ToList())
                    {
                        //modify version                    
                        var temp2 = item.Home;

                        //FillExistingHomeInfo(item, existingItem);

                        //if (item.Rooms != null && item.Rooms.Count > 0)
                        //{
                        //    AddOrEditExistingRoomItems(item, existingItem);
                        //}

                    }
                }
            }

            return homeModel;
        }



        private void AddOrEditExistingRoomItems(Home item, Home existingItem)
        {
            foreach (var nextRoom in item.Rooms)
            {
                var tempExistingRoom = existingItem.Rooms.Where(p => p.Id == nextRoom.Id).FirstOrDefault();
                if (tempExistingRoom != null)
                {
                    //modify
                    FillExistingRoomInfo(nextRoom, tempExistingRoom);
                }
                else
                {
                    //add
                    existingItem.Rooms.Add(nextRoom);
                }
            }
        }

        private void FillExistingRoomInfo(Room nextRoomDetail, Room tempExistingRoomDetail)
        {
            tempExistingRoomDetail.ObjectState = ObjectState.Modified;
            tempExistingRoomDetail.Id = nextRoomDetail.Id;
            tempExistingRoomDetail.HId = nextRoomDetail.HId;
            tempExistingRoomDetail.Name = nextRoomDetail.Name;
            tempExistingRoomDetail.RoomNumber = nextRoomDetail.RoomNumber;
            tempExistingRoomDetail.Comment = nextRoomDetail.Comment;
            tempExistingRoomDetail.IsMasterRoom = nextRoomDetail.IsMasterRoom;
            tempExistingRoomDetail.IsActive = nextRoomDetail.IsActive;
            tempExistingRoomDetail.AuditField = new AuditFields();
        }

        private void FillExistingHomeInfo(Home item, Home existingItem)
        {

            existingItem.Id = item.Id;
            existingItem.Name = item.Name;
            existingItem.TimeZone = item.TimeZone;
            //existingItem.RegistrationKey = item.RegistrationKey;
            //existingItem.HardwareId = item.HardwareId;
            //existingItem.TrialCount = item.TrialCount;
            existingItem.Comment = item.Comment;
            existingItem.IsActive = item.IsActive;
            existingItem.IsDefault = item.IsDefault;
            existingItem.IsAdmin = item.IsAdmin;
            existingItem.MeshMode = item.MeshMode;
            existingItem.Phone = item.Phone;
            existingItem.PassPhrase = item.PassPhrase;
            existingItem.IsInternet = item.IsInternet;

            existingItem.ObjectState = ObjectState.Modified;
        }

        private IEnumerable<UserHomeLink> IsHomeExists(string key)
        {
            return _userHomeRepository.Query(e => e.HId == key).Include(x=>x.Home).Include(x=>x.UserInfo).Select();
        }



        #endregion


        #region Userinfo AddOrUpdateUserInfoGraphRange
        public IEnumerable<UserInfo> AddOrUpdateUserInfoGraphRange(IEnumerable<UserInfo> model)
        {
            List<UserInfo> userInfoModel = new List<UserInfo>();
            userInfoModel = FillUserInformations(model, userInfoModel);
            _userInfoRepository.InsertOrUpdateGraphRange(userInfoModel);
            return userInfoModel;
        }

        public List<UserInfo> FillUserInformations(IEnumerable<UserInfo> model, List<UserInfo> userInfoModel)
        {
            foreach (var item in model)
            {
                //check already exist or not.
                IEnumerable<UserInfo> temp = IsUserExists(item.Id);
                if (temp.Count() == 0)
                {
                    item.DateOfBirth = System.DateTime.Now;
                    //new item
                    userInfoModel.Add(item);
                    continue;
                }
                else
                {
                    //existing item               
                    // versionModel = temp;
                    foreach (var existingItem in temp.ToList())
                    {
                        //modify version                    
                        FillExistingUserInfo(item, existingItem);

                        //if (item.Rooms != null && item.Rooms.Count > 0)
                        //{
                        //    AddOrEditExistingRoomItems(item, existingItem);
                        //}

                    }
                }
            }

            return userInfoModel;
        }


        private void FillExistingUserInfo(UserInfo item, UserInfo existingItem)
        {
            existingItem.UserInfoId = item.UserInfoId;
            existingItem.Id = item.Id;
            existingItem.LocalId = item.LocalId;
            existingItem.Password = item.Password;
            existingItem.UserName = item.UserName;
            existingItem.FirstName = item.FirstName;
            existingItem.LastName = item.LastName;
            existingItem.MiddleName = item.MiddleName;
            //existingItem.FullName = item.FullName;
            existingItem.AccNo = item.AccNo;
            existingItem.CellPhone = item.CellPhone;
            existingItem.DateOfBirth = item.DateOfBirth;
            existingItem.Gender = item.Gender;
            existingItem.Email = item.Email;
            existingItem.ExpireDate = item.ExpireDate;
            existingItem.OldAcc = item.OldAcc;
            existingItem.SocialSecurityNumber = item.SocialSecurityNumber;
            existingItem.IsEmailRecipient = item.IsEmailRecipient;
            existingItem.IsLoggedIn = item.IsLoggedIn;
            existingItem.IsSMSRecipient = item.IsSMSRecipient;
            existingItem.LastLogIn = item.LastLogIn;
            existingItem.IsActive = item.IsActive;
            existingItem.Country = item.Country;
            existingItem.LoginStatus = item.LoginStatus;
            existingItem.RegStatus = item.RegStatus;
            existingItem.IsSynced = item.IsSynced;

            existingItem.ObjectState = ObjectState.Modified;
        }

        private IEnumerable<UserInfo> IsUserExists(string key)
        {
            return _userInfoRepository.Query(e => e.Id == key).Select();
        }



        #endregion



        #region Home AddOrUpdateGraphRange
        //public IEnumerable<Home> AddOrUpdateHomeGraphRange(IEnumerable<Home> model)
        //{
        //    List<Home> homeModel = new List<Home>();
        //    homeModel = FillHomeInformations(model, homeModel);
        //    _homeRepository.InsertOrUpdateGraphRange(homeModel);
        //    return homeModel;
        //}

        //public List<Home> FillHomeInformations(IEnumerable<Home> model, List<Home> homeModel)
        //{
        //    foreach (var item in model)
        //    {
        //        //check already exist or not.
        //        IEnumerable<Home> temp = IsHomeExists(item.Id);
        //        if (temp.Count() == 0)
        //        {
        //            //new item
        //            homeModel.Add(item);
        //            continue;
        //        }
        //        else
        //        {
        //            //existing item               
        //            // versionModel = temp;
        //            foreach (var existingItem in temp.ToList())
        //            {
        //                //modify version                    
        //                FillExistingHomeInfo(item, existingItem);

        //                if (item.Rooms != null && item.Rooms.Count > 0)
        //                {
        //                    AddOrEditExistingRoomItems(item, existingItem);
        //                }

        //            }
        //        }
        //    }

        //    return homeModel;
        //}



        //private void AddOrEditExistingRoomItems(Home item, Home existingItem)
        //{
        //    foreach (var nextRoom in item.Rooms)
        //    {
        //        var tempExistingRoom = existingItem.Rooms.Where(p => p.Id == nextRoom.Id).FirstOrDefault();
        //        if (tempExistingRoom != null)
        //        {
        //            //modify
        //            FillExistingRoomInfo(nextRoom, tempExistingRoom);
        //        }
        //        else
        //        {
        //            //add
        //            existingItem.Rooms.Add(nextRoom);
        //        }
        //    }
        //}

        //private void FillExistingRoomInfo(Room nextRoomDetail, Room tempExistingRoomDetail)
        //{
        //    tempExistingRoomDetail.ObjectState = ObjectState.Modified;
        //    tempExistingRoomDetail.Id = nextRoomDetail.Id;
        //    tempExistingRoomDetail.HId = nextRoomDetail.HId;
        //    tempExistingRoomDetail.Name = nextRoomDetail.Name;
        //    tempExistingRoomDetail.RoomNumber = nextRoomDetail.RoomNumber;
        //    tempExistingRoomDetail.Comment = nextRoomDetail.Comment;
        //    tempExistingRoomDetail.IsMasterRoom = nextRoomDetail.IsMasterRoom;
        //    tempExistingRoomDetail.IsActive = nextRoomDetail.IsActive;
        //    tempExistingRoomDetail.AuditField = new AuditFields();
        //}

        //private void FillExistingHomeInfo(Home item, Home existingItem)
        //{

        //    existingItem.Id = item.Id;
        //    existingItem.Name = item.Name;
        //    existingItem.TimeZone = item.TimeZone;
        //    //existingItem.RegistrationKey = item.RegistrationKey;
        //    //existingItem.HardwareId = item.HardwareId;
        //    //existingItem.TrialCount = item.TrialCount;
        //    existingItem.Comment = item.Comment;
        //    existingItem.IsActive = item.IsActive;
        //    existingItem.IsDefault = item.IsDefault;
        //    existingItem.IsAdmin = item.IsAdmin;
        //    existingItem.MeshMode = item.MeshMode;
        //    existingItem.Phone = item.Phone;
        //    existingItem.PassPhrase = item.PassPhrase;
        //    existingItem.IsInternet = item.IsInternet;

        //    existingItem.ObjectState = ObjectState.Modified;
        //}

        //private IEnumerable<Home> IsHomeExists(string key)
        //{
        //    return _homeRepository.Query(e => e.Id == key).Include(x => x.Rooms).Select();
        //}



        #endregion


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
                            AddOrEditExistingRgbwStatus(item, existingItem);
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


        #region AddOrEditExistingRgbwStatus
        private void AddOrEditExistingRgbwStatus(Device item, Device existingItem)
        {
            foreach (var nextStatus in item.RgbwStatuses)
            {
                var tempExistingDStatus = existingItem.RgbwStatuses.Where(p => p.Id == nextStatus.Id).FirstOrDefault();
                if (tempExistingDStatus != null)
                {
                    //modify
                    FillExistingRgbInfo(nextStatus, tempExistingDStatus);
                }
                else
                {
                    //add
                    existingItem.RgbwStatuses.Add(nextStatus);
                }
            }
        }

        private void FillExistingRgbInfo(RgbwStatus nextStatus, RgbwStatus tempExistingStatus)
        {
            tempExistingStatus.ObjectState = ObjectState.Modified;
            tempExistingStatus.Id = nextStatus.Id;
            tempExistingStatus.DId = nextStatus.DId;
            tempExistingStatus.RGBColorStatusType = nextStatus.RGBColorStatusType;
            tempExistingStatus.IsPowerOn = nextStatus.IsPowerOn;
            tempExistingStatus.ColorR = nextStatus.ColorR;
            tempExistingStatus.ColorG = nextStatus.ColorG;
            tempExistingStatus.ColorB = nextStatus.ColorB;
            tempExistingStatus.IsWhiteEnabled = nextStatus.IsWhiteEnabled;
            tempExistingStatus.DimmingValue = nextStatus.DimmingValue;
            tempExistingStatus.AuditField = new AuditFields();
        }
        #endregion

        #region AddOrEditExistingDeviceStatus
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
        #endregion

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
            return _deviceRepository.Query(e => e.Id == key && e.DeviceHash == deviceHash).Include(x => x.DeviceStatus).Include(x => x.RgbwStatuses).Include(x => x.Channels.Select(y => y.ChannelStatuses)).Select();
        }


        #endregion


        public IEnumerable<DeviceInfoEntity> GetsDeviceAllInfo()
        {
            // add business logic here
            // return _repository.GetsAllDevice();

            int parentSequence = 0;
            int channelMasterID = 0;
            string displayCaption = string.Empty;
            List<DeviceInfoEntity> dInfoEntity = new List<DeviceInfoEntity>();
            var device = _deviceRepository.Query().Include(x => x.DeviceStatus).Include(x => x.RgbwStatuses).Include(x => x.Channels.Select(y => y.ChannelStatuses)).Select().ToList();

            foreach (Device nextDeviceInfo in device)
            {

                DeviceInfoEntity deviceInfo = new DeviceInfoEntity();
                FillDeviceInfo(out parentSequence, out displayCaption, dInfoEntity, nextDeviceInfo, out deviceInfo);

                deviceInfo = AddCaption(parentSequence, "DeviceStatus", dInfoEntity);
                channelMasterID = deviceInfo.SequenceId;
                foreach (DeviceStatus nextDStatus in nextDeviceInfo.DeviceStatus)
                {
                    AddCaption(channelMasterID, "StatusType--" + nextDStatus.StatusType.ToString() + ", Value--" + nextDStatus.Value.ToString(), dInfoEntity);
                }

                deviceInfo = AddCaption(parentSequence, "RgbwStatuses", dInfoEntity);
                channelMasterID = deviceInfo.SequenceId;
                foreach (RgbwStatus nextStatus in nextDeviceInfo.RgbwStatuses)
                {
                    displayCaption = " StatusType--" + nextStatus.RGBColorStatusType.ToString() + ", IsPowerOn--" + nextStatus.IsPowerOn + ", ColorR--" + nextStatus.ColorR.ToString() + ", ColorG--" + nextStatus.ColorG.ToString() + ", ColorB--" + nextStatus.ColorB.ToString() + ", DimmingValue--" + nextStatus.DimmingValue.ToString() + ", IsWhiteEnabled--" + nextStatus.IsWhiteEnabled.ToString();
                    AddCaption(channelMasterID, displayCaption, dInfoEntity);

                }
                deviceInfo = AddCaption(parentSequence, "Channel", dInfoEntity);
                channelMasterID = deviceInfo.SequenceId;

                foreach (Channel nextChannelInfo in nextDeviceInfo.Channels)
                {
                    displayCaption = " ChannelNo--" + nextChannelInfo.ChannelNo.ToString() + ", LoadName--" + nextChannelInfo.LoadName + ", LoadType--" + nextChannelInfo.LoadType.ToString();
                    deviceInfo = AddCaption(channelMasterID, displayCaption, dInfoEntity);

                    foreach (ChannelStatus nextCStatus in nextChannelInfo.ChannelStatuses)
                    {
                        displayCaption = " Status--" + nextCStatus.Status.ToString() + ", Value--" + nextCStatus.Value.ToString();//nextChannelInfo.ChannelNo.ToString();
                        AddCaption(deviceInfo.SequenceId, displayCaption, dInfoEntity);
                    }
                }


            }

            return dInfoEntity;
        }

        private void FillDeviceInfo(out int parentSequence, out string displayCaption, List<DeviceInfoEntity> dInfoEntity, Device nextDeviceInfo, out DeviceInfoEntity deviceInfo)
        {
            displayCaption = "DId--" + nextDeviceInfo.DId + ", DeviceName--" + nextDeviceInfo.DeviceName + ", DeviceHash--" + nextDeviceInfo.DeviceHash + ", DType--" + nextDeviceInfo.DeviceType.ToString();
            deviceInfo = AddCaption(0, displayCaption, dInfoEntity);
            parentSequence = deviceInfo.SequenceId;
        }

        private DeviceInfoEntity AddCaption(int parentId, string caption, List<DeviceInfoEntity> dInfoEntity)
        {
            DeviceInfoEntity deviceInfo = new DeviceInfoEntity();
            deviceInfo.DisplayName = caption;
            deviceInfo.SequenceId = dInfoEntity.Count() + 1;
            deviceInfo.ParentId = parentId;
            dInfoEntity.Add(deviceInfo);
            return deviceInfo;
        }

        public IEnumerable<VersionInfoEntity> GetsAllVersion()
        {
            // add business logic here
            // return _repository.GetsAllVersion();

            int parentSequence = 0;
            List<VersionInfoEntity> vInfoEntity = new List<VersionInfoEntity>();
            var version = _versionRepository.Query().Include(x => x.VersionDetails).Select().ToList();
            foreach (Version nextVersion in version)
            {
                //AppName,AuthCode,PassPhrase
                VersionInfoEntity versionInfo = new VersionInfoEntity();
                versionInfo.DisplayName = "AppName--" + nextVersion.AppName + ", AuthCode--" + nextVersion.AuthCode + ", PassPhrase--" + nextVersion.PassPhrase;
                versionInfo.ParentId = 0;
                versionInfo.SequenceId = vInfoEntity.Count() + 1;
                parentSequence = versionInfo.SequenceId;
                vInfoEntity.Add(versionInfo);
                foreach (VersionDetail nextVDetail in nextVersion.VersionDetails)
                {
                    versionInfo = new VersionInfoEntity();
                    versionInfo.DisplayName = "DeviceType--" + nextVDetail.DeviceType;
                    versionInfo.ParentId = parentSequence;
                    versionInfo.SequenceId = vInfoEntity.Count() + 1;
                    vInfoEntity.Add(versionInfo);
                }
            }
            return vInfoEntity;
        }
    }
}
