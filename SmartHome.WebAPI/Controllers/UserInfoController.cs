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
using SmartHome.Json;
using SmartHome.Model.Enums;

namespace SmartHome.WebAPI.Controllers
{
    public class UserInfoController : ApiController
    {
        public UserInfoController()
        {

        }

        [Route("api/RegisterUser")]
        public HttpResponseMessage RegisterUser(JObject encryptedString)
        {
            #region Initialization

            HttpResponseMessage response;
            LoginObjectEntity oLoginObject = new LoginObjectEntity();
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
                    response = ProcessGetRegisteredUser(oUserInfo, unitOfWork, service);
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
                        response = ProcessUserInfomations(oRootObject, response, oLoginObject, msg, service, serviceConfigure);
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

        [Route("api/IsUserExist")]
        [HttpPost]
        public HttpResponseMessage IsUserExist(JObject encryptedString)
        {

            #region Initialization

            HttpResponseMessage response;
            string msg = string.Empty;

            msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
            if (string.IsNullOrEmpty(msg))
            {
                return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
            }
            var tempJsonObject = JsonConvert.DeserializeObject<dynamic>(msg);

            #endregion

            using (IDataContextAsync context = new SmartHomeDataContext())
            {
                using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
                {
                    IUserInfoService service = new UserInfoService(unitOfWork);
                    response = ProcessIsUserExist(Convert.ToString(tempJsonObject.Email), service);
                }
            }
            return response;
        }

        [Route("api/ChangePassword")]
        [HttpPost]
        public HttpResponseMessage ChangePassword(JObject encryptedString)
        {
            #region Initialization

            HttpResponseMessage response;
            string msg = string.Empty;
            //"{\"Email\":\"kk.test@apl.com\",\"Password\":\"6ZlPF3qG/+LcC5brkDMZgA==\"}"
            msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
            if (string.IsNullOrEmpty(msg))
            {
                return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
            }
            var tempJsonObject = JsonConvert.DeserializeObject<dynamic>(msg);

            #endregion

            using (IDataContextAsync context = new SmartHomeDataContext())
            {
                using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
                {
                    IUserInfoService service = new UserInfoService(unitOfWork);
                    response = ProcessPasswordUpdate(Convert.ToString(tempJsonObject.Email), Convert.ToString(tempJsonObject.Password), service);
                }
            }
            return response;
        }

        [Route("api/ConfigurationProcess")]
        [HttpPost]
        public HttpResponseMessage ConfigurationProcess(JObject encryptedString)
        {
            HttpResponseMessage response;
            PasswordRecoveryRootObjectEntity oRootObject = new PasswordRecoveryRootObjectEntity();
            try
            {
                #region Initialization

                oRootObject.data = new PasswordRecoveryObjectEntity();
                string msg = string.Empty;

                msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
                if (string.IsNullOrEmpty(msg))
                {
                    return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
                }

                #endregion

                JsonParser jsonManager = new JsonParser(msg, MessageReceivedFrom.Api);
                jsonManager.Save();

                FillPasswordRecoveryInfos("", " Configuration Successfully Process.", HttpStatusCode.OK, oRootObject);
                response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }
            catch (Exception ex)
            {
                FillPasswordRecoveryInfos(string.Empty, ex.ToString(), HttpStatusCode.BadRequest, oRootObject);
                response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }

            return response;
        }

        [Route("api/PasswordRecovery")]
        [HttpPost]
        public HttpResponseMessage PasswordRecovery(JObject encryptedString)
        {
            #region Initialization

            HttpResponseMessage response;
            string msg = string.Empty;

            msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
            if (string.IsNullOrEmpty(msg))
            {
                return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
            }
            var tempJsonObject = JsonConvert.DeserializeObject<dynamic>(msg);


            #endregion

            using (IDataContextAsync context = new SmartHomeDataContext())
            {
                using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
                {
                    IUserInfoService service = new UserInfoService(unitOfWork);
                    response = ProcessPasswordRecovery(Convert.ToString(tempJsonObject.Email), service);
                }
            }
            return response;
        }

        [Route("api/GetFirmwareUpdate")]
        [HttpPost]
        public HttpResponseMessage FirmwareUpdateInfo()
        {
            string firmwareMessage = "{\"SMSW6G\":{\"version\":\"2\", \"file\":\"Switch.img\"}, \"SMRB12\":{\"version\":\"2\", \"file\":\"RGBW_Dynamic_DName_update.img\"}, \"SMCRTV\":{\"version\":\"2\", \"file\":\"Switch.img\"}, \"SMCRTH\":{\"version\":\"2\", \"file\":\"Switch.img\"}, \"SMRWTR\":{\"version\":\"2\", \"file\":\"Switch.img\"}}";
            //string msg = JsonConvert.SerializeObject(firmwareMessage);
            return new HttpResponseMessage() { Content = new StringContent(firmwareMessage, Encoding.UTF8, "application/json") };
        }

        #region No Action Methods

        [NonAction]
        private HttpResponseMessage ProcessPasswordRecovery(string userEmail, IUserInfoService service)
        {
            HttpResponseMessage response;
            PasswordRecoveryRootObjectEntity oRootObject = new PasswordRecoveryRootObjectEntity();
            try
            {
                oRootObject.data = new PasswordRecoveryObjectEntity();
                var isEmailExists = service.IsLoginIdUnique(userEmail);
                if (isEmailExists)
                {

                    FillPasswordRecoveryInfos(service.PasswordRecoveryByEmail(userEmail), "User password", HttpStatusCode.OK, oRootObject);
                    oRootObject.data.UserName = service.GetsUserInfosByEmail(userEmail, oRootObject.data.Password).FirstOrDefault().UserName;
                    response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
                }
                else
                {
                    FillPasswordRecoveryInfos(string.Empty, "User not exist", HttpStatusCode.BadRequest, oRootObject);
                    response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
                }
            }
            catch (Exception ex)
            {
                FillPasswordRecoveryInfos(string.Empty, ex.ToString(), HttpStatusCode.BadRequest, oRootObject);
                response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }

            return response;
        }

        private void FillPasswordRecoveryInfos(string userPassword, string message, HttpStatusCode code, PasswordRecoveryRootObjectEntity oRootObject)
        {
            oRootObject.data.Password = userPassword;
            oRootObject.MESSAGE = new LoginMessage();
            oRootObject.MESSAGE = SetResponseMessage(message, code);
        }

        [NonAction]
        private LoginMessage SetResponseMessage(string message, HttpStatusCode code)
        {
            LoginMessage oLoginMessage = new LoginMessage();
            oLoginMessage.HTTP_MESSAGE = message;
            oLoginMessage.HTTP_STATUS = (int)code;
            return oLoginMessage;
        }

        [NonAction]
        private HttpResponseMessage PrepareJsonResponse<T>(T oRootObject)
        {
            string msg = JsonConvert.SerializeObject(oRootObject);
            return new HttpResponseMessage() { Content = new StringContent(msg, Encoding.UTF8, "application/json") };
        }

        [NonAction]
        private HttpResponseMessage ProcessIsUserExist(string userEmail, IUserInfoService service)
        {
            HttpResponseMessage response;
            PasswordRecoveryRootObjectEntity oRootObject = new PasswordRecoveryRootObjectEntity();
            try
            {
                oRootObject.data = new PasswordRecoveryObjectEntity();
                var isEmailExists = service.IsLoginIdUnique(userEmail);
                if (isEmailExists)
                {
                    FillPasswordRecoveryInfos("", "User already exist", HttpStatusCode.Conflict, oRootObject);
                    response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
                }
                else
                {
                    FillPasswordRecoveryInfos("", "User not exist", HttpStatusCode.OK, oRootObject);
                    response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
                }
            }
            catch (Exception ex)
            {
                FillPasswordRecoveryInfos(string.Empty, ex.ToString(), HttpStatusCode.BadRequest, oRootObject);
                response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }

            return response;
        }

        [NonAction]
        private HttpResponseMessage ProcessPasswordUpdate(string userEmail, string userPassword, IUserInfoService service)
        {
            HttpResponseMessage response;
            PasswordRecoveryRootObjectEntity oRootObject = new PasswordRecoveryRootObjectEntity();
            try
            {
                oRootObject.data = new PasswordRecoveryObjectEntity();
                var isEmailExists = service.IsLoginIdUnique(userEmail);
                if (isEmailExists)
                {
                    service.PasswordUpdate(userEmail, userPassword);
                    FillPasswordRecoveryInfos("", "Successfully Password Update", HttpStatusCode.OK, oRootObject);
                    response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
                }
                else
                {
                    FillPasswordRecoveryInfos("", "Password not update", HttpStatusCode.BadRequest, oRootObject);
                    response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
                }
            }
            catch (Exception ex)
            {
                FillPasswordRecoveryInfos(string.Empty, ex.ToString(), HttpStatusCode.BadRequest, oRootObject);
                response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }

            return response;
        }

        [NonAction]
        private HttpResponseMessage ProcessGetRegisteredUser(UserInfoEntity oUserInfo, IUnitOfWorkAsync unitOfWork, IUserInfoService service)
        {
            HttpResponseMessage response;
            LoginRootObjectEntity oRootObject = new LoginRootObjectEntity();
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
            var user = service.GetsUserInfosByEmail(oUserInfo.Email, oUserInfo.Password).FirstOrDefault();
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
            FillNextAssociatedDeviceInfoToLoginObject(oLoginObject, homeViewModel);
        }

        [NonAction]
        private void FillNextAssociatedDeviceInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<NextAssociatedDevice, NextAssociatedDeviceEntity>();
            IEnumerable<NextAssociatedDeviceEntity> oNADeviceEntity = Mapper.Map<IEnumerable<NextAssociatedDevice>, IEnumerable<NextAssociatedDeviceEntity>>(homeViewModel.NextAssociatedDevice);
            oLoginObject.NextAssociatedDeviceId.AddRange(oNADeviceEntity);
        }

