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
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IUserInfoService _userInfoService;


        public UserInfoController(IUnitOfWorkAsync unitOfWorkAsync, IUserInfoService userInfoService)
        {
            this._unitOfWorkAsync = unitOfWorkAsync;
            this._userInfoService = userInfoService;
        }

        [Route("api/Userinfos")]
        public HttpResponseMessage Get()
        {
            Mapper.CreateMap<UserInfo, UserInfoEntity>();
            var userInfos = _userInfoService.GetsUserInfos();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, SecurityManager.Encrypt(Mapper.Map<IEnumerable<UserInfo>, IEnumerable<UserInfoEntity>>(userInfos).ToString()));
            return response;
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
            HttpResponseMessage response;

            var isEmailExists = _userInfoService.IsLoginIdUnique(userInfo.Email);
            if (!isEmailExists)
            {
                _unitOfWorkAsync.BeginTransaction();
                try
                {
                    _userInfoService.Add(userInfo);
                    var changes = _unitOfWorkAsync.SaveChangesAsync();
                    _unitOfWorkAsync.Commit();
                }
                catch (Exception)
                {
                    _unitOfWorkAsync.Rollback();
                }
                response = Request.CreateResponse(HttpStatusCode.OK, userInfo);
            }
            else
                response = Request.CreateResponse(HttpStatusCode.OK, true);

            return response;
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
            UserInfo oUserInfo = new UserInfo();
            HttpResponseMessage response;
            email = email + ".com";

            var isEmailExists = _userInfoService.IsValidLogin(email, pass);
            if (isEmailExists)
            {
                _unitOfWorkAsync.BeginTransaction();
                try
                {
                    //oUserInfo = _userInfoService.GetUserInfos(email, pass);
                    _unitOfWorkAsync.Commit();
                }
                catch (Exception ex)
                {
                    _unitOfWorkAsync.Rollback();
                }
                response = Request.CreateResponse(HttpStatusCode.OK, oUserInfo);
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "User not found");
            }

            return response;
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


            try
            {
                var isEmailExists = _userInfoService.IsLoginIdUnique(oUserInfo.Email);
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
                    _unitOfWorkAsync.BeginTransaction();
                    try
                    {
                        _userInfoService.Add(oUserInfo);
                        var changes = _unitOfWorkAsync.SaveChanges();
                        _unitOfWorkAsync.Commit();
                    }
                    catch (Exception ex)
                    {
                        _unitOfWorkAsync.Rollback();
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

        //login test
        [Route("api/CheckUser")]
        [HttpPost]
        public HttpResponseMessage PostUniqueUserByEmail(JObject encryptedString)
        {
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

            var isEmailExists = _userInfoService.IsValidLogin(oUserInfo.Email, oUserInfo.Password);
            if (isEmailExists)
            {
                _unitOfWorkAsync.BeginTransaction();
                try
                {
                    var oUserEntity = _userInfoService.GetUserInfos(oUserInfo.Email, oUserInfo.Password).ToList();
                    Mapper.CreateMap<UserInfo, UserInfoEntity>()
                    .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.Gender))
                    .ForMember(dest => dest.LoginStatus, opt => opt.MapFrom(src => src.LoginStatus == true ? 1 : 0));

                    IEnumerable<UserInfoEntity> oUserInfoEntity = Mapper.Map<IEnumerable<UserInfo>, IEnumerable<UserInfoEntity>>(oUserEntity);
                    oLoginObject.UserInfo = new List<UserInfoEntity>();
                    oLoginObject.UserInfo.Add(oUserInfoEntity.First());


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



                    _unitOfWorkAsync.Commit();
                }
                catch (Exception ex)
                {

                    oRootObject.data = new LoginObjectEntity();
                    LoginMessage oLoginMessage = SetLoginMessage(ex.ToString(), HttpStatusCode.BadRequest);
                    oRootObject.MESSAGE = new LoginMessage();
                    oRootObject.MESSAGE = oLoginMessage;
                    msg = JsonConvert.SerializeObject(oRootObject);
                    response = Request.CreateResponse(HttpStatusCode.NotFound, msg, Configuration.Formatters.JsonFormatter);//failure

                    _unitOfWorkAsync.Rollback();


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
                response = Request.CreateResponse(HttpStatusCode.NotFound, msg);//failure
            }
            return response;
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


