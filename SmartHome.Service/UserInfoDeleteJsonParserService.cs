using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.Models;
using System;
using System.Linq;

namespace SmartHome.Service
{
    class UserInfoDeleteJsonParserService : IHomeDeleteJsonParserService
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<UserInfo> _userRepository;
        private readonly IRepositoryAsync<UserRoomLink> _userRoomLinkRepository;
        private readonly IRepositoryAsync<UserHomeLink> _userHomeRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }

        #endregion

        public UserInfoDeleteJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _userRepository = _unitOfWorkAsync.RepositoryAsync<UserInfo>();
            _userRoomLinkRepository = _unitOfWorkAsync.RepositoryAsync<UserRoomLink>();
            _userHomeRepository = _unitOfWorkAsync.RepositoryAsync<UserHomeLink>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
        }

        public bool DeleteJsonData()
        {
            MessageLog messageLog = new CommonService(_unitOfWorkAsync).SaveMessageLog(_homeJsonMessage, _receivedFrom);

            _unitOfWorkAsync.BeginTransaction();
            try
            {
                DeleteUser();
                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWorkAsync.Rollback();
                return false;
            }

            new CommonService(_unitOfWorkAsync).UpdateMessageLog(messageLog, _homeJsonEntity.Home[0].PassPhrase);

            return true;
        }
        private void DeleteUser()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            //int appsUserId = _homeJsonEntity.UserInfo.FirstOrDefault().AppsUserId;
            string email = _homeJsonEntity.UserInfo.FirstOrDefault().Email;

            UserInfo userInfo = GetUserInfo(email);
            if (userInfo != null)
            {
                DeleteHomeUser(userInfo);
                DeleteRoomUser(userInfo);
                DeleteUser(userInfo);
            }
        }
        private void DeleteUser(UserInfo userInfo)
        {
            userInfo.ObjectState = ObjectState.Deleted;
            _userRepository.Delete(userInfo);
        }
        private void DeleteRoomUser(UserInfo userInfo)
        {
            var dbRoomUser = _userRoomLinkRepository.Queryable().Where(p => p.UserInfo.AppsUserId == userInfo.AppsUserId);
            foreach (var roomUser in dbRoomUser)
            {
                roomUser.ObjectState = ObjectState.Deleted;
                _userRoomLinkRepository.Delete(roomUser);
            }
        }
        private void DeleteHomeUser(UserInfo userInfo)
        {
            var dbHomeUser = _userHomeRepository.Queryable().Where(p => p.UserInfo.AppsUserId == userInfo.AppsUserId).FirstOrDefault();
            dbHomeUser.ObjectState = ObjectState.Deleted;
            _userHomeRepository.Delete(dbHomeUser);
        }
        private UserInfo GetUserInfo(string email)
        {
            return _userRepository.Queryable().Where(p => p.Email == email).FirstOrDefault();
        }
        private UserInfo InsertUser(UserInfoEntity userInfoEntity)
        {
            UserInfo entity = Mapper.Map<UserInfoEntity, UserInfo>(userInfoEntity);
            entity.LoginStatus = Convert.ToBoolean(userInfoEntity.LoginStatus);
            entity.RegStatus = Convert.ToBoolean(userInfoEntity.RegStatus);
            entity.IsSynced = Convert.ToBoolean(userInfoEntity.IsSynced);
            entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            entity.ObjectState = ObjectState.Added;
            _userRepository.Insert(entity);
            return entity;
        }
    }
}
