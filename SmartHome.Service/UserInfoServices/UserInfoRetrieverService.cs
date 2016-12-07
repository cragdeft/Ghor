using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Model.Models;
using SmartHome.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service.UserInfoServices
{
    public class UserInfoRetrieverService : IUserInfoRetrieverService<UserInfo>
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<UserInfo> _usreInfoRepository;

        public dynamic _userEntity { get; private set; }

        #endregion
        public UserInfoRetrieverService(IUnitOfWorkAsync unitOfWorkAsync, dynamic userEntity)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _userEntity = userEntity;

            _usreInfoRepository = _unitOfWorkAsync.RepositoryAsync<UserInfo>();
        }

        public UserInfo RetriveUserData()
        {
            bool isValalidUser = new CommonService(_unitOfWorkAsync).IsUserExist(Convert.ToString(_userEntity.Email));
            if (isValalidUser)
            {
                return new CommonService(_unitOfWorkAsync).GetUserByEmail(Convert.ToString(_userEntity.Email));
            }
            return null;
        }
    }
}
