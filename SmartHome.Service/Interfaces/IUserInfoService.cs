using Service.Pattern;
using SmartHome.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Entity;

namespace SmartHome.Service.Interfaces
{

    public interface IUserInfoService : IService<UserInfo>
    {
        IEnumerable<UserInfo> GetsUserInfos();
        bool IsLoginIdUnique(string email);

        bool IsValidLogin(string email, string pass);
        IEnumerable<UserInfo> GetUserInfos(string email, string pass);
        UserInfoEntity Add(UserInfoEntity model);
    }
}
