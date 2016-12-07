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
    public class UserUniquenessCheckService : IUserInfoRetrieverService<UserInfo>
    {

        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<UserInfo> _usreInfoRepository;

        public dynamic _userEntity { get; private set; }

        #endregion
        public UserUniquenessCheckService(IUnitOfWorkAsync unitOfWorkAsync, dynamic userEntity)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _userEntity = userEntity;
            _usreInfoRepository = _unitOfWorkAsync.RepositoryAsync<UserInfo>();
        }

        //public bool IsLoginIdUnique(string email)
        //{
        //    return !_usreInfoRepository.Queryable().Any(p => p.Email == email);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns>when usre in unique  return new user object </returns>
        public UserInfo RetriveUserData()
        {


            if (!new CommonService(_unitOfWorkAsync).IsUserExist(Convert.ToString(_userEntity.Email)))
            {
                return new UserInfo();
            }
            return null;
        }
    }
}
