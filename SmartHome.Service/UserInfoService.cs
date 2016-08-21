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
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<UserInfo> _userInfoRepository;
        private readonly IRepositoryAsync<Channel> _channelRepository;
        private readonly IRepositoryAsync<WebPagesRole> _webPagesRoleRepository;
        private readonly IRepositoryAsync<UserHomeLink> _userHomeLinkRepository;

        public UserInfoService(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWorkAsync = unitOfWork;
            _userInfoRepository = unitOfWork.RepositoryAsync<UserInfo>();
            _channelRepository = unitOfWork.RepositoryAsync<Channel>();
            _webPagesRoleRepository = unitOfWork.RepositoryAsync<WebPagesRole>();
            _userHomeLinkRepository = unitOfWork.RepositoryAsync<UserHomeLink>();
        }
        public IEnumerable<UserInfo> GetsUserInfos()
        {
            return _userInfoRepository.Query().Select();
        }
        public IEnumerable<UserInfo> GetsUserInfos(string username, string password)
        {
            return _userInfoRepository.Query(p => p.UserName == username && p.Password == password).Include(x => x.UserHomeLinks).Select();
        }

        public IEnumerable<UserInfo> GetsUserInfosByEmail(string email, string password)
        {
            return _userInfoRepository.Query(p => p.Email == email && p.Password == password).Include(x => x.UserHomeLinks).Select();
        }

        public bool IsLoginIdUnique(string email)
        {
            return _userInfoRepository.Queryable().Any(p => p.Email == email);
        }

        public bool PasswordUpdate(string email, string password)
        {


            _unitOfWorkAsync.BeginTransaction();

            try
            {
                //SaveHomeAndRouter();
                var entity = MapUserInfoProperty(email, password);
                entity.ObjectState = ObjectState.Modified;
                _userInfoRepository.Update(entity);

                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWorkAsync.Rollback();
                return false;
            }

            return true;


        }

        private UserInfo MapUserInfoProperty(string email, string password)
        {
            UserInfo model = _userInfoRepository.Queryable().Where(p => p.Email == email).FirstOrDefault();
            model.AuditField.LastUpdatedBy = "admin";
            model.AuditField.LastUpdatedDateTime = DateTime.Now;
            model.Password = password;
            return model;
        }


        public string PasswordRecoveryByEmail(string email)
        {
            var tempPass = _userInfoRepository.Queryable()
                    .Where(p => p.Email == email)
                    .FirstOrDefault();

            return tempPass != null ? tempPass.Password : string.Empty;
        }

        public bool IsValidLogin(string email, string pass)
        {
            return _userInfoRepository.Query(p => p.Email == email && p.Password == pass).Select().Count() == 0 ? false : true;
        }
        public IEnumerable<UserInfo> GetUserInfos(string email, string pass)
        {
            var tempCha = _channelRepository.Queryable().Include(x => x.ChannelStatuses).ToList();

            var temp = _userInfoRepository.Queryable().Where(x => x.Email == email && x.Password == pass)
                .Include(x => x.UserHomeLinks.Select(y => y.Home.SmartRouterInfos))
                .Include(x => x.UserHomeLinks.Select(y => y.Home))
                .Include(x => x.UserRoomLinks.Select(y => y.Room.SmartDevices.Select(p => p.DeviceStatus))).ToList();

            return temp;
        }
        public UserInfoEntity Add(UserInfoEntity entity)
        {
            Mapper.CreateMap<UserInfoEntity, UserInfo>()
            //.ForMember(dest => dest.DateOfBirth, opt => opt.UseValue(DateTime.Now))
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields("Admin", DateTime.Now, "Admin", DateTime.Now)))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state                                                                                         
            UserInfo model = Mapper.Map<UserInfoEntity, UserInfo>(entity);

            _userInfoRepository.Insert(model);
            return entity;

        }
        public IEnumerable<WebPagesRole> GetsWebPagesRoles()
        {
            return _webPagesRoleRepository.Query().Select();
        }
    }
}
