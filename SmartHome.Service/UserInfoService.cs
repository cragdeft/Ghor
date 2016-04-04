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

        public bool IsValidLogin(string email, string pass)
        {
            return _repository.Query(p => p.Email == email && p.Password == pass).Select().Count() == 0 ? false : true;
        }

        public UserInfo GetUserInfos(string email, string pass)
        {
            return _repository.Query(x => x.Email == email && x.Password == pass)
                .Include(x => x.UserHomeLinks.Select(y => y.Home))
                .Include(x => x.UserRooms.Select(y => y.Room)).Select().FirstOrDefault();
        }


        public UserInfoEntity Add(UserInfoEntity entity)
        {
            Mapper.CreateMap<UserInfoEntity, Model.Models.UserInfo>()
            .ForMember(dest => dest.DateOfBirth, opt => opt.UseValue(DateTime.Now))
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state                                                                                         
            Model.Models.UserInfo model = Mapper.Map<Entity.UserInfoEntity, Model.Models.UserInfo>(entity);

            base.Insert(model);
            return entity;


        }

    }
}
