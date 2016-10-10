using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.Models;
using SmartHome.Service.UserInfoServices;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SmartHome.Service
{
    public class UserInfoDeleteJsonParserService : IHomeDeleteJsonParserService<UserInfo>
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

        public UserInfo DeleteJsonData()
        {
            UserInfo userInfo = null;
            try
            {
                userInfo = DeleteUser();
            }
            catch (Exception ex)
            {
                return null;
            }

            return userInfo;
        }
        private UserInfo DeleteUser()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            string email = _homeJsonEntity.UserInfo.FirstOrDefault().Email;

            UserInfo userInfo = new CommonService(_unitOfWorkAsync).GetUserByEmail(email);
            if (userInfo != null)
            {
                new UserInfoInteractionWithHomeAndRoomService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).DeleteHomeUser(userInfo);
                new UserInfoInteractionWithHomeAndRoomService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).DeleteRoomUser(userInfo);
                return DeleteUser(userInfo);
            }
            return null;
        }
        private UserInfo DeleteUser(UserInfo userInfo)
        {
            userInfo.ObjectState = ObjectState.Deleted;
            _userRepository.Delete(userInfo);
            return userInfo;
        }
    }
}
