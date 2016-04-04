using AutoMapper;
using Newtonsoft.Json;
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

        [Route("api/GetRegisterUser/{userInfo}")]
        public HttpResponseMessage GetUniqueUserByEmail(string userInfo)
        {
            RootObjectEntity oRootObject = JsonConvert.DeserializeObject<RootObjectEntity>(userInfo);
            var oUserInfo = oRootObject.UserInfo.First();
            oUserInfo.Email = oUserInfo.Email + ".com";
            HttpResponseMessage response;

            var isEmailExists = _userInfoService.IsLoginIdUnique(oUserInfo.Email);
            if (isEmailExists)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Already registered.");

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
                response = Request.CreateResponse(HttpStatusCode.OK, "Unique");
            }


            return response;
        }



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
                    oUserInfo = _userInfoService.GetUserInfos(email, pass);
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

            return response ;
        }
    }
}


