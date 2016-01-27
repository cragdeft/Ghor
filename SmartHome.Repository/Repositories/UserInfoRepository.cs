using Repository.Pattern.Repositories;
using SmartHome.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Repository.Repositories
{
    public static class UserInfoRepository
    {
        public static IEnumerable<UserInfo> GetsUserInfos(this IRepositoryAsync<UserInfo> repository)
        {
            return repository
                .Queryable()
                .AsEnumerable();
        }

        public static bool UserValidatyCheckByUserName(this IRepositoryAsync<UserInfo> repository, string userName)
        {
            return repository
                .Queryable()
                .Where(u => u.UserName == userName)
                .AsEnumerable().Count() == 0 ? false : true;
        }

        public static bool IsLoginIdUnique(this IRepositoryAsync<UserInfo> repository, string email)
        {
            return repository
                .Queryable()
                .Where(u => u.Email == email)
                .AsEnumerable().Count() == 0 ? false : true;
        }
    }
}
