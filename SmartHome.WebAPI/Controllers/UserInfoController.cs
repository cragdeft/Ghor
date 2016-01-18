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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<IEnumerable<UserInfo>, IEnumerable<UserInfoEntity>>(userInfos));
            return response;
        }

        [Route("api/IsValidUser/{userName}")]
        public HttpResponseMessage Get(string userName)
        {
            var userInfos = _userInfoService.UserValidatyCheckByUserName(userName);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, userInfos);
            return response;
        }

        //[Route("api/Userinfos")]
        //public HttpResponseMessage Get()
        //{
        //    using (IDataContextAsync context = new SmartHomeDataContext())
        //    using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
        //    {
        //        Mapper.CreateMap<UserInfo, UserInfoEntity>();
        //        IRepositoryAsync<UserInfo> userInfoRepository = new Repository<UserInfo>(context, unitOfWork);
        //        var userInfos = userInfoRepository.GetsUserInfos();
        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<IEnumerable<UserInfo>, IEnumerable<UserInfoEntity>>(userInfos));                
        //        return response;                
        //    }
        //}



        //[Route("api/IsValidUser/{userName}")]
        //public HttpResponseMessage Get(string userName)
        //{
        //    using (IDataContextAsync context = new SmartHomeDataContext())
        //    using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
        //    {
        //        Mapper.CreateMap<UserInfo, UserInfoEntity>();
        //        IRepositoryAsync<UserInfo> userInfoRepository = new Repository<UserInfo>(context, unitOfWork);
        //        var userInfos = userInfoRepository.UserValidatyCheckByUserName(userName);
        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, userInfos);
        //        return response;
        //    }
        //}


    }
}
