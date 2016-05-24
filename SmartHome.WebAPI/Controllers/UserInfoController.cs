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


        [Route("api/GetRegisteredUser")]
        public HttpResponseMessage PostRegisteredUserByEmail(JObject encryptedString)
        {

            #region MyRegion

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
                return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");//failure
            }

            oLoginObject = JsonConvert.DeserializeObject<LoginObjectEntity>(msg);
            var oUserInfo = oLoginObject.UserInfo.First();

            #endregion


            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IUserInfoService service = new UserInfoService(unitOfWork);

                try
                {


                    try
                    {
                        var isEmailExists = service.IsLoginIdUnique(oUserInfo.Email);
                        if (isEmailExists)
                        {

                            oRootObject.data = new LoginObjectEntity();
                            LoginMessage oLoginMessage = SetLoginMessage("User already exist", HttpStatusCode.Conflict);
                            oRootObject.MESSAGE = new LoginMessage();
                            oRootObject.MESSAGE = oLoginMessage;
                            msg = JsonConvert.SerializeObject(oRootObject);
                            //already exist

                            //response = Request.CreateResponse(HttpStatusCode.Conflict, msg);//failed

                            response = new HttpResponseMessage() { Content = new StringContent(msg, Encoding.UTF8, "application/json") };

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
                            }

                            oRootObject.data = new LoginObjectEntity();
                            LoginMessage oLoginMessage = SetLoginMessage("Unique user", HttpStatusCode.OK);
                            oRootObject.MESSAGE = new LoginMessage();
                            oRootObject.MESSAGE = oLoginMessage;
                            msg = JsonConvert.SerializeObject(oRootObject);
                            //already exist

                            //response = Request.CreateResponse(HttpStatusCode.OK, msg);//failed

                            response = new HttpResponseMessage() { Content = new StringContent(msg, Encoding.UTF8, "application/json") };


                        }
                    }
                    catch (Exception ex)
                    {

                        oRootObject.data = new LoginObjectEntity();
                        LoginMessage oLoginMessage = SetLoginMessage(ex.ToString(), HttpStatusCode.BadRequest);
                        oRootObject.MESSAGE = new LoginMessage();
                        oRootObject.MESSAGE = oLoginMessage;
                        msg = JsonConvert.SerializeObject(oRootObject);
                        //already exist

                        //response = Request.CreateResponse(HttpStatusCode.BadRequest, msg);
                        response = new HttpResponseMessage() { Content = new StringContent(msg, Encoding.UTF8, "application/json") };

                    }

                    return response;

                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                }
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Internal Server Error");
        }


        [Route("api/CheckUser")]
        [HttpPost]
        public HttpResponseMessage PostUniqueUserByEmail(JObject encryptedString)
        {

            HttpResponseMessage response;
            LoginRootObjectEntity oRootObject = new LoginRootObjectEntity();
            LoginObjectEntity oLoginObject = new LoginObjectEntity();


            string msg = string.Empty;

            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IUserInfoService service = new UserInfoService(unitOfWork);

                try
                {
                    msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
                    if (string.IsNullOrEmpty(msg))
                    {
                        return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");//failure
                    }

                    oLoginObject = JsonConvert.DeserializeObject<LoginObjectEntity>(msg);


                    var oUserInfo = oLoginObject.UserInfo.First();
                    //oUserInfo.Email = "shuaibmeister@gmail.com";
                    //oUserInfo.Password = "dJOreK9VX5ZEdu8j8sqDVg==";
                    var isEmailExists = service.IsValidLogin(oUserInfo.Email, oUserInfo.Password);
                    if (isEmailExists)
                    {


                        unitOfWork.BeginTransaction();
                        try
                        {
                            ObjectInitialization(oLoginObject);

                            var oUserEntity = service.GetUserInfos(oUserInfo.Email, oUserInfo.Password);
                            FillLoginObjectByData(oLoginObject, oUserEntity);

                            oRootObject.data = new LoginObjectEntity();
                            oRootObject.data = oLoginObject;

                            LoginMessage oLoginMessage = SetLoginMessage("Success", HttpStatusCode.OK);
                            oRootObject.MESSAGE = new LoginMessage();
                            oRootObject.MESSAGE = oLoginMessage;
                            msg = JsonConvert.SerializeObject(oRootObject);

                            msg = msg.Replace("false", "0");
                            msg = msg.Replace("true", "1");
                            unitOfWork.Commit();
                        }
                        catch (Exception ex)
                        {

                            oRootObject.data = new LoginObjectEntity();
                            LoginMessage oLoginMessage = SetLoginMessage(ex.ToString(), HttpStatusCode.BadRequest);
                            oRootObject.MESSAGE = new LoginMessage();
                            oRootObject.MESSAGE = oLoginMessage;
                            msg = JsonConvert.SerializeObject(oRootObject);
                            response = new HttpResponseMessage() { Content = new StringContent(msg, Encoding.UTF8, "application/json") };
                            unitOfWork.Rollback();
                        }
                        response = new HttpResponseMessage() { Content = new StringContent(msg, Encoding.UTF8, "application/json") };
                    }
                    else
                    {
                        oRootObject.data = new LoginObjectEntity();
                        LoginMessage oLoginMessage = SetLoginMessage("User not found", HttpStatusCode.NotFound);
                        oRootObject.MESSAGE = new LoginMessage();
                        oRootObject.MESSAGE = oLoginMessage;
                        msg = JsonConvert.SerializeObject(oRootObject);
                        response = new HttpResponseMessage() { Content = new StringContent(msg, Encoding.UTF8, "application/json") };
                        //response = Request.CreateResponse(HttpStatusCode.NotFound, msg);//failure
                    }
                    return response;
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                }
            }
            return null;
        }

        [Route("api/IsUserExist")]
        [HttpPost]
        public HttpResponseMessage PostIsUserExist(JObject encryptedString)
        {
            #region MyRegion

            HttpResponseMessage response;
            LoginObjectEntity oLoginObject = new LoginObjectEntity();
            LoginRootObjectEntity oRootObject = new LoginRootObjectEntity();
            string msg = string.Empty;

            msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
            if (string.IsNullOrEmpty(msg))
            {
                return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");//failure
            }

            oLoginObject = JsonConvert.DeserializeObject<LoginObjectEntity>(msg);
            var oUserInfo = oLoginObject.UserInfo.First();

            #endregion


            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IUserInfoService service = new UserInfoService(unitOfWork);

                try
                {


                    try
                    {
                        oRootObject.data = new LoginObjectEntity();
                        var isEmailExists = service.IsLoginIdUnique(oUserInfo.Email);
                        if (isEmailExists)
                        {


                            LoginMessage oLoginMessage = SetLoginMessage("User already exist", HttpStatusCode.Conflict);
                            oRootObject.MESSAGE = new LoginMessage();
                            oRootObject.MESSAGE = oLoginMessage;
                            msg = JsonConvert.SerializeObject(oRootObject);
                            response = new HttpResponseMessage() { Content = new StringContent(msg, Encoding.UTF8, "application/json") };

                        }
                        else
                        {
                            LoginMessage oLoginMessage = SetLoginMessage("User not exist", HttpStatusCode.OK);
                            oRootObject.MESSAGE = new LoginMessage();
                            oRootObject.MESSAGE = oLoginMessage;
                            msg = JsonConvert.SerializeObject(oRootObject);
                            response = new HttpResponseMessage() { Content = new StringContent(msg, Encoding.UTF8, "application/json") };


                        }
                    }
                    catch (Exception ex)
                    {

                        oRootObject.data = new LoginObjectEntity();
                        LoginMessage oLoginMessage = SetLoginMessage(ex.ToString(), HttpStatusCode.BadRequest);
                        oRootObject.MESSAGE = new LoginMessage();
                        oRootObject.MESSAGE = oLoginMessage;
                        msg = JsonConvert.SerializeObject(oRootObject);
                        //already exist                        
                        response = new HttpResponseMessage() { Content = new StringContent(msg, Encoding.UTF8, "application/json") };

                    }

                    return response;

                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                }
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Internal Server Error");
        }

        private void FillLoginObjectByData(LoginObjectEntity oLoginObject, IEnumerable<UserInfo> oUserEntity)
        {
            FillUserInfoToLoginObject(oLoginObject, oUserEntity);

            FillUserHomeLinkInfoToLoginObject(oLoginObject, oUserEntity);

            FillHomeInfoToLoginObject(oLoginObject, oUserEntity);


            //user room link
            FillUserRoomLinkInfoToLoginObject(oLoginObject, oUserEntity);

            //smart router
            FillSmartRouterInfoToLoginObject(oLoginObject, oUserEntity);

            //room

            FillRoomInfoToLoginObject(oLoginObject, oUserEntity);


            //smart device


            FillSmartDeviceInfoToLoginObject(oLoginObject, oUserEntity);


            //d-status
            FillSmartDeviceStatusInfoToLoginObject(oLoginObject, oUserEntity);

            //channel

            FillChannelInfoToLoginObject(oLoginObject, oUserEntity);
            //c-status
            FillChannelStatusInfoToLoginObject(oLoginObject, oUserEntity);
        }

        private void FillChannelStatusInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserInfo> oUserEntity)
        {
            Mapper.CreateMap<ChannelStatus, ChannelStatusEntity>();
            IEnumerable<ChannelStatusEntity> oChannelStatusEntity = Mapper.Map<IEnumerable<ChannelStatus>, IEnumerable<ChannelStatusEntity>>(oUserEntity.SelectMany(p => p.UserRoomLinks).Select(x => x.Room).SelectMany(y => y.SmartDevices).Where(p => p.DeviceType == Model.Enums.DeviceType.SmartSwitch6g).SelectMany(x => ((SmartSwitch)x).Channels).SelectMany(x => x.ChannelStatuses));
            oLoginObject.ChannelStatus.AddRange(oChannelStatusEntity);
        }

        private void FillChannelInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserInfo> oUserEntity)
        {
            Mapper.CreateMap<Channel, ChannelEntity>();
            IEnumerable<ChannelEntity> oChannelEntity = Mapper.Map<IEnumerable<Channel>, IEnumerable<ChannelEntity>>(oUserEntity.SelectMany(p => p.UserRoomLinks).Select(x => x.Room).SelectMany(y => y.SmartDevices).Where(p => p.DeviceType == Model.Enums.DeviceType.SmartSwitch6g).SelectMany(x => ((SmartSwitch)x).Channels));
            oLoginObject.Channel.AddRange(oChannelEntity);
        }

        private void FillSmartDeviceStatusInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserInfo> oUserEntity)
        {
            Mapper.CreateMap<DeviceStatus, DeviceStatusEntity>();
            IEnumerable<DeviceStatusEntity> oDeviceStatusEntity = Mapper.Map<IEnumerable<DeviceStatus>, IEnumerable<DeviceStatusEntity>>(oUserEntity.SelectMany(p => p.UserRoomLinks).Select(x => x.Room).SelectMany(y => y.SmartDevices).SelectMany(z => z.DeviceStatus));
            oLoginObject.DeviceStatus.AddRange(oDeviceStatusEntity);
        }

        private void FillSmartDeviceInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserInfo> oUserEntity)
        {
            Mapper.CreateMap<SmartDevice, DeviceEntity>();
            IEnumerable<DeviceEntity> oDeviceEntity = Mapper.Map<IEnumerable<SmartDevice>, IEnumerable<DeviceEntity>>(oUserEntity.SelectMany(p => p.UserRoomLinks).Select(x => x.Room).SelectMany(x => x.SmartDevices));
            oLoginObject.Device.AddRange(oDeviceEntity);
        }

        private void FillRoomInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserInfo> oUserEntity)
        {
            Mapper.CreateMap<Room, RoomEntity>();
            IEnumerable<RoomEntity> oRoomEntity = Mapper.Map<IEnumerable<Room>, IEnumerable<RoomEntity>>(oUserEntity.SelectMany(p => p.UserRoomLinks).Select(x => x.Room));
            oLoginObject.Room.AddRange(oRoomEntity);
        }

        private void FillSmartRouterInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserInfo> oUserEntity)
        {
            Mapper.CreateMap<SmartRouterInfo, SmartRouterEntity>();
            IEnumerable<SmartRouterEntity> oSmartRouterEntity = Mapper.Map<IEnumerable<SmartRouterInfo>, IEnumerable<SmartRouterEntity>>(oUserEntity.SelectMany(p => p.UserHomeLinks).Select(x => x.Home).SelectMany(x => x.SmartRouterInfoes));
            oLoginObject.RouterInfo.AddRange(oSmartRouterEntity);
        }

        private void FillHomeInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserInfo> oUserEntity)
        {
            Mapper.CreateMap<Home, HomeEntity>()
                  .ForMember(dest => dest.MeshMode, opt => opt.MapFrom(src => (int)src.MeshMode));
            IEnumerable<HomeEntity> oHomeEntity = Mapper.Map<IEnumerable<Home>, IEnumerable<HomeEntity>>(oUserEntity.SelectMany(p => p.UserHomeLinks).Select(x => x.Home));
            oLoginObject.Home.AddRange(oHomeEntity);
        }

        private void FillUserHomeLinkInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserInfo> oUserEntity)
        {
            Mapper.CreateMap<UserHomeLink, UserHomeLinkEntity>()
                                        .ForMember(dest => dest.UserHomeLinkEntityId, opt => opt.MapFrom(src => src.HId))
                                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.HId))
                                        .ForMember(dest => dest.Home, opt => opt.MapFrom(src => src.Home.Id))
                                        .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.UserInfo.Id));
            IEnumerable<UserHomeLinkEntity> oUserHomeLinkEntity = Mapper.Map<IEnumerable<UserHomeLink>, IEnumerable<UserHomeLinkEntity>>(oUserEntity.SelectMany(p => p.UserHomeLinks));
            oLoginObject.UserHomeLink.AddRange(oUserHomeLinkEntity);
        }


        private void FillUserRoomLinkInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserInfo> oUserEntity)
        {
            Mapper.CreateMap<UserRoomLink, UserRoomLinkEntity>()
                                        //.ForMember(dest => dest.UserRoomLinkEntityId, opt => opt.MapFrom(src => src))
                                        .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.UserInfo.Id))
                                        .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Room.Id));
            IEnumerable<UserRoomLinkEntity> oUserRoomLinkEntity = Mapper.Map<IEnumerable<UserRoomLink>, IEnumerable<UserRoomLinkEntity>>(oUserEntity.SelectMany(p => p.UserRoomLinks));
            oLoginObject.UserRoomLink.AddRange(oUserRoomLinkEntity);
        }

        private void FillUserInfoToLoginObject(LoginObjectEntity oLoginObject, IEnumerable<UserInfo> oUserEntity)
        {
            Mapper.CreateMap<UserInfo, UserInfoEntity>()
                                        .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.Gender))
                                        .ForMember(dest => dest.LoginStatus, opt => opt.MapFrom(src => src.LoginStatus == true ? 1 : 0));

            IEnumerable<UserInfoEntity> oUserInfoEntity = Mapper.Map<IEnumerable<UserInfo>, IEnumerable<UserInfoEntity>>(oUserEntity);
            oLoginObject.UserInfo.Add(oUserInfoEntity.First());
        }

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







    }
}


