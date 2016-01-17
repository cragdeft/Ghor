using Service.Pattern;
using SmartHome.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service.Interfaces
{
    
    public interface IUserInfoService : IService<UserInfo>
    {        
        IEnumerable<UserInfo> GetsUserInfos();
        
    }
}
