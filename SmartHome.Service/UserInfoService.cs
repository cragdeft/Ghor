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

        //public UserInfoService(IRepositoryAsync<UserInfo> repository) : base(repository)
        //{
        //    _repository = repository;

        //}       


        public UserInfoService(IUnitOfWorkAsync unitOfWor)
        {
            _userInfoRepository = unitOfWor.RepositoryAsync<UserInfo>();
            _channelRepository = unitOfWor.RepositoryAsync<Channel>();
        }

        public IEnumerable<UserInfo> GetsUserInfos()
        {
            return _userInfoRepository.GetsUserInfos();

        }
        public bool IsLoginIdUnique(string email)
        {
            return _userInfoRepository.IsLoginIdUnique(email);
        }

        public bool IsValidLogin(string email, string pass)
        {
            return _userInfoRepository.Query(p => p.Email == email && p.Password == pass).Select().Count() == 0 ? false : true;
        }

        public IEnumerable<UserInfo> GetUserInfos(string email, string pass)
        {
            var tempCha = _channelRepository.Queryable().Include(x => x.ChannelStatuses).ToList();


            return _userInfoRepository.Queryable().Where(x => x.Email == email && x.Password == pass)
                .Include(x => x.UserHomeLinks.Select(y => y.Home.SmartRouterInfoes))
                .Include(x => x.UserHomeLinks.Select(y => y.Home.Rooms.Select(z => z.SmartDevices.Select(p => p.DeviceStatus))))
                .Include(x => x.UserRooms.Select(y => y.Room)).ToList();

            //return _userInfoRepository.Query(x => x.Email == email && x.Password == pass)
            //    .Include(x => x.UserHomeLinks.Select(y => y.Home))
            //    .Include(x => x.UserHomeLinks.Select(y => y.Home.Rooms.Select(z => z.SmartDevices.Select(p => p.DeviceStatus))))
            //    .Include(x => x.UserRooms.Select(y => y.Room)).Select();
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

    }
}
