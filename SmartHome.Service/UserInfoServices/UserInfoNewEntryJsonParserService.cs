using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.Models;
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
            //SetMapper();
            try
            {
                userInfo=SaveNewUser();
            }
            catch (Exception ex)
            {
                _unitOfWorkAsync.Rollback();
                return null;
            }
            return userInfo;
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
        private UserInfo SaveNewUser()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            Home home = null;
            home = new CommonService(_unitOfWorkAsync).GetHomeWithRooms(passPhrase);
            if (home != null)
            {
                UserInfo userInfo = InsertUser(_homeJsonEntity.UserInfo[0]);
                SaveRoomUser(home, userInfo);
                SaveHomeUser(home, userInfo);
                return userInfo;
            }
            return null;
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
        public void FillSaveHomeUser(Home home, UserHomeLinkEntity userRoomLinkEntity, UserHomeLink userHome)
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

        public void SaveHomeForExistingDbUsers(Home home, IList<UserInfo> listOfExistingDbUsers, List<UserHomeLinkEntity> homeUserList)
        {
            foreach (var userRoomLinkEntity in homeUserList)
            {
                foreach (var dbUser in listOfExistingDbUsers)
                {
                    UserHomeLink userHome = new UserHomeLink();
                    userHome.UserInfo = dbUser;

                    FillSaveHomeUser(home, userRoomLinkEntity, userHome);
                }
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
        private void SetMapper()
        {
            // Mapper.CreateMap<HomeEntity, Home>();
            //  Mapper.CreateMap<RoomEntity, Room>();
            Mapper.CreateMap<UserInfoEntity, UserInfo>();
        }

        public IList<UserInfo> DeleteUser(Home home, IList<UserInfo> listOfUsersFromJson)
        {

            IList<UserInfo> listOfExistingDbUser = new List<UserInfo>();

            IList<long> userIdsFromJson = (from u in listOfUsersFromJson
                                           select u.UserInfoId).ToList();

            IList<long> needToDeleteUserIds =
                _userHomeRepository.Queryable()
                    .Where(w => w.Home.HomeId == home.HomeId && !userIdsFromJson.Contains(w.UserInfo.UserInfoId))
                    .Select(s => s.UserInfo.UserInfoId)
                    .ToList();

            foreach (long id in needToDeleteUserIds)
            {
                UserInfo user =
                    _userRepository.Queryable()
                        .Include(x => x.UserRoomLinks)
                        .Include(x => x.UserHomeLinks)
                        .Where(u => u.UserInfoId == id)
                        .FirstOrDefault();

                if (user != null)
                {
                    DeleteRoomUser(user);
                    DeleteHomeUser(user);
                    if (_receivedFrom == MessageReceivedFrom.MQTT)
                    {
                        user.ObjectState = ObjectState.Deleted;
                        _userRepository.Delete(user);
                    }
                    else
                    {
                        listOfExistingDbUser.Add(user);
                    }
                }
            }
            return listOfExistingDbUser;

        }

        private void DeleteHomeUser(UserInfo entity)
        {
            IList<long> userHomeLinkIds = entity.UserHomeLinks.Select(s => s.UserHomeLinkId).ToList();
            foreach (long id in userHomeLinkIds)
            {
                UserHomeLink userHomeLink = _userHomeRepository.Find(id);
                userHomeLink.ObjectState = ObjectState.Deleted;
                _userHomeRepository.Delete(userHomeLink);
            }
        }
        private void DeleteRoomUser(UserInfo entity)
        {
            IList<long> userHomeLinkIds = entity.UserRoomLinks.Select(s => s.UserRoomLinkId).ToList();
            foreach (long userRoomLinkid in userHomeLinkIds)
            {
                UserRoomLink userRoomLink = _userRoomLinkRepository.Find(userRoomLinkid);
                userRoomLink.ObjectState = ObjectState.Deleted;
                _userRoomLinkRepository.Delete(userRoomLink);
            }
        }
    }
}
