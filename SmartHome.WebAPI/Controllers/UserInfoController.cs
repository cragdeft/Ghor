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
            oLoginObject.Home = new List<HomeEntity>();
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
            var oUserHomeLink = serviceConfigure.GetsHomesAllInfo(user.UserInfoId, user.UserHomeLinks.FirstOrDefault() == null ? false : user.UserHomeLinks.FirstOrDefault().IsAdmin);
            FillLoginObjectByData(oLoginObject, oUserHomeLink);
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
        private void FillLoginObjectByData(LoginObjectEntity oLoginObject, IEnumerable<UserHomeLink> oUserHomeLink)
        {
            FillUserInfoToLoginObject(oLoginObject, oUserHomeLink);
            FillUserHomeLinkInfoToLoginObject(oLoginObject, oUserHomeLink);
            FillHomeInfoToLoginObject(oLoginObject, oUserHomeLink);
            //user room link
            FillUserRoomLinkInfoToLoginObject(oLoginObject, oUserHomeLink);
            //smart router
            FillSmartRouterInfoToLoginObject(oLoginObject, oUserHomeLink);
            //room
            FillRoomInfoToLoginObject(oLoginObject, oUserHomeLink);
            //smart device
            FillSmartDeviceInfoToLoginObject(oLoginObject, oUserHomeLink);
            //d-status
            FillSmartDeviceStatusInfoToLoginObject(oLoginObject, oUserHomeLink);
            //channel
            FillChannelInfoToLoginObject(oLoginObject, oUserHomeLink);
            //c-status
            FillChannelStatusInfoToLoginObject(oLoginObject, oUserHomeLink);
        }
        [NonAction]
        private void FillChannelStatusInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserHomeLink> oUserHomeLink)
        {
            Mapper.CreateMap<ChannelStatus, ChannelStatusEntity>();
            IEnumerable<ChannelStatusEntity> oChannelStatusEntity = Mapper.Map<IEnumerable<ChannelStatus>, IEnumerable<ChannelStatusEntity>>(oUserHomeLink.Select(p => p.Home).SelectMany(x => x.Rooms.SelectMany(y => y.SmartDevices).Where(p => p.DeviceType == Model.Enums.DeviceType.SmartSwitch6g).SelectMany(p => ((SmartSwitch)p).Channels).SelectMany(z => z.ChannelStatuses)));
            oLoginObject.ChannelStatus.AddRange(oChannelStatusEntity);
        }
        [NonAction]
        private void FillChannelInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserHomeLink> oUserHomeLink)
        {
            Mapper.CreateMap<Channel, ChannelEntity>();
            IEnumerable<ChannelEntity> oChannelEntity = Mapper.Map<IEnumerable<Channel>, IEnumerable<ChannelEntity>>(oUserHomeLink.Select(p => p.Home).SelectMany(x => x.Rooms.SelectMany(y => y.SmartDevices).Where(p => p.DeviceType == Model.Enums.DeviceType.SmartSwitch6g).SelectMany(z => ((SmartSwitch)z).Channels)));
            oLoginObject.Channel.AddRange(oChannelEntity);
        }
        [NonAction]
        private void FillSmartDeviceStatusInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserHomeLink> oUserHomeLink)
        {
            Mapper.CreateMap<DeviceStatus, DeviceStatusEntity>();
            IEnumerable<DeviceStatusEntity> oDeviceStatusEntity = Mapper.Map<IEnumerable<DeviceStatus>, IEnumerable<DeviceStatusEntity>>(oUserHomeLink.Select(p => p.Home).SelectMany(x => x.Rooms.SelectMany(y => y.SmartDevices).SelectMany(z => z.DeviceStatus)));
            oLoginObject.DeviceStatus.AddRange(oDeviceStatusEntity);
        }
        [NonAction]
        private void FillSmartDeviceInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserHomeLink> oUserHomeLink)
        {
            Mapper.CreateMap<SmartDevice, DeviceEntity>();
            IEnumerable<DeviceEntity> oDeviceEntity = Mapper.Map<IEnumerable<SmartDevice>, IEnumerable<DeviceEntity>>(oUserHomeLink.Select(p => p.Home).SelectMany(x => x.Rooms.SelectMany(z => z.SmartDevices)));
            oLoginObject.Device.AddRange(oDeviceEntity);
        }
        [NonAction]
        private void FillRoomInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserHomeLink> oUserHomeLink)
        {
            Mapper.CreateMap<Room, RoomEntity>();
            IEnumerable<RoomEntity> oRoomEntity = Mapper.Map<IEnumerable<Room>, IEnumerable<RoomEntity>>(oUserHomeLink.Select(p => p.Home).SelectMany(x => x.Rooms));
            oLoginObject.Room.AddRange(oRoomEntity);
        }
        [NonAction]
        private void FillSmartRouterInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserHomeLink> oUserHomeLink)
        {
            Mapper.CreateMap<SmartRouterInfo, SmartRouterEntity>();
            IEnumerable<SmartRouterEntity> oSmartRouterEntity = Mapper.Map<IEnumerable<SmartRouterInfo>, IEnumerable<SmartRouterEntity>>(oUserHomeLink.Select(p => p.Home).SelectMany(x => x.SmartRouterInfoes));
            oLoginObject.RouterInfo.AddRange(oSmartRouterEntity);
        }
        [NonAction]
        private void FillHomeInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserHomeLink> oUserHomeLink)
        {
            Mapper.CreateMap<Home, HomeEntity>()
                  .ForMember(dest => dest.MeshMode, opt => opt.MapFrom(src => (int)src.MeshMode));
            IEnumerable<HomeEntity> oHomeEntity = Mapper.Map<IEnumerable<Home>, IEnumerable<HomeEntity>>(oUserHomeLink.Select(p => p.Home));
            oLoginObject.Home.AddRange(oHomeEntity);
        }
        [NonAction]
        private void FillUserHomeLinkInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserHomeLink> oUserHomeLink)
        {
            Mapper.CreateMap<UserHomeLink, UserHomeLinkEntity>()
                                        .ForMember(dest => dest.UserHomeLinkEntityId, opt => opt.MapFrom(src => src.HId))
                                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.HId))
                                        .ForMember(dest => dest.Home, opt => opt.MapFrom(src => src.Home.AppsHomeId))
                                        .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.UserInfo.Id));
            IEnumerable<UserHomeLinkEntity> oUserHomeLinkEntity = Mapper.Map<IEnumerable<UserHomeLink>, IEnumerable<UserHomeLinkEntity>>(oUserHomeLink);
            oLoginObject.UserHomeLink.AddRange(oUserHomeLinkEntity);
        }

        [NonAction]
        private void FillUserRoomLinkInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserHomeLink> oUserHomeLink)
        {
            var temp = oUserHomeLink.Select(e => e.UInfoId).First();
            
            Mapper.CreateMap<UserRoomLink, UserRoomLinkEntity>()
                                        //.ForMember(dest => dest.UserRoomLinkEntityId, opt => opt.MapFrom(src => src))
                                        .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.UserInfo.Id))
                                        .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Room.Id));
            IEnumerable<UserRoomLinkEntity> oUserRoomLinkEntity = Mapper.Map<IEnumerable<UserRoomLink>, IEnumerable<UserRoomLinkEntity>>(oUserHomeLink.Select(x => x.Home).SelectMany(y => y.Rooms.SelectMany(z => z.UserRoomLinks).Where(p=>p.UserInfo.UserInfoId==temp)));
            oLoginObject.UserRoomLink.AddRange(oUserRoomLinkEntity.Where(p=>p.User==temp.ToString()));
        }
        [NonAction]
        private void FillUserInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserHomeLink> oUserHomeLink)
        {
            Mapper.CreateMap<UserInfo, UserInfoEntity>()
                                        .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.Gender))
                                        .ForMember(dest => dest.LoginStatus, opt => opt.MapFrom(src => src.LoginStatus == true ? 1 : 0));

            IEnumerable<UserInfoEntity> oUserInfoEntity = Mapper.Map<IEnumerable<UserInfo>, IEnumerable<UserInfoEntity>>(oUserHomeLink.Select(x => x.UserInfo));
            oLoginObject.UserInfo.Add(oUserInfoEntity.First());
        }

        [NonAction]
        private void ObjectInitialization(LoginObjectEntity oLoginObject)
        {
            oLoginObject.UserInfo = new List<UserInfoEntity>();
            oLoginObject.UserHomeLink = new List<UserHomeLinkEntity>();
            oLoginObject.UserRoomLink = new List<UserRoomLinkEntity>();
            oLoginObject.RgbwStatus = new List<RgbwStatusEntity>();
            oLoginObject.Home = new List<HomeEntity>();
            oLoginObject.RouterInfo = new List<SmartRouterEntity>();
            oLoginObject.Room = new List<RoomEntity>();
            oLoginObject.ChannelStatus = new List<ChannelStatusEntity>();
            oLoginObject.Channel = new List<ChannelEntity>();
            oLoginObject.Device = new List<DeviceEntity>();
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


