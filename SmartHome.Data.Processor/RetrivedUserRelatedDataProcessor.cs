using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
using SmartHome.Model.Enums;
using SmartHome.Model.ModelDataContext;
using SmartHome.Model.Models;
using SmartHome.Service;
using SmartHome.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Entity;
using SmartHome.Model.ViewModels;
using AutoMapper;

namespace SmartHome.Data.Processor
{
    public class RetrivedUserRelatedDataProcessor : BaseDataProcessor
    {

        public dynamic _userEntity { get; private set; }
        public LoginObjectEntity _loginObject { get; private set; }
        public LoginRootObjectEntity _rootObject { get; private set; }

        public RetrivedUserRelatedDataProcessor(string jsonString, MessageReceivedFrom receivedFrom, LoginObjectEntity oLoginObject, LoginRootObjectEntity oRootObject)
        {
            _receivedFrom = receivedFrom;
            _homeJsonMessage = jsonString;
            _userEntity = JsonDesrialized<dynamic>(jsonString);
            _loginObject = oLoginObject;
            _rootObject = oRootObject;
        }



        public bool GetUserInfo()
        {
            MessageLog messageLog = null;
            bool isUserExist = false;

            if (_userEntity == null)
                return false;

            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {

                var transactionRunner = new UnitOfWorkTransactionRunner(unitOfWork);
                try
                {
                    messageLog = transactionRunner.RunTransaction(() => new CommonService(unitOfWork).SaveMessageLog(_homeJsonMessage, _receivedFrom));

                    UserInfo dbUser = transactionRunner.RunSelectTransaction(() => new UserInfoService(unitOfWork).GetsUserInfosByEmailAndPassword(Convert.ToString(_userEntity.Email), Convert.ToString(_userEntity.Password)));
                    if (dbUser != null)
                    {
                        var homeViewModel = transactionRunner.RunSelectTransaction(() => new ConfigurationParserManagerService(unitOfWork).GetsHomesAllInfo(dbUser.UserInfoId));
                        FillLoginObjectByData(homeViewModel);

                        //_rootObject = new LoginRootObjectEntity();
                        _rootObject.data = new LoginObjectEntity();
                        _rootObject.data = _loginObject;
                        isUserExist = true;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    transactionRunner.RunTransaction(() => new CommonService(unitOfWork).UpdateMessageLog(messageLog, string.Empty));
                }
            }
            return isUserExist;

        }

        private void FillLoginObjectByData(HomeViewModel homeViewModel)
        {
            LoginObjectEntity oLoginObject = _loginObject;

            FillUserInfoToLoginObject(oLoginObject, homeViewModel);
            FillUserHomeLinkInfoToLoginObject(oLoginObject, homeViewModel);
            FillHomeInfoToLoginObject(oLoginObject, homeViewModel);
            //user room link
            FillUserRoomLinkInfoToLoginObject(oLoginObject, homeViewModel);
            //smart router
            FillSmartRouterInfoToLoginObject(oLoginObject, homeViewModel);
            //room
            FillRoomInfoToLoginObject(oLoginObject, homeViewModel);
            //smart device
            FillSmartDeviceInfoToLoginObject(oLoginObject, homeViewModel);
            //d-status
            FillSmartDeviceStatusInfoToLoginObject(oLoginObject, homeViewModel);
            //channel
            FillChannelInfoToLoginObject(oLoginObject, homeViewModel);
            //c-status
            FillChannelStatusInfoToLoginObject(oLoginObject, homeViewModel);
            //Rgb-status
            FillRgbwStatusInfoToLoginObject(oLoginObject, homeViewModel);
            FillNextAssociatedDeviceInfoToLoginObject(oLoginObject, homeViewModel);
        }

        private void FillUserInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<UserInfo, UserInfoEntity>()
                            .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.Sex))
                            .ForMember(dest => dest.LoginStatus, opt => opt.MapFrom(src => src.LoginStatus == true ? 1 : 0))
                            .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0))
                            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == true ? 1 : 0))
                            .ForMember(dest => dest.RegStatus, opt => opt.MapFrom(src => src.RegStatus == true ? 1 : 0));

            IEnumerable<UserInfoEntity> oUserInfoEntity = Mapper.Map<IEnumerable<UserInfo>, IEnumerable<UserInfoEntity>>(homeViewModel.Users);
            oLoginObject.UserInfo.AddRange(oUserInfoEntity);
        }

        private void FillUserHomeLinkInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<UserHomeLink, UserHomeLinkEntity>()
                            .ForMember(dest => dest.UserHomeLinkEntityId, opt => opt.MapFrom(src => src.AppsHomeId))
                            .ForMember(dest => dest.AppsUserHomeLinkId, opt => opt.MapFrom(src => src.AppsUserHomeLinkId))
                            .ForMember(dest => dest.AppsHomeId, opt => opt.MapFrom(src => src.Home.AppsHomeId))
                            .ForMember(dest => dest.AppsUserId, opt => opt.MapFrom(src => src.UserInfo.AppsUserId))
                            .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.IsAdmin == true ? 1 : 0))
                            .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));

            IEnumerable<UserHomeLinkEntity> linkEntity = Mapper.Map<IEnumerable<UserHomeLink>, IEnumerable<UserHomeLinkEntity>>(homeViewModel.UserHomeLinks);
            oLoginObject.UserHomeLink.AddRange(linkEntity);
        }

        private void FillHomeInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<Home, HomeEntity>()
                  .ForMember(dest => dest.MeshMode, opt => opt.MapFrom(src => (int)src.MeshMode))
                  .ForMember(dest => dest.IsInternet, opt => opt.MapFrom(src => src.IsInternet == true ? 1 : 0))
                  .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0))
                  .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == true ? 1 : 0))
                  .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(src => src.IsDefault == true ? 1 : 0));
            IEnumerable<HomeEntity> oHomeEntity = Mapper.Map<IEnumerable<Home>, IEnumerable<HomeEntity>>(homeViewModel.Homes);
            oLoginObject.Home.AddRange(oHomeEntity);
        }

        private void FillUserRoomLinkInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<UserRoomLink, UserRoomLinkEntity>()
                                        .ForMember(dest => dest.AppsUserId, opt => opt.MapFrom(src => src.UserInfo.AppsUserId))
                                        .ForMember(dest => dest.AppsRoomId, opt => opt.MapFrom(src => src.Room.AppsRoomId))
                                        .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));
            IEnumerable<UserRoomLinkEntity> oUserRoomLinkEntity = Mapper.Map<IEnumerable<UserRoomLink>, IEnumerable<UserRoomLinkEntity>>(homeViewModel.UserRoomLinks);
            oLoginObject.UserRoomLink.AddRange(oUserRoomLinkEntity);
        }

        private void FillSmartRouterInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<RouterInfo, RouterInfoEntity>()
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0))
                .ForMember(dest => dest.IsExternal, opt => opt.MapFrom(src => src.IsExternal == true ? 1 : 0));
            IEnumerable<RouterInfoEntity> oSmartRouterEntity = Mapper.Map<IEnumerable<RouterInfo>, IEnumerable<RouterInfoEntity>>(homeViewModel.Routers);
            oLoginObject.RouterInfo.AddRange(oSmartRouterEntity);
        }

        private void FillRoomInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<Room, RoomEntity>()
                .ForMember(dest => dest.IsMasterRoom, opt => opt.MapFrom(src => src.IsMasterRoom == true ? 1 : 0))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == true ? 1 : 0))
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));
            IEnumerable<RoomEntity> oRoomEntity = Mapper.Map<IEnumerable<Room>, IEnumerable<RoomEntity>>(homeViewModel.Rooms);
            oLoginObject.Room.AddRange(oRoomEntity);
        }

        private void FillSmartDeviceInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<SmartDevice, SmartDeviceEntity>()
                 .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0))
                 .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted == true ? 1 : 0));
            IEnumerable<SmartDeviceEntity> oDeviceEntity = Mapper.Map<IEnumerable<SmartDevice>, IEnumerable<SmartDeviceEntity>>(homeViewModel.SmartDevices);
            oLoginObject.Device.AddRange(oDeviceEntity);
        }

        private void FillSmartDeviceStatusInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            List<DeviceStatus> deviceStatuses = new List<DeviceStatus>();
            foreach (SmartDevice device in homeViewModel.SmartDevices)
            {
                deviceStatuses.AddRange(device.DeviceStatus);
            }
            Mapper.CreateMap<DeviceStatus, DeviceStatusEntity>()
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));
            IEnumerable<DeviceStatusEntity> oDeviceStatusEntity = Mapper.Map<IEnumerable<DeviceStatus>, IEnumerable<DeviceStatusEntity>>(deviceStatuses);
            oLoginObject.DeviceStatus.AddRange(oDeviceStatusEntity);
        }

        private void FillChannelInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<Channel, ChannelEntity>()
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));
            IEnumerable<ChannelEntity> oChannelEntity = Mapper.Map<IEnumerable<Channel>, IEnumerable<ChannelEntity>>(homeViewModel.Channels);
            oLoginObject.Channel.AddRange(oChannelEntity);
        }

        private void FillChannelStatusInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            List<ChannelStatus> channelStatuses = new List<ChannelStatus>();
            foreach (Channel ch in homeViewModel.Channels)
            {
                channelStatuses.AddRange(ch.ChannelStatuses);
            }
            Mapper.CreateMap<ChannelStatus, ChannelStatusEntity>()
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));
            IEnumerable<ChannelStatusEntity> oChannelStatusEntity = Mapper.Map<IEnumerable<ChannelStatus>, IEnumerable<ChannelStatusEntity>>(channelStatuses);
            oLoginObject.ChannelStatus.AddRange(oChannelStatusEntity);
        }

        private void FillRgbwStatusInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<RgbwStatus, RgbwStatusEntity>()
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0))
                .ForMember(dest => dest.IsPowerOn, opt => opt.MapFrom(src => src.IsPowerOn == true ? 1 : 0))
                .ForMember(dest => dest.IsWhiteEnabled, opt => opt.MapFrom(src => src.IsWhiteEnabled == true ? 1 : 0));

            IEnumerable<RgbwStatusEntity> oDeviceStatusEntity = Mapper.Map<IEnumerable<RgbwStatus>, IEnumerable<RgbwStatusEntity>>(homeViewModel.RgbwStatuses);
            oLoginObject.RgbwStatus.AddRange(oDeviceStatusEntity);
        }

        private void FillNextAssociatedDeviceInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<NextAssociatedDevice, NextAssociatedDeviceEntity>();
            IEnumerable<NextAssociatedDeviceEntity> oNADeviceEntity = Mapper.Map<IEnumerable<NextAssociatedDevice>, IEnumerable<NextAssociatedDeviceEntity>>(homeViewModel.NextAssociatedDevice);
            oLoginObject.NextAssociatedDeviceId.AddRange(oNADeviceEntity);
        }
    }
}
