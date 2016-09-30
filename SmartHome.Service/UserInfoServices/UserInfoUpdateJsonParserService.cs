﻿using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.Models;
using SmartHome.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{
    public class UserInfoUpdateJsonParserService : IHomeUpdateJsonParserService<UserInfo>
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

        public UserInfoUpdateJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _userRepository = _unitOfWorkAsync.RepositoryAsync<UserInfo>();
            _userRoomLinkRepository = _unitOfWorkAsync.RepositoryAsync<UserRoomLink>();
            _userHomeRepository = _unitOfWorkAsync.RepositoryAsync<UserHomeLink>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
        }
        public UserInfo UpdateJsonData()
        {
            UserInfo userInfo = null;
            SetMapper();
            try
            {
                userInfo=UpdateUserInfos();
            }
            catch (Exception ex)
            {
                return null;
            }
            return userInfo;
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
        private UserInfo UpdateUserInfos()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            string email = _homeJsonEntity.UserInfo.FirstOrDefault().Email;
            Home home = null;

            UserInfo dbUser = new CommonService(_unitOfWorkAsync).GetUserByEmail(email);
            home = new CommonService(_unitOfWorkAsync).GetHomeWithRooms(passPhrase);

            UserInfo entity = UpdateUser(dbUser);

            DeleteHomeUser(entity);
            DeleteRoomUser(entity);

            SaveHomeUser(home, entity);
            SaveRoomUser(home, entity);

            return entity;

        }
        private void DeleteHomeUser(UserInfo userInfo)
        {
            var dbHomeUser = _userHomeRepository.Queryable().Where(p => p.UserInfo.AppsUserId == userInfo.AppsUserId).FirstOrDefault();
            dbHomeUser.ObjectState = ObjectState.Deleted;
            _userHomeRepository.Delete(dbHomeUser);
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
        private UserInfo UpdateUser(UserInfo dbUser)
        {
            UserInfo entity = SmartHomeTranslater.MapUserInfoProperties(_homeJsonEntity.UserInfo.FirstOrDefault(), dbUser);
            entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            entity.ObjectState = ObjectState.Modified;
            _userRepository.Update(entity);
            return entity;
        }
        private void SaveHomeUser(Home home, UserInfo userInfo)
        {
            var homeUserList = _homeJsonEntity.UserHomeLink.FindAll(x => x.AppsHomeId == home.AppsHomeId);

            foreach (var userRoomLinkEntity in homeUserList)
            {
                UserHomeLink userHome = new UserHomeLink();
                userHome.UserInfo = userInfo;

                FillSaveHomeUser(home, userRoomLinkEntity, userHome);
            }
        }
        private void SaveRoomUser(Home home, UserInfo userInfo)
        {
            foreach (var userRoomLinkEntity in _homeJsonEntity.UserRoomLink)
            {
                Room dbroom = home.Rooms.Where(p => p.AppsRoomId == userRoomLinkEntity.AppsRoomId).FirstOrDefault();
                if (dbroom != null)
                {
                    UserRoomLink userRoom = new UserRoomLink();
                    userRoom.UserInfo = userInfo;
                    FillSaveRoomUser(dbroom, userRoomLinkEntity, userRoom);
                }
            }
        }
        private void FillSaveRoomUser(Room entity, UserRoomLinkEntity userRoomLinkEntity, UserRoomLink userRoom)
        {
            userRoom.Room = entity;
            userRoom.IsSynced = Convert.ToBoolean(userRoomLinkEntity.IsSynced);
            userRoom.AppsUserRoomLinkId = userRoomLinkEntity.AppsUserRoomLinkId;
            userRoom.AppsRoomId = userRoomLinkEntity.AppsRoomId;
            userRoom.AppsUserId = userRoomLinkEntity.AppsUserId;
            userRoom.ObjectState = ObjectState.Added;
            _userRoomLinkRepository.Insert(userRoom);
        }
        private void FillSaveHomeUser(Home home, UserHomeLinkEntity userRoomLinkEntity, UserHomeLink userHome)
        {
            userHome.Home = home;
            userHome.IsSynced = Convert.ToBoolean(userRoomLinkEntity.IsSynced);
            userHome.AppsUserHomeLinkId = userRoomLinkEntity.AppsUserHomeLinkId;
            userHome.AppsHomeId = userRoomLinkEntity.AppsHomeId;
            userHome.AppsUserId = userRoomLinkEntity.AppsUserId;
            userHome.IsAdmin = Convert.ToBoolean(userRoomLinkEntity.IsAdmin);
            userHome.ObjectState = ObjectState.Added;
            _userHomeRepository.Insert(userHome);
        }
        private void SetMapper()
        {
            Mapper.CreateMap<UserInfoEntity, UserInfo>();
        }
    }
}