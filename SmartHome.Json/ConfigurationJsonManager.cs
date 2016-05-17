using AutoMapper;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.ModelDataContext;
using SmartHome.Model.Models;
using SmartHome.Service;
using SmartHome.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

//namespace SmartHome.Json
//{
//    class ConfigurationJsonManager
//    {
//    }
//}

namespace SmartHome.Json
{
    public class ConfigurationJsonManager
    {
        public ConfigurationJsonManager()
        {

        }

        public void JsonProcess(string JsonString)
        {
            try
            {
                RootObjectEntity oRootObject = JsonDesrialized(JsonString);
                SaveUpdateHomeLink(oRootObject);
                SaveUpdateUserRoomLink(oRootObject);
                SaveUpdateVersion(oRootObject);
                SaveUpdateDevice(oRootObject);
            }
            catch (Exception ex)
            {

            }
        }

        #region Save Information
        private void SaveUpdateDevice(RootObjectEntity oRootObject)
        {
            List<SmartDevice> oSmartDevice = new List<SmartDevice>();
            IEnumerable<SmartSwitch> oDeviceSwitch = MapSmartDeviceObject<SmartSwitch>(oRootObject, DeviceType.SmartSwitch6g);
            IEnumerable<SmartRainbow> oDeviceRainbow = MapSmartDeviceObject<SmartRainbow>(oRootObject, DeviceType.SmartRainbow12);

            IEnumerable<SmartRouter> oDeviceRouter = MapSmartDeviceObject<SmartRouter>(oRootObject, DeviceType.SmartRouter);

            IEnumerable<RgbwStatus> oRgbwStatus = ConfigureRgbwStatus(oRootObject);
            IEnumerable<Channel> oChannel = ConfigureChannel(oRootObject);
            IEnumerable<ChannelStatus> oChannelStatus = ConfigureChannelStatus(oRootObject);
            IEnumerable<DeviceStatus> oDeviceStatus = ConfigureDeviceStatus(oRootObject);

            MergeDeviceDeviceStatusAndChannel(oDeviceSwitch, oChannel, oChannelStatus, oDeviceStatus, oSmartDevice);
            MergeDeviceDeviceStatusAndRgbStatus(oDeviceRainbow, oRgbwStatus, oSmartDevice);
            MergeDeviceDeviceAndRouter(oDeviceRouter, oDeviceStatus, oSmartDevice);
            StoreDeviceAndChannel(oSmartDevice, oRootObject.Device);
        }



        private void SaveUpdateVersion(RootObjectEntity oRootObject)
        {
            IEnumerable<Model.Models.Version> oVersion = ConfigureVersion(oRootObject);
            IEnumerable<Model.Models.VersionDetail> oVersionDetail = ConfigureVersionDetail(oRootObject);
            MergeVersionAndVersionDetail(oVersion, oVersionDetail);
            StoreVersionAndVersionDetail(oVersion);
        }



        private void SaveUpdateHomeLink(RootObjectEntity oRootObject)
        {
            IEnumerable<Model.Models.UserInfo> oUserInfo = ConfigureUserInfo(oRootObject);
            IEnumerable<Model.Models.Home> oHome = ConfigureHome(oRootObject);
            IEnumerable<Model.Models.Room> oRoom = ConfigureRoom(oRootObject);
            IEnumerable<Model.Models.SmartRouterInfo> oSmartRouterInfo = ConfigureSmartRouterInfo(oRootObject);
            List<UserHomeLink> oUserHomeLink = new List<UserHomeLink>();
            oUserHomeLink = MergeHomeAndRoomAndUser(oHome, oRoom, oUserInfo, oSmartRouterInfo, oRootObject.UserHomeLink);
            StoreHomeAndRoomAndUser(oUserHomeLink);
        }




        private void SaveUpdateUserRoomLink(RootObjectEntity oRootObject)
        {
            IEnumerable<Model.Models.UserInfo> oUserInfo = ConfigureUserInfo(oRootObject);
            //IEnumerable<Model.Models.Home> oHome = ConfigureHome(oRootObject);
            IEnumerable<Model.Models.Room> oRoom = ConfigureRoom(oRootObject);
            IEnumerable<Model.Models.SmartRouterInfo> oSmartRouterInfo = ConfigureSmartRouterInfo(oRootObject);
            List<UserRoomLink> oUserRoomLink = new List<UserRoomLink>();
            oUserRoomLink = MergeRoomAndUser(oRoom, oUserInfo, oRootObject.UserRoomLink);
            StoreRoomAndUser(oUserRoomLink);
        }

