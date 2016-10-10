using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.Models;
using SmartHome.Service.UserInfoServices;
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
                userInfo = UpdateUserInfos();
            }
            catch (Exception ex)
            {
                return null;
            }
            return userInfo;
        }

        private UserInfo UpdateUserInfos()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            string email = _homeJsonEntity.UserInfo.FirstOrDefault().Email;
            Home home = null;

            UserInfo dbUser = new CommonService(_unitOfWorkAsync).GetUserByEmail(email);
            home = new CommonService(_unitOfWorkAsync).GetHomeWithRooms(passPhrase);

            UserInfo entity = UpdateUser(_homeJsonEntity.UserInfo.FirstOrDefault(), dbUser);

            new UserInfoInteractionWithHomeAndRoomService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).SaveHomeUser(home, entity);
            new UserInfoInteractionWithHomeAndRoomService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).SaveRoomUser(home, entity);

            return entity;
        }

        public UserInfo UpdateUser(UserInfoEntity userInfoEntity, UserInfo dbUser)
        {
            UserInfo entity = SmartHomeTranslater.MapUserInfoProperties(userInfoEntity, dbUser);
            entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            entity.ObjectState = ObjectState.Modified;
            _userRepository.Update(entity);

            new UserInfoInteractionWithHomeAndRoomService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).DeleteHomeUser(entity);
            new UserInfoInteractionWithHomeAndRoomService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).DeleteRoomUser(entity);
            return entity;
        }

        private void SetMapper()
        {
            Mapper.CreateMap<UserInfoEntity, UserInfo>();
        }
    }
}
