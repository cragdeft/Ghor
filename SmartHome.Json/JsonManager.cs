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

namespace SmartHome.Json
{
    public class JsonManager
    {
        private IUnitOfWorkAsync _unitOfWorkAsync;
        private IConfigurationParserManagerService _configurationPerserService;

        public JsonManager()
        {
            IDataContextAsync context = new SmartHomeDataContext();
            _unitOfWorkAsync = new UnitOfWork(context);
            _configurationPerserService = new ConfigurationParserManagerService(_unitOfWorkAsync);
        }

        public void JsonProcess(string JsonString)
        {
            try
            {

                RootObjectEntity oRootObject = JsonDesrialized(JsonString);


                IEnumerable<Model.Models.UserInfo> oUserInfo = ConfigureUserInfo(oRootObject);
                IEnumerable<Model.Models.Home> oHome = ConfigureHome(oRootObject);
                IEnumerable<Model.Models.Room> oRoom = ConfigureRoom(oRootObject);
                List<UserHomeLink> oUserHomeLink = new List<UserHomeLink>();
                oUserHomeLink = MergeHomeAndRoomAndUser(oHome, oRoom, oUserInfo, oRootObject.UserHomeLink);
                //StoreUserInfo(oUserInfo);

                StoreHomeAndRoomAndUser(oUserHomeLink);


                //IEnumerable<Model.Models.VersionDetail> oVersionDetail = ConfigureVersionDetail(oRootObject);
                //MergeVersionAndVersionDetail(oVersion, oVersionDetail);
                //StoreVersionAndVersionDetail(oVersion);



                IEnumerable<Model.Models.Version> oVersion = ConfigureVersion(oRootObject);
                IEnumerable<Model.Models.VersionDetail> oVersionDetail = ConfigureVersionDetail(oRootObject);
                MergeVersionAndVersionDetail(oVersion, oVersionDetail);
                StoreVersionAndVersionDetail(oVersion);

                IEnumerable<Model.Models.Device> oDevice = ConfigureDevice(oRootObject);
                //new
                IEnumerable<Model.Models.RgbwStatus> oRgbwStatus = ConfigureRgbwStatus(oRootObject);
                IEnumerable<Model.Models.Channel> oChannel = ConfigureChannel(oRootObject);
                IEnumerable<Model.Models.ChannelStatus> oChannelStatus = ConfigureChannelStatus(oRootObject);
                IEnumerable<Model.Models.DeviceStatus> oDeviceStatus = ConfigureDeviceStatus(oRootObject);
                MergeDeviceDeviceStatusAndChannel(oDevice, oRgbwStatus, oChannel, oChannelStatus, oDeviceStatus);
                StoreDeviceAndChannel(oDevice, oRootObject.Device);

            }
            catch (Exception ex)
            {

            }
        }

        public T JsonProcess<T>(string JsonString)
        {
            return JsonConvert.DeserializeObject<T>(JsonString);
        }

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


        #region MyRegion

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

        #region Merge

