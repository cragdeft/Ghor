using Repository.Pattern.Repositories;
using Service.Pattern;
using SmartHome.Model.Models;
using SmartHome.Repository.Repositories;
using SmartHome.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{
    public class UserInfoService : Service<UserInfo>, IUserInfoService
    {
        private readonly IRepositoryAsync<UserInfo> _repository;

        public UserInfoService(IRepositoryAsync<UserInfo> repository) : base(repository)
        {
            _repository = repository;
        }

        public IEnumerable<UserInfo> GetsUserInfos()
        {
            return _repository.GetsUserInfos();
            
        }

        public bool UserValidatyCheckByUserName(string userName)
        {
            return _repository.UserValidatyCheckByUserName(userName);
        }
    }
}
