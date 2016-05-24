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
using Repository.Pattern.UnitOfWork;
using System.Data.Entity;

namespace SmartHome.Service
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IRepositoryAsync<UserInfo> _userInfoRepository;
        private readonly IRepositoryAsync<Channel> _channelRepository;
        private readonly IRepositoryAsync<WebPagesRole> _webPagesRoleRepository;

        public UserInfoService(IUnitOfWorkAsync unitOfWork)
        {
            _userInfoRepository = unitOfWork.RepositoryAsync<UserInfo>();
            _channelRepository = unitOfWork.RepositoryAsync<Channel>();
            _webPagesRoleRepository = unitOfWork.RepositoryAsync<WebPagesRole>();
        }
        public IEnumerable<UserInfo> GetsUserInfos()
        {
            return _userInfoRepository.Query().Select();
        }
        public IEnumerable<UserInfo> GetsUserInfos(string username, string password)
        {
            return _userInfoRepository.Query(p => p.UserName == username && p.Password == password).Include(x=>x.UserHomeLinks).Select();

        }
        public bool IsLoginIdUnique(string email)
        {
            return _userInfoRepository.Queryable().Any(p => p.Email == email);
        }
        public bool IsValidLogin(string email, string pass)
        {
            return _userInfoRepository.Query(p => p.Email == email && p.Password == pass).Select().Count() == 0 ? false : true;
        }
        public IEnumerable<UserInfo> GetUserInfos(string email, string pass)
        {
            var tempCha = _channelRepository.Queryable().Include(x => x.ChannelStatuses).ToList();

            var temp=_userInfoRepository.Queryable().Where(x => x.Email == email && x.Password == pass)
                .Include(x => x.UserHomeLinks.Select(y => y.Home.SmartRouterInfoes))
                .Include(x => x.UserHomeLinks.Select(y => y.Home))
                .Include(x => x.UserRoomLinks.Select(y => y.Room.SmartDevices.Select(p => p.DeviceStatus))).ToList();

            return temp;
        }
        public UserInfoEntity Add(UserInfoEntity entity)
        {
            Mapper.CreateMap<UserInfoEntity, Model.Models.UserInfo>()
            .ForMember(dest => dest.DateOfBirth, opt => opt.UseValue(DateTime.Now))
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state                                                                                         
            Model.Models.UserInfo model = Mapper.Map<Entity.UserInfoEntity, Model.Models.UserInfo>(entity);

            _userInfoRepository.Insert(model);
            return entity;
            
        }
        public IEnumerable<WebPagesRole> GetsWebPagesRoles()
        {
            return _webPagesRoleRepository.Query().Select();
        }
    }
}
