using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.ModelDataContext;
using SmartHome.Model.Models;
using SmartHome.Repository.Repositories;
using SmartHome.Service;
using SmartHome.Service.Interfaces;
using SmartHome.Utility.EncryptionAndDecryption;
using SmartHome.WebAPI.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using SmartHome.Model.ViewModels;

namespace SmartHome.WebAPI.Controllers
{
    public class UserInfoController : ApiController
    {

        public UserInfoController()
        {

        }


        // [Route("api/GetRegisteredUser")]
        [Route("api/RegisterUser")]
        public HttpResponseMessage RegisterUser(JObject encryptedString)
        {
            #region Initialization

            HttpResponseMessage response;
            LoginRootObjectEntity oRootObject = new LoginRootObjectEntity();
            LoginObjectEntity oLoginObject = new LoginObjectEntity();
            oLoginObject.UserHomeLink = new List<UserHomeLinkEntity>();
            oLoginObject.UserInfo = new List<UserInfoEntity>();
            oLoginObject.Home = new List<Entity.HomeEntity>();
            oLoginObject.Room = new List<RoomEntity>();
            string msg = string.Empty;


            msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
            if (string.IsNullOrEmpty(msg))
            {
                return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
            }
            oLoginObject = JsonConvert.DeserializeObject<LoginObjectEntity>(msg);
            var oUserInfo = oLoginObject.UserInfo.First();

            #endregion

            using (IDataContextAsync context = new SmartHomeDataContext())
            {
                using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
                {
                    IUserInfoService service = new UserInfoService(unitOfWork);
                    response = ProcessGetRegisteredUser(oRootObject, oUserInfo, unitOfWork, service);
                }
            }
            return response;
        }      

        [Route("api/GetUserInfo")]
        [HttpPost]
        public HttpResponseMessage GetUserInfo(JObject encryptedString)
        {
            #region Initialization

            LoginRootObjectEntity oRootObject = new LoginRootObjectEntity();
            HttpResponseMessage response = new HttpResponseMessage();
            LoginObjectEntity oLoginObject = new LoginObjectEntity();
            string msg = string.Empty;

            msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
            if (string.IsNullOrEmpty(msg))
            {
                return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
            }

            #endregion
            using (IDataContextAsync context = new SmartHomeDataContext())
            {
                using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
                {
                    IUserInfoService service = new UserInfoService(unitOfWork);
                    IConfigurationParserManagerService serviceConfigure = new ConfigurationParserManagerService(unitOfWork);
                    try
                    {
                        ProcessUserInfomations(oRootObject, response, oLoginObject, msg, service, serviceConfigure);
                    }
                    catch (Exception ex)
                    {
                        oRootObject.data = new LoginObjectEntity();
                        response = PrepareJsonResponse(oRootObject, ex.ToString(), HttpStatusCode.BadRequest);
                    }
                }
            }
            return response;
        }

        // [Route("api/IsUserExist")]
        [Route("api/IsUserExist")]
        [HttpPost]
        public HttpResponseMessage IsUserExist(JObject encryptedString)
        {
            #region Initialization

            HttpResponseMessage response;
            LoginObjectEntity oLoginObject = new LoginObjectEntity();
            LoginRootObjectEntity oRootObject = new LoginRootObjectEntity();
            string msg = string.Empty;

            msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
            if (string.IsNullOrEmpty(msg))
            {
                return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
            }

            oLoginObject = JsonConvert.DeserializeObject<LoginObjectEntity>(msg);
            var oUserInfo = oLoginObject.UserInfo.First();

            #endregion

            using (IDataContextAsync context = new SmartHomeDataContext())
            {
                using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
                {
                    IUserInfoService service = new UserInfoService(unitOfWork);
                    response = ProcessIsUserExist(oRootObject, oUserInfo, service);
                }
            }
            return response;
        }

