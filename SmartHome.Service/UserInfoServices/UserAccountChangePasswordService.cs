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
    public class UserAccountChangePasswordService : IUserInfoModificationService<UserInfo>
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<UserInfo> _usreInfoRepository;

        public dynamic _userEntity { get; private set; }

        #endregion
        public UserAccountChangePasswordService(IUnitOfWorkAsync unitOfWorkAsync, dynamic userEntity)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _userEntity = userEntity;
            _usreInfoRepository = _unitOfWorkAsync.RepositoryAsync<UserInfo>();
        }

        public UserInfo UpdateData()
        {
            bool isValidUser = new CommonService(_unitOfWorkAsync).IsUserExist(Convert.ToString(_userEntity.Email));
            if (isValidUser)
            {
                new CommonService(_unitOfWorkAsync).PasswordUpdate(Convert.ToString(_userEntity.Email), Convert.ToString(_userEntity.Password));
                return new UserInfo();
            }
            return null;

        }
    }
}
