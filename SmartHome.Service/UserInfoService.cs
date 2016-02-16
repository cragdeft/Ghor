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
using AutoMapper;
using Repository.Pattern.Infrastructure;
using SmartHome.Entity;

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
        public bool IsLoginIdUnique(string email)
        {
            return _repository.IsLoginIdUnique(email);
        }

        public UserInfoEntity Add(UserInfoEntity entity)
        {
            UserInfo model = Mapper.Map<UserInfoEntity, UserInfo>(entity);
            model.AuditField = new AuditFields();
            model.ObjectState = ObjectState.Added;
            base.Insert(model);
            return entity;
        }
    }
}