        #region No Action Methods
        [NonAction]
        private HttpResponseMessage ProcessIsUserExist(LoginRootObjectEntity oRootObject, UserInfoEntity oUserInfo, IUserInfoService service)
        {
            HttpResponseMessage response;
            try
            {
                oRootObject.data = new LoginObjectEntity();
                var isEmailExists = service.IsLoginIdUnique(oUserInfo.Email);
                if (isEmailExists)
                {
                    response = PrepareJsonResponse(oRootObject, "User already exist", HttpStatusCode.Conflict);
                }
                else
                {
                    response = PrepareJsonResponse(oRootObject, "User not exist", HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                oRootObject.data = new LoginObjectEntity();
                response = PrepareJsonResponse(oRootObject, ex.ToString(), HttpStatusCode.BadRequest);

            }

            return response;
        }

        [NonAction]
        private HttpResponseMessage ProcessGetRegisteredUser(LoginRootObjectEntity oRootObject, UserInfoEntity oUserInfo, IUnitOfWorkAsync unitOfWork, IUserInfoService service)
        {
            HttpResponseMessage response;
            try
            {
                var isEmailExists = service.IsLoginIdUnique(oUserInfo.Email);
                if (isEmailExists)
                {
                    oRootObject.data = new LoginObjectEntity();
                    response = PrepareJsonResponse(oRootObject, "User already exist", HttpStatusCode.Conflict);
                }
                else
                {
                    unitOfWork.BeginTransaction();
                    try
                    {
                        service.Add(oUserInfo);
                        var changes = unitOfWork.SaveChanges();
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        oRootObject.data = new LoginObjectEntity();
                        response = PrepareJsonResponse(oRootObject, ex.ToString(), HttpStatusCode.BadRequest);
                    }
                    oRootObject.data = new LoginObjectEntity();
                    response = PrepareJsonResponse(oRootObject, "Unique user", HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                oRootObject.data = new LoginObjectEntity();
                response = PrepareJsonResponse(oRootObject, ex.ToString(), HttpStatusCode.BadRequest);
            }

            return response;
        }

        [NonAction]
        private HttpResponseMessage ProcessUserInfomations(LoginRootObjectEntity oRootObject, HttpResponseMessage response, LoginObjectEntity oLoginObject, string msg, IUserInfoService service, IConfigurationParserManagerService serviceConfigure)
        {
            oLoginObject = JsonConvert.DeserializeObject<LoginObjectEntity>(msg);
            var oUserInfo = oLoginObject.UserInfo.First();
            var user = service.GetsUserInfos(oUserInfo.Email, oUserInfo.Password).FirstOrDefault();
            if (user != null)
            {
                try
                {
                    response = GetUserInformationsFormDatabase(oRootObject, oLoginObject, serviceConfigure, user);
                }
                catch (Exception ex)
                {
                    oRootObject.data = new LoginObjectEntity();
                    response = PrepareJsonResponse(oRootObject, ex.ToString(), HttpStatusCode.BadRequest);
                }
            }
            else
            {
                oRootObject.data = new LoginObjectEntity();
                response = PrepareJsonResponse(oRootObject, "User not found", HttpStatusCode.NotFound);
            }
            return response;
        }

        [NonAction]
        private HttpResponseMessage GetUserInformationsFormDatabase(LoginRootObjectEntity oRootObject, LoginObjectEntity oLoginObject, IConfigurationParserManagerService serviceConfigure, UserInfo user)
        {
            HttpResponseMessage response;
            ObjectInitialization(oLoginObject);
            var homeViewModel = serviceConfigure.GetsHomesAllInfo(user.UserInfoId);
            FillLoginObjectByData(oLoginObject, homeViewModel);
            oRootObject.data = new LoginObjectEntity();
            oRootObject.data = oLoginObject;
            response = PrepareJsonResponseForGetUserInfos(oRootObject, "Success", HttpStatusCode.OK);
            return response;
        }
        [NonAction]
        private HttpResponseMessage PrepareJsonResponseForGetUserInfos(LoginRootObjectEntity oRootObject, string message, HttpStatusCode code)
        {
            oRootObject.MESSAGE = new LoginMessage();
            oRootObject.MESSAGE = SetLoginMessage(message, code);
            string msg = JsonConvert.SerializeObject(oRootObject);
            msg = msg.Replace("false", "0");
            msg = msg.Replace("true", "1");
            return new HttpResponseMessage() { Content = new StringContent(msg, Encoding.UTF8, "application/json") };
        }


        [NonAction]
        private HttpResponseMessage PrepareJsonResponse(LoginRootObjectEntity oRootObject, string message, HttpStatusCode code)
        {
            oRootObject.MESSAGE = new LoginMessage();
            oRootObject.MESSAGE = SetLoginMessage(message, code);
            string msg = JsonConvert.SerializeObject(oRootObject);
            return new HttpResponseMessage() { Content = new StringContent(msg, Encoding.UTF8, "application/json") };
        }
        private void FillLoginObjectByData(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
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
        }
        [NonAction]
        private void FillRgbwStatusInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<RgbwStatus, RgbwStatusEntity>();
            IEnumerable<RgbwStatusEntity> oDeviceStatusEntity = Mapper.Map<IEnumerable<RgbwStatus>, IEnumerable<RgbwStatusEntity>>(homeViewModel.RgbwStatuses);
            oLoginObject.RgbwStatus.AddRange(oDeviceStatusEntity);
        }
        [NonAction]
        private void FillChannelStatusInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            List<ChannelStatus> channelStatuses = new List<ChannelStatus>();
            foreach (Channel ch in homeViewModel.Channels)
            {
                channelStatuses.AddRange(ch.ChannelStatuses);
            }
            Mapper.CreateMap<ChannelStatus, ChannelStatusEntity>();
            IEnumerable<ChannelStatusEntity> oChannelStatusEntity = Mapper.Map<IEnumerable<ChannelStatus>, IEnumerable<ChannelStatusEntity>>(channelStatuses);
            oLoginObject.ChannelStatus.AddRange(oChannelStatusEntity);
        }
        [NonAction]
        private void FillChannelInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<Channel, ChannelEntity>();
            IEnumerable<ChannelEntity> oChannelEntity = Mapper.Map<IEnumerable<Channel>, IEnumerable<ChannelEntity>>(homeViewModel.Channels);
            oLoginObject.Channel.AddRange(oChannelEntity);
        }
        [NonAction]
        private void FillSmartDeviceStatusInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            List<DeviceStatus> deviceStatuses = new List<DeviceStatus>();
            foreach (SmartDevice device in homeViewModel.SmartDevices)
            {
                deviceStatuses.AddRange(device.DeviceStatus);
            }
            Mapper.CreateMap<DeviceStatus, DeviceStatusEntity>();
            IEnumerable<DeviceStatusEntity> oDeviceStatusEntity = Mapper.Map<IEnumerable<DeviceStatus>, IEnumerable<DeviceStatusEntity>>(deviceStatuses);
            oLoginObject.DeviceStatus.AddRange(oDeviceStatusEntity);
        }
        [NonAction]
        private void FillSmartDeviceInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<SmartDevice, SmartDeviceEntity>();
            IEnumerable<SmartDeviceEntity> oDeviceEntity = Mapper.Map<IEnumerable<SmartDevice>, IEnumerable<SmartDeviceEntity>>(homeViewModel.SmartDevices);
            oLoginObject.Device.AddRange(oDeviceEntity);
        }
        [NonAction]
        private void FillRoomInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<Room, RoomEntity>();
            IEnumerable<RoomEntity> oRoomEntity = Mapper.Map<IEnumerable<Room>, IEnumerable<RoomEntity>>(homeViewModel.Rooms);
            oLoginObject.Room.AddRange(oRoomEntity);
        }
        [NonAction]
        private void FillSmartRouterInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<RouterInfo, RouterInfoEntity>();
            IEnumerable<RouterInfoEntity> oSmartRouterEntity = Mapper.Map<IEnumerable<RouterInfo>, IEnumerable<RouterInfoEntity>>(homeViewModel.Routers);
            oLoginObject.RouterInfo.AddRange(oSmartRouterEntity);
        }
        [NonAction]
        private void FillHomeInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<Home, Home>()
                  .ForMember(dest => dest.MeshMode, opt => opt.MapFrom(src => (int)src.MeshMode));
            foreach(Home home in homeViewModel.Homes)
            {
                HomeEntity oHomeEntity = Mapper.Map<Home, HomeEntity>(home);
                oHomeEntity.IsInternet = Convert.ToInt32(home.IsInternet);
                oHomeEntity.IsDefault = Convert.ToInt32(home.IsDefault);
                oHomeEntity.IsActive = Convert.ToInt32(home.IsActive);
                oHomeEntity.IsJsonSynced = Convert.ToInt32(home.IsSynced);

                oLoginObject.Home.Add(oHomeEntity);
            }
            
        }
        [NonAction]
        private void FillUserHomeLinkInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<UserHomeLink, UserHomeLinkEntity>()
                                        .ForMember(dest => dest.UserHomeLinkEntityId, opt => opt.MapFrom(src => src.AppsHomeId))
                                        .ForMember(dest => dest.AppsUserHomeLinkId, opt => opt.MapFrom(src => src.AppsHomeId))
                                        .ForMember(dest => dest.AppsHomeId, opt => opt.MapFrom(src => src.Home.AppsHomeId))
                                        .ForMember(dest => dest.AppsUserId, opt => opt.MapFrom(src => src.UserInfo.AppsUserId));
            
            foreach (UserHomeLink entity in homeViewModel.UserHomeLinks)
            {
                UserHomeLinkEntity linkEntity = Mapper.Map<UserHomeLink, UserHomeLinkEntity>(entity);
                linkEntity.IsAdmin = Convert.ToInt32(entity.IsAdmin);
                linkEntity.IsJsonSynced = Convert.ToInt32(entity.IsSynced);
                oLoginObject.UserHomeLink.Add(linkEntity);
            }
        }

        [NonAction]
        private void FillUserRoomLinkInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {        
            Mapper.CreateMap<UserRoomLink, UserRoomLinkEntity>()
                                        //.ForMember(dest => dest.UserRoomLinkEntityId, opt => opt.MapFrom(src => src))
                                        .ForMember(dest => dest.AppsUserId, opt => opt.MapFrom(src => src.UserInfo.AppsUserId))
                                        .ForMember(dest => dest.AppsRoomId, opt => opt.MapFrom(src => src.Room.AppsRoomId));
            foreach (UserRoomLink link in homeViewModel.UserRoomLinks)
            {
                UserRoomLinkEntity oUserRoomLinkEntity = Mapper.Map<UserRoomLink, UserRoomLinkEntity>(link);
                oUserRoomLinkEntity.IsJsonSynced = Convert.ToInt32(link.IsSynced);
                oLoginObject.UserRoomLink.Add(oUserRoomLinkEntity);
            }
        }
        [NonAction]
        private void FillUserInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<UserInfo, UserInfoEntity>()
                                        .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.Sex))
                                        .ForMember(dest => dest.LoginStatus, opt => opt.MapFrom(src => src.LoginStatus == true ? 1 : 0));

            foreach (UserInfo user in homeViewModel.Users)
            {
                UserInfoEntity oUserInfoEntity = Mapper.Map<UserInfo, UserInfoEntity>(user);
                oUserInfoEntity.LoginStatus = Convert.ToInt32(user.LoginStatus);
                oUserInfoEntity.RegStatus = Convert.ToInt32(user.RegStatus);
                oUserInfoEntity.IsJsonSynced = Convert.ToInt32(user.IsSynced);
                oLoginObject.UserInfo.Add(oUserInfoEntity);
            }            
        }

        [NonAction]
        private void ObjectInitialization(LoginObjectEntity oLoginObject)
        {
            oLoginObject.UserInfo = new List<UserInfoEntity>();
            oLoginObject.UserHomeLink = new List<UserHomeLinkEntity>();
            oLoginObject.UserRoomLink = new List<UserRoomLinkEntity>();
            oLoginObject.RgbwStatus = new List<RgbwStatusEntity>();
            oLoginObject.Home = new List<Entity.HomeEntity>();
            oLoginObject.RouterInfo = new List<RouterInfoEntity>();
            oLoginObject.Room = new List<RoomEntity>();
            oLoginObject.ChannelStatus = new List<ChannelStatusEntity>();
            oLoginObject.Channel = new List<ChannelEntity>();
            oLoginObject.Device = new List<SmartDeviceEntity>();
            oLoginObject.DeviceStatus = new List<DeviceStatusEntity>();
        }

        [NonAction]
        private LoginMessage SetLoginMessage(string message, HttpStatusCode code)
        {
            LoginMessage oLoginMessage = new LoginMessage();
            oLoginMessage.HTTP_MESSAGE = message;
            oLoginMessage.HTTP_STATUS = (int)code;
            return oLoginMessage;
        }
        #endregion
    }
}


