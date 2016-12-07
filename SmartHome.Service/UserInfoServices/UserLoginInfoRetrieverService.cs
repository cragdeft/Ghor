using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Models;
using SmartHome.Model.ViewModels;
using SmartHome.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service.UserInfoServices
{
    public class UserLoginInfoRetrieverService : IUserInfoRetrieverService<HomeViewModel>
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<UserInfo> _usreInfoRepository;

        public dynamic _userEntity { get; private set; }

        #endregion
        public UserLoginInfoRetrieverService(IUnitOfWorkAsync unitOfWorkAsync, dynamic userEntity)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _userEntity = userEntity;
            _usreInfoRepository = _unitOfWorkAsync.RepositoryAsync<UserInfo>();
        }

        public HomeViewModel RetriveUserData()
        {
            UserInfo dbUser = new UserInfoService(_unitOfWorkAsync).GetsUserInfosByEmailAndPassword(Convert.ToString(_userEntity.Email), Convert.ToString(_userEntity.Password));

            if (dbUser != null)
            {
                HomeViewModel homeViewModel = new ConfigurationParserManagerService(_unitOfWorkAsync).GetsHomesAllInfo(dbUser.UserInfoId);
                return homeViewModel;
            }

            return null;
        }
    }
}
