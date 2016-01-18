using AutoMapper;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.ModelDataContext;
using SmartHome.Model.Models;
using SmartHome.Repository.Repositories;
using SmartHome.Service.Interfaces;
using SmartHome.Utility.EncriptionAndDecryption;
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

        [Route("api/IsValidUser/{userName}")]
        public HttpResponseMessage Get(string userName)
        {
            var userInfos = _userInfoService.UserValidatyCheckByUserName(userName);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, userInfos);
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
    }


}


