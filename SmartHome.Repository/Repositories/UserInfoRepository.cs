﻿using Repository.Pattern.Repositories;
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
    }
}