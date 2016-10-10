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
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{
    public class UserInfoNewEntryJsonParserService : IHomeJsonParserService<UserInfo>
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

        public UserInfoNewEntryJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _userRepository = _unitOfWorkAsync.RepositoryAsync<UserInfo>();
            _userRoomLinkRepository = _unitOfWorkAsync.RepositoryAsync<UserRoomLink>();
            _userHomeRepository = _unitOfWorkAsync.RepositoryAsync<UserHomeLink>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
            SetMapper();
        }
        public UserInfo SaveJsonData()
        {
            UserInfo userInfo = null;
            try
            {
                userInfo = SaveNewUser();
            }
            catch (Exception ex)
            {
                _unitOfWorkAsync.Rollback();
                return null;
            }
            return userInfo;
        }
        private UserInfo SaveNewUser()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            Home home = null;
            home = new CommonService(_unitOfWorkAsync).GetHomeWithRooms(passPhrase);
            if (home != null)
            {
                UserInfo userInfo = InsertUser(_homeJsonEntity.UserInfo[0]);

                new UserInfoInteractionWithHomeAndRoomService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).SaveRoomUser(home, userInfo);
                new UserInfoInteractionWithHomeAndRoomService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).SaveHomeUser(home, userInfo);

                return userInfo;
            }
            return null;
        }
        public UserInfo InsertUser(UserInfoEntity userInfoEntity)
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
        private void SetMapper()
        {
            Mapper.CreateMap<UserInfoEntity, UserInfo>();
        }


    }
}