        #endregion

        //public T JsonProcess<T>(string JsonString)
        //{
        //    return JsonConvert.DeserializeObject<T>(JsonString);
        //}

        private RootObjectEntity JsonDesrialized(string JsonString)
        {
            return JsonConvert.DeserializeObject<RootObjectEntity>(JsonString);
        }

        #region Version Conversion
        private IEnumerable<Model.Models.VersionDetail> ConfigureVersionDetail(RootObjectEntity oRootObject)
        {
            Mapper.CreateMap<VersionDetailEntity, Model.Models.VersionDetail>()
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state
                                                                                         // .ForMember(dest => dest.VId, opt => opt.MapFrom(src => src.VersionID));
            IEnumerable<Model.Models.VersionDetail> oVersionDetail = Mapper.Map<IEnumerable<Entity.VersionDetailEntity>, IEnumerable<Model.Models.VersionDetail>>(oRootObject.VersionDetails);
            return oVersionDetail;
        }

        private IEnumerable<Model.Models.Version> ConfigureVersion(RootObjectEntity myObj)
        {
            Mapper.CreateMap<VersionEntity, Model.Models.Version>()
                        //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
                        .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))//audit field
                        .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state
            IEnumerable<Model.Models.Version> oVersion = Mapper.Map<IEnumerable<VersionEntity>, IEnumerable<Model.Models.Version>>(myObj.Version);
            return oVersion;
        }






        #endregion

        #region Merge

        private List<UserHomeLink> MergeHomeAndRoomAndUser(IEnumerable<Home> oHome, IEnumerable<Room> oRoom, IEnumerable<UserInfo> oUserInfo, IEnumerable<SmartRouterInfo> oSmartRouterInfo, IEnumerable<UserHomeLinkEntity> oUserHomeLink)
        {
            foreach (var item in oHome)
            {
                item.Rooms = oRoom.Where(p => p.HId == item.Id).ToArray();
                item.SmartRouterInfoes = oSmartRouterInfo.Where(p => p.HId == item.Id).ToArray();
            }

            List<UserHomeLink> oUserHomeList = new List<UserHomeLink>();
            foreach (var item in oUserHomeLink)
            {

                var userHomeLink = new UserHomeLink
                {
                    Id = item.Id,
                    HId = Convert.ToInt32(item.Home),
                    UInfoId = Convert.ToInt32(item.User),
                    Home = oHome.FirstOrDefault(p => p.Id == item.Home),
                    UserInfo = oUserInfo.FirstOrDefault(p => p.Id == item.User),
                    IsAdmin = item.IsAdmin,
                    IsSynced = item.IsSynced,
                    ObjectState = ObjectState.Added
                };
                oUserHomeList.Add(userHomeLink);
            }
            return oUserHomeList;

        }


        private List<UserRoomLink> MergeRoomAndUser(IEnumerable<Room> oRoom, IEnumerable<UserInfo> oUserInfo,  IEnumerable<UserRoomLinkEntity> oUserRoomLinkEntity)
        {
          

            List<UserRoomLink> oUserRoomLink = new List<UserRoomLink>();
            foreach (var item in oUserRoomLinkEntity)
            {

                var userRoomLink = new UserRoomLink
                {
                    Id = item.Id,
                    RId = Convert.ToInt32(item.Room),                    
                    UInfoId = Convert.ToInt32(item.User),
                    Room = oRoom.FirstOrDefault(p => p.Id == item.Room),
                    UserInfo = oUserInfo.FirstOrDefault(p => p.Id == item.User),
                    IsSynced = item.IsSynced,
                    ObjectState = ObjectState.Added
                };
                oUserRoomLink.Add(userRoomLink);
            }
            return oUserRoomLink;

        }

        private void MergeVersionAndVersionDetail(IEnumerable<Model.Models.Version> oVersion, IEnumerable<VersionDetail> oVersionDetail)
        {
            foreach (var item in oVersion)
            {
                item.VersionDetails = oVersionDetail.Where(p => p.VId == item.Id).ToArray();
            }
        }


        private void MergeDeviceDeviceStatusAndChannel(IEnumerable<SmartSwitch> oDevice, IEnumerable<Channel> oChannel, IEnumerable<ChannelStatus> oChannelStatus, IEnumerable<DeviceStatus> oDeviceStatus, List<SmartDevice> oSmartDevice)
        {
            foreach (var item in oChannel)
            {
                item.ChannelStatuses = oChannelStatus.Where(p => p.CId == item.Id).ToArray();
            }

            foreach (var item in oDevice)
            {
                item.Channels = oChannel.Where(p => p.DId == item.Id).ToArray();
                item.DeviceStatus = oDeviceStatus.Where(p => p.DId == item.Id).ToArray();
                item.AuditField = new AuditFields();
                item.ObjectState = ObjectState.Added;
                oSmartDevice.Add(item);
            }
        }

        private void MergeDeviceDeviceStatusAndRgbStatus(IEnumerable<SmartRainbow> oDevice, IEnumerable<RgbwStatus> oRgbwStatus, List<SmartDevice> oSmartDevice)
        {

            foreach (var item in oDevice)
            {
                item.RgbwStatuses = oRgbwStatus.Where(p => p.DId == item.Id).ToArray();
                item.AuditField = new AuditFields();
                item.ObjectState = ObjectState.Added;
                oSmartDevice.Add(item);
            }
        }


        private void MergeDeviceDeviceAndRouter(IEnumerable<SmartRouter> oDevice, IEnumerable<DeviceStatus> oDeviceStatus, List<SmartDevice> oSmartDevice)
        {
            foreach (var item in oDevice)
            {
                //item.Channels = oChannel.Where(p => p.DId == item.Id).ToArray();
                item.DeviceStatus = oDeviceStatus.Where(p => p.DId == item.Id).ToArray();
                item.AuditField = new AuditFields();
                item.ObjectState = ObjectState.Added;
                oSmartDevice.Add(item);
            }

        }



        #endregion


        #region Configuration

        #region Device




        private IEnumerable<T> MapSmartDeviceObject<T>(RootObjectEntity myObj, DeviceType dType)
        {
            Mapper.CreateMap<DeviceEntity, T>();
            //.ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            //.ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state            
            IEnumerable<T> oDevice = Mapper.Map<IEnumerable<DeviceEntity>, IEnumerable<T>>(myObj.Device.Where(x => x.DeviceType == dType));
            return oDevice;
        }



        private IEnumerable<Model.Models.RgbwStatus> ConfigureRgbwStatus(RootObjectEntity myObj)
        {
            Mapper.CreateMap<RgbwStatusEntity, Model.Models.RgbwStatus>()
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state
            IEnumerable<Model.Models.RgbwStatus> oDevice = Mapper.Map<IEnumerable<RgbwStatusEntity>, IEnumerable<Model.Models.RgbwStatus>>(myObj.RgbwStatus);
            return oDevice;
        }

        private IEnumerable<Model.Models.DeviceStatus> ConfigureDeviceStatus(RootObjectEntity myObj)
        {
            Mapper.CreateMap<DeviceStatusEntity, Model.Models.DeviceStatus>()
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state
            IEnumerable<Model.Models.DeviceStatus> oDeviceStatus = Mapper.Map<IEnumerable<DeviceStatusEntity>, IEnumerable<Model.Models.DeviceStatus>>(myObj.DeviceStatus);
            return oDeviceStatus;
        }
        #endregion

        #region Channel
        private IEnumerable<Model.Models.Channel> ConfigureChannel(RootObjectEntity myObj)
        {

            Mapper.CreateMap<ChannelEntity, Model.Models.Channel>()
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state
            IEnumerable<Model.Models.Channel> oDevice = Mapper.Map<IEnumerable<ChannelEntity>, IEnumerable<Model.Models.Channel>>(myObj.Channel);
            return oDevice;
        }

        private IEnumerable<Model.Models.ChannelStatus> ConfigureChannelStatus(RootObjectEntity myObj)
        {
            Mapper.CreateMap<ChannelStatusEntity, Model.Models.ChannelStatus>()
                   .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
                   .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state
            IEnumerable<Model.Models.ChannelStatus> oDevice = Mapper.Map<IEnumerable<ChannelStatusEntity>, IEnumerable<Model.Models.ChannelStatus>>(myObj.ChannelStatus);
            return oDevice;
        }
        #endregion

        #region Home

        private IEnumerable<Model.Models.Home> ConfigureHome(RootObjectEntity myObj)
        {
            Mapper.CreateMap<HomeEntity, Model.Models.Home>()
                        .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))//audit field
                        .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state
            IEnumerable<Model.Models.Home> oHome = Mapper.Map<IEnumerable<HomeEntity>, IEnumerable<Model.Models.Home>>(myObj.Home);
            return oHome;
        }
        #endregion

        #region Room

        private IEnumerable<Model.Models.Room> ConfigureRoom(RootObjectEntity oRootObject)
        {
            Mapper.CreateMap<RoomEntity, Model.Models.Room>()
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state                                                                                         
            IEnumerable<Model.Models.Room> oRoom = Mapper.Map<IEnumerable<Entity.RoomEntity>, IEnumerable<Model.Models.Room>>(oRootObject.Room);
            return oRoom;
        }


        #endregion


        #region SmartRouter

        private IEnumerable<Model.Models.SmartRouterInfo> ConfigureSmartRouterInfo(RootObjectEntity oRootObject)
        {
            Mapper.CreateMap<SmartRouterEntity, Model.Models.SmartRouterInfo>()
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state                                                                                         
            IEnumerable<Model.Models.SmartRouterInfo> oSmartRouter = Mapper.Map<IEnumerable<Entity.SmartRouterEntity>, IEnumerable<Model.Models.SmartRouterInfo>>(oRootObject.RouterInfo);
            return oSmartRouter;
        }


        #endregion




        #region UserInfo

        private IEnumerable<Model.Models.UserInfo> ConfigureUserInfo(RootObjectEntity oRootObject)
        {
            Mapper.CreateMap<UserInfoEntity, Model.Models.UserInfo>()
            .ForMember(dest => dest.DateOfBirth, opt => opt.UseValue(DateTime.Now))
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state                                                                                         
            IEnumerable<Model.Models.UserInfo> oUserInfo = Mapper.Map<IEnumerable<Entity.UserInfoEntity>, IEnumerable<Model.Models.UserInfo>>(oRootObject.UserInfo);
            return oUserInfo;
        }


        #endregion

        #endregion

        #region Store
        private void StoreDeviceAndChannel(IEnumerable<Model.Models.SmartDevice> oDevice, IEnumerable<DeviceEntity> oDeviceEntity)
        {
            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IConfigurationParserManagerService service = new ConfigurationParserManagerService(unitOfWork);

                try
                {
                    unitOfWork.BeginTransaction();
                    service.AddOrUpdateDeviceGraphRange(oDevice, oDeviceEntity);
                    var changes = unitOfWork.SaveChanges();
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                }
            }
        }

        private void StoreHomeAndRoomAndUser(IEnumerable<Model.Models.UserHomeLink> oUserHomeLink)
        {
            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IConfigurationParserManagerService service = new ConfigurationParserManagerService(unitOfWork);

                try
                {
                    unitOfWork.BeginTransaction();
                    service.AddOrUpdateHomeGraphRange(oUserHomeLink);
                    var changes = unitOfWork.SaveChanges();
                    unitOfWork.Commit();

                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                }
            }

        }


        private void StoreRoomAndUser(IEnumerable<Model.Models.UserRoomLink> oUserRoomLink)
        {
            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IConfigurationParserManagerService service = new ConfigurationParserManagerService(unitOfWork);

                try
                {
                    unitOfWork.BeginTransaction();
                    service.AddOrUpdateRoomGraphRange(oUserRoomLink);
                    var changes = unitOfWork.SaveChanges();
                    unitOfWork.Commit();

                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                }
            }

        }

        private void StoreVersionAndVersionDetail(IEnumerable<Model.Models.Version> oVersion)
        {

            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IConfigurationParserManagerService service = new ConfigurationParserManagerService(unitOfWork);

                try
                {
                    unitOfWork.BeginTransaction();
                    service.AddOrUpdateVersionGraphRange(oVersion);
                    var changes = unitOfWork.SaveChanges();
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                }
            }
        }
        #endregion


    }
}