        [NonAction]
        private void FillRgbwStatusInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<RgbwStatus, RgbwStatusEntity>()
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0))
                .ForMember(dest => dest.IsPowerOn, opt => opt.MapFrom(src => src.IsPowerOn == true ? 1 : 0))
                .ForMember(dest => dest.IsWhiteEnabled, opt => opt.MapFrom(src => src.IsWhiteEnabled == true ? 1 : 0));

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
            Mapper.CreateMap<ChannelStatus, ChannelStatusEntity>()
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));
            IEnumerable<ChannelStatusEntity> oChannelStatusEntity = Mapper.Map<IEnumerable<ChannelStatus>, IEnumerable<ChannelStatusEntity>>(channelStatuses);
            oLoginObject.ChannelStatus.AddRange(oChannelStatusEntity);
        }
        [NonAction]
        private void FillChannelInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<Channel, ChannelEntity>()
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));
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
            Mapper.CreateMap<DeviceStatus, DeviceStatusEntity>()
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));
            IEnumerable<DeviceStatusEntity> oDeviceStatusEntity = Mapper.Map<IEnumerable<DeviceStatus>, IEnumerable<DeviceStatusEntity>>(deviceStatuses);
            oLoginObject.DeviceStatus.AddRange(oDeviceStatusEntity);
        }
        [NonAction]
        private void FillSmartDeviceInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<SmartDevice, SmartDeviceEntity>()
                 .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0))
                 .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted == true ? 1 : 0));
            IEnumerable<SmartDeviceEntity> oDeviceEntity = Mapper.Map<IEnumerable<SmartDevice>, IEnumerable<SmartDeviceEntity>>(homeViewModel.SmartDevices);
            oLoginObject.Device.AddRange(oDeviceEntity);
        }
        [NonAction]
        private void FillRoomInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<Room, RoomEntity>()
                .ForMember(dest => dest.IsMasterRoom, opt => opt.MapFrom(src => src.IsMasterRoom == true ? 1 : 0))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == true ? 1 : 0))
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));
            IEnumerable<RoomEntity> oRoomEntity = Mapper.Map<IEnumerable<Room>, IEnumerable<RoomEntity>>(homeViewModel.Rooms);
            oLoginObject.Room.AddRange(oRoomEntity);
        }
        [NonAction]
        private void FillSmartRouterInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<RouterInfo, RouterInfoEntity>()
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));
            IEnumerable<RouterInfoEntity> oSmartRouterEntity = Mapper.Map<IEnumerable<RouterInfo>, IEnumerable<RouterInfoEntity>>(homeViewModel.Routers);
            oLoginObject.RouterInfo.AddRange(oSmartRouterEntity);
        }
        [NonAction]
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
        [NonAction]
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

        [NonAction]
        private void FillUserRoomLinkInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<UserRoomLink, UserRoomLinkEntity>()
                                        .ForMember(dest => dest.AppsUserId, opt => opt.MapFrom(src => src.UserInfo.AppsUserId))
                                        .ForMember(dest => dest.AppsRoomId, opt => opt.MapFrom(src => src.Room.AppsRoomId))
                                        .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));
            IEnumerable<UserRoomLinkEntity> oUserRoomLinkEntity = Mapper.Map<IEnumerable<UserRoomLink>, IEnumerable<UserRoomLinkEntity>>(homeViewModel.UserRoomLinks);
            oLoginObject.UserRoomLink.AddRange(oUserRoomLinkEntity);
        }
        [NonAction]
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
            oLoginObject.NextAssociatedDeviceId = new List<NextAssociatedDeviceEntity>();
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


