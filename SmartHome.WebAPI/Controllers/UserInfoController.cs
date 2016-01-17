using AutoMapper;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.ModelDataContext;
using SmartHome.Model.Models;
using SmartHome.Repository.Repositories;
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
        // GET api/ptemployees
        [Route("api/Userinfos")]
        public HttpResponseMessage Get()
        {
            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                Mapper.CreateMap<UserInfo, UserInfoEntity>();
                IRepositoryAsync<UserInfo> userInfoRepository = new Repository<UserInfo>(context, unitOfWork);
                var userInfos = userInfoRepository.GetsUserInfos();                
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<IEnumerable<UserInfo>, IEnumerable<UserInfoEntity>>(userInfos));
                return response;
            }
        }
    }
}