        private List<UserHomeLink> MergeHomeAndRoomAndUser(IEnumerable<Home> oHome, IEnumerable<Room> oRoom, IEnumerable<UserInfo> oUserInfo, IEnumerable<UserHomeLinkEntity> oUserHomeLink)
        {
            foreach (var item in oHome)
            {
                item.Rooms = oRoom.Where(p => p.HId == item.Id).ToArray();
            }

            List<UserHomeLink> oUserHomeList = new List<UserHomeLink>();
            foreach (var item in oUserHomeLink)
            {

                var userHomeLink = new UserHomeLink
                {
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

        private void MergeVersionAndVersionDetail(IEnumerable<Model.Models.Version> oVersion, IEnumerable<VersionDetail> oVersionDetail)
        {
            foreach (var item in oVersion)
            {
                item.VersionDetails = oVersionDetail.Where(p => p.VId == item.Id).ToArray();
            }

            //oVersion.ToList().ForEach(p => p.VersionDetails = oVersionDetail.Where(q => q.VId == p.Id).ToList());
        }

        private void MergeDeviceDeviceStatusAndChannel(IEnumerable<Device> oDevice, IEnumerable<RgbwStatus> oRgbwStatus, IEnumerable<Channel> oChannel, IEnumerable<ChannelStatus> oChannelStatus, IEnumerable<DeviceStatus> oDeviceStatus)
        {
            foreach (var item in oChannel)
            {
                item.ChannelStatuses = oChannelStatus.Where(p => p.CId == item.Id).ToArray();
            }

            foreach (var item in oDevice)
            {
                item.RgbwStatuses = oRgbwStatus.Where(p => p.DId == item.Id).ToArray();
                item.Channels = oChannel.Where(p => p.DId == item.Id).ToArray();
                item.DeviceStatus = oDeviceStatus.Where(p => p.DId == item.Id).ToArray();
            }
        }

        #endregion


        #region Device conversion




        private IEnumerable<Model.Models.Device> ConfigureDevice(RootObjectEntity myObj)
        {
            Mapper.CreateMap<DeviceEntity, Model.Models.Device>()
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state
            IEnumerable<Model.Models.Device> oDevice = Mapper.Map<IEnumerable<DeviceEntity>, IEnumerable<Model.Models.Device>>(myObj.Device);
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

        private IEnumerable<Model.Models.Channel> ConfigureChannel(RootObjectEntity myObj)
        {

            Mapper.CreateMap<ChannelEntity, Model.Models.Channel>()
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
            //.ForMember(dest => dest.DId, opt => opt.MapFrom(src => src.DeviceID))
            //.ForMember(dest => dest.LoadType, opt => opt.UseValue(58))
            //.ForMember(dest => dest.LoadType, opt => opt.MapFrom(src => (int)GetEnumValue<LoadType>(src.LoadType)))//enum
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



        private IEnumerable<Model.Models.DeviceStatus> ConfigureDeviceStatus(RootObjectEntity myObj)
        {
            Mapper.CreateMap<DeviceStatusEntity, Model.Models.DeviceStatus>()
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
            //.ForMember(dest => dest.DId, opt => opt.MapFrom(src => src.DeviceID))
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state
            IEnumerable<Model.Models.DeviceStatus> oDeviceStatus = Mapper.Map<IEnumerable<DeviceStatusEntity>, IEnumerable<Model.Models.DeviceStatus>>(myObj.DeviceStatus);
            return oDeviceStatus;
        }
        #endregion

        #region Store
        private void StoreDeviceAndChannel(IEnumerable<Model.Models.Device> oDevice, IEnumerable<DeviceEntity> oDeviceEntity)
        {
            _unitOfWorkAsync.BeginTransaction();
            try
            {
                _configurationPerserService.AddOrUpdateDeviceGraphRange(oDevice, oDeviceEntity);
                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();

            }
            catch (Exception ex)
            {
                _unitOfWorkAsync.Rollback();
            }
        }





        private void StoreHomeAndRoomAndUser(IEnumerable<Model.Models.UserHomeLink> oUserHomeLink)
        {
            _unitOfWorkAsync.BeginTransaction();
            try
            {
                _configurationPerserService.AddOrUpdateHomeGraphRange(oUserHomeLink);
                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();

            }
            catch (Exception ex)
            {
                _unitOfWorkAsync.Rollback();
            }
        }

        private void StoreVersionAndVersionDetail(IEnumerable<Model.Models.Version> oVersion)
        {
            _unitOfWorkAsync.BeginTransaction();
            try
            {
                _configurationPerserService.AddOrUpdateVersionGraphRange(oVersion);
                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();

            }
            catch (Exception ex)
            {
                _unitOfWorkAsync.Rollback();
            }
        }
        #endregion


    }
}
