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
        // private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        // private readonly IUserInfoService _userInfoService;

        public UserInfoController()
        {

        }


        //public UserInfoController(IUnitOfWorkAsync unitOfWorkAsync, IUserInfoService userInfoService)
        //{
        //    this._unitOfWorkAsync = unitOfWorkAsync;
        //    this._userInfoService = userInfoService;
        //}

        [Route("api/Userinfos")]
        public HttpResponseMessage Get()
        {

            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IUserInfoService service = new UserInfoService(unitOfWork);

                try
                {
                    unitOfWork.BeginTransaction();

                    Mapper.CreateMap<UserInfo, UserInfoEntity>();
                    var userInfos = service.GetsUserInfos();
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, SecurityManager.Encrypt(Mapper.Map<IEnumerable<UserInfo>, IEnumerable<UserInfoEntity>>(userInfos).ToString()));
                    unitOfWork.Commit();
                    return response;
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                }
            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, "");
        }

        [ValidateAntiForgeryToken]
        [Route("api/users")]
        public HttpResponseMessage Post()
        {
            // var employees = EmployeesRepository.InsertEmployee(e);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "");
            return response;
        }

        [Route("api/users")]
        public HttpResponseMessage Put()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "");
            return response;
        }

        [Route("api/users")]
        public HttpResponseMessage Delete()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "");
            return response;
        }

        #region MyRegion
        [Route("api/RegisterUser")]
        public HttpResponseMessage RegisterUser(UserInfoEntity userInfo)
        {


            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IUserInfoService service = new UserInfoService(unitOfWork);

                try
                {
                    unitOfWork.BeginTransaction();

                    HttpResponseMessage response;

                    var isEmailExists = service.IsLoginIdUnique(userInfo.Email);
                    if (!isEmailExists)
                    {
                        unitOfWork.BeginTransaction();
                        try
                        {
                            service.Add(userInfo);
                            // var changes = service.SaveChangesAsync();
                            unitOfWork.Commit();
                        }
                        catch (Exception)
                        {
                            unitOfWork.Rollback();
                        }
                        response = Request.CreateResponse(HttpStatusCode.OK, userInfo);
                    }
                    else
                        response = Request.CreateResponse(HttpStatusCode.OK, true);

                    return response;
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                }
            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, "");

        }

        //[Route("api/GetRegisterUser/{userInfo}")]
        //public HttpResponseMessage GetUniqueUserByEmail(string userInfo)
        //{
        //    RootObjectEntity oRootObject = JsonConvert.DeserializeObject<RootObjectEntity>(userInfo);
        //    var oUserInfo = oRootObject.UserInfo.First();
        //    oUserInfo.Email = oUserInfo.Email + ".com";
        //    HttpResponseMessage response;

        //    try
        //    {
        //        var isEmailExists = _userInfoService.IsLoginIdUnique(oUserInfo.Email);
        //        if (isEmailExists)
        //        {
        //            //response = Request.CreateResponse("message", HttpStatusCode.Conflict,"");//failed

        //        }
        //        else
        //        {
        //            _unitOfWorkAsync.BeginTransaction();
        //            try
        //            {
        //                _userInfoService.Add(oUserInfo);
        //                var changes = _unitOfWorkAsync.SaveChanges();
        //                _unitOfWorkAsync.Commit();
        //            }
        //            catch (Exception ex)
        //            {
        //                _unitOfWorkAsync.Rollback();
        //            }
        //            //response = Request.CreateResponse("message", HttpStatusCode.OK,"");//success
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        //response = Request.CreateResponse("message", ex,"");
        //    }

        //    response = Request.CreateResponse(HttpStatusCode.OK, "test");
        //    return response;
        //} 
        #endregion



        [Route("api/GetUserInfo/{email}/{pass}")]
        public HttpResponseMessage GetUniqueUserByEmail(string email, string pass)
        {

            //

            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IUserInfoService service = new UserInfoService(unitOfWork);

                try
                {
                    unitOfWork.BeginTransaction();
                    UserInfo oUserInfo = new UserInfo();
                    HttpResponseMessage response;
                    email = email + ".com";

                    var isEmailExists = service.IsValidLogin(email, pass);
                    if (isEmailExists)
                    {

                        try
                        {
                            //oUserInfo = _userInfoService.GetUserInfos(email, pass);
                            unitOfWork.Commit();
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                        }
                        response = Request.CreateResponse(HttpStatusCode.OK, oUserInfo);
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.NotFound, "User not found");
                    }

                    return response;

                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                }
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, "User not found");

        }





        //check register u
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

        //login test
        [Route("api/CheckUser")]
        [HttpPost]
        public HttpResponseMessage PostUniqueUserByEmail(JObject encryptedString)
        {

            HttpResponseMessage response;
            LoginRootObjectEntity oRootObject = new LoginRootObjectEntity();
            LoginObjectEntity oLoginObject = new LoginObjectEntity();
            oLoginObject.UserHomeLink = new List<UserHomeLinkEntity>();
            oLoginObject.UserRoomLink = new List<UserRoomLinkEntity>();
            oLoginObject.UserInfo = new List<UserInfoEntity>();
            oLoginObject.Home = new List<HomeEntity>();
            oLoginObject.Room = new List<RoomEntity>();

            oLoginObject.Device = new List<DeviceEntity>();
            oLoginObject.DeviceStatus = new List<DeviceStatusEntity>();
            oLoginObject.Channel = new List<ChannelEntity>();
            oLoginObject.ChannelStatus = new List<ChannelStatusEntity>();

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
                    //oUserInfo.Email = "a@a.com";
                    //oUserInfo.Password = "dJOreK9VX5ZEdu8j8sqDVg==";
                    var isEmailExists = service.IsValidLogin(oUserInfo.Email, oUserInfo.Password);
                    if (isEmailExists)
                    {
                        unitOfWork.BeginTransaction();
                        try
                        {
                            var oUserEntity = service.GetUserInfos(oUserInfo.Email, oUserInfo.Password);
                            Mapper.CreateMap<UserInfo, UserInfoEntity>()
                            .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.Gender))
                            .ForMember(dest => dest.LoginStatus, opt => opt.MapFrom(src => src.LoginStatus == true ? 1 : 0));

                            IEnumerable<UserInfoEntity> oUserInfoEntity = Mapper.Map<IEnumerable<UserInfo>, IEnumerable<UserInfoEntity>>(oUserEntity);
                            oLoginObject.UserInfo = new List<UserInfoEntity>();
                            oLoginObject.UserInfo.Add(oUserInfoEntity.First());

                          //  oLoginObject.UserRoomLink.Add(new UserRoomLinkEntity());
                            foreach (var item in oUserEntity)
                            {

                                Mapper.CreateMap<UserHomeLink, UserHomeLinkEntity>()
                                .ForMember(dest => dest.UserHomeLinkEntityId, opt => opt.MapFrom(src => src.HId))
                                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.HId))
                                .ForMember(dest => dest.Home, opt => opt.MapFrom(src => src.Home.Id))
                                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.UserInfo.Id));
                                IEnumerable<UserHomeLinkEntity> oUserHomeLinkEntity = Mapper.Map<IEnumerable<UserHomeLink>, IEnumerable<UserHomeLinkEntity>>(item.UserHomeLinks);
                                oLoginObject.UserHomeLink = new List<UserHomeLinkEntity>();
                                foreach (UserHomeLinkEntity nextHLink in oUserHomeLinkEntity)
                                {
                                    oLoginObject.UserHomeLink.Add(nextHLink);
                                }

                                oLoginObject.UserRoomLink = new List<UserRoomLinkEntity>();
                                oLoginObject.RgbwStatus = new List<RgbwStatusEntity>();
                                oLoginObject.RouterInfo = new List<RouterInfoEntity>();


                                Mapper.CreateMap<Home, HomeEntity>()
                                      .ForMember(dest => dest.MeshMode, opt => opt.MapFrom(src => (int)src.MeshMode));
                                IEnumerable<HomeEntity> oHomeEntity = Mapper.Map<IEnumerable<Home>, IEnumerable<HomeEntity>>(item.UserHomeLinks.Select(x => x.Home));
                                oLoginObject.Home = new List<HomeEntity>();
                                foreach (HomeEntity nextHome in oHomeEntity)
                                {
                                    oLoginObject.Home.Add(nextHome);
                                }

                                

                                foreach (var nextRoomList in item.UserHomeLinks.Select(x => x.Home).Select(x => x.Rooms))
                                {
                                    Mapper.CreateMap<Room, RoomEntity>();
                                    IEnumerable<RoomEntity> oRoomEntity = Mapper.Map<IEnumerable<Room>, IEnumerable<RoomEntity>>(nextRoomList);
                                    oLoginObject.Room = new List<RoomEntity>();
                                    foreach (RoomEntity nextRoom in oRoomEntity)
                                    {
                                        oLoginObject.Room.Add(nextRoom);
                                    }
                                    //smart device
                                    oLoginObject.ChannelStatus = new List<ChannelStatusEntity>();
                                    oLoginObject.Channel = new List<ChannelEntity>();
                                    oLoginObject.Device = new List<DeviceEntity>();
                                    oLoginObject.DeviceStatus = new List<DeviceStatusEntity>();
                                    foreach (var nextSmartDeviceList in nextRoomList.Select(x => x.SmartDevices))
                                    {
                                        Mapper.CreateMap<SmartDevice, DeviceEntity>();
                                        IEnumerable<DeviceEntity> oDeviceEntity = Mapper.Map<IEnumerable<SmartDevice>, IEnumerable<DeviceEntity>>(nextSmartDeviceList);

                                        foreach (DeviceEntity nextDevice in oDeviceEntity)
                                        {
                                            oLoginObject.Device.Add(nextDevice);
                                        }
                                        //d-status
                                        foreach (var nextDeviceStatusList in nextSmartDeviceList.Select(x => x.DeviceStatus))
                                        {
                                            Mapper.CreateMap<DeviceStatus, DeviceStatusEntity>();
                                            IEnumerable<DeviceStatusEntity> oDeviceStatusEntity = Mapper.Map<IEnumerable<DeviceStatus>, IEnumerable<DeviceStatusEntity>>(nextDeviceStatusList);

                                            foreach (DeviceStatusEntity nextDeviceStatus in oDeviceStatusEntity)
                                            {
                                                oLoginObject.DeviceStatus.Add(nextDeviceStatus);
                                            }

                                        }
                                        //channel

                                        foreach (var nextChannelList in nextSmartDeviceList.Where(p => p.DeviceType == Model.Enums.DeviceType.SmartSwitch6g).Select(x => ((SmartSwitch)x).Channels))
                                        {
                                            Mapper.CreateMap<Channel, ChannelEntity>();
                                            IEnumerable<ChannelEntity> oChannelEntity = Mapper.Map<IEnumerable<Channel>, IEnumerable<ChannelEntity>>(nextChannelList);

                                            foreach (ChannelEntity nextChannel in oChannelEntity)
                                            {
                                                oLoginObject.Channel.Add(nextChannel);
                                            }


                                            foreach (var nextChannelStatusList in nextChannelList.Select(x => x.ChannelStatuses))
                                            {
                                                Mapper.CreateMap<ChannelStatus, ChannelStatusEntity>();
                                                IEnumerable<ChannelStatusEntity> oChannelStatusEntity = Mapper.Map<IEnumerable<ChannelStatus>, IEnumerable<ChannelStatusEntity>>(nextChannelStatusList);

                                                foreach (ChannelStatusEntity nextChannelStatus in oChannelStatusEntity)
                                                {
                                                    oLoginObject.ChannelStatus.Add(nextChannelStatus);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
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
                            response = Request.CreateResponse(HttpStatusCode.NotFound, msg, Configuration.Formatters.JsonFormatter);//failure
                            unitOfWork.Rollback();
                        }
                        //response = Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(oRootObject), new JsonMediaTypeFormatter(), "application/json");//success
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


