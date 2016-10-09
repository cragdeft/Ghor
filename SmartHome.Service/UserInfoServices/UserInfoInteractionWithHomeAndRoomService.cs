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

namespace SmartHome.Service.UserInfoServices
{
    public class UserInfoInteractionWithHomeAndRoomService : IHomeJsonParserService<UserInfo>
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

        public UserInfoInteractionWithHomeAndRoomService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _userRepository = _unitOfWorkAsync.RepositoryAsync<UserInfo>();
            _userRoomLinkRepository = _unitOfWorkAsync.RepositoryAsync<UserRoomLink>();
            _userHomeRepository = _unitOfWorkAsync.RepositoryAsync<UserHomeLink>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
        }
        public void SaveHomeUser(Home home, UserInfo userInfo)
        {
            var homeUserList = _homeJsonEntity.UserHomeLink.FindAll(x => x.AppsHomeId == home.AppsHomeId);

            foreach (var userRoomLinkEntity in homeUserList)
            {
                UserHomeLink userHome = new UserHomeLink();
                userHome.UserInfo = userInfo;

                FillSaveHomeUser(home, userRoomLinkEntity, userHome);
            }
        }
        public void SaveRoomUser(Home home, UserInfo userInfo)
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
        public void FillSaveRoomUser(Room entity, UserRoomLinkEntity userRoomLinkEntity, UserRoomLink userRoom)
        {
            userRoom.Room = entity;
            userRoom.IsSynced = Convert.ToBoolean(userRoomLinkEntity.IsSynced);
            userRoom.AppsUserRoomLinkId = userRoomLinkEntity.AppsUserRoomLinkId;
            userRoom.AppsRoomId = userRoomLinkEntity.AppsRoomId;
            userRoom.AppsUserId = userRoomLinkEntity.AppsUserId;
            userRoom.ObjectState = ObjectState.Added;
            _userRoomLinkRepository.Insert(userRoom);
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


        public void SaveRoomUserFromJson(Room entity, IList<UserInfo> listOfUsers)
        {
            var roomLinkList = _homeJsonEntity.UserRoomLink.FindAll(x => x.AppsRoomId == entity.AppsRoomId);

            foreach (var userRoomLinkEntity in roomLinkList)
            {
                UserInfoEntity userentity = _homeJsonEntity.UserInfo.Find(x => x.AppsUserId.ToString() == userRoomLinkEntity.AppsUserId.ToString());

                UserRoomLink userRoom = new UserRoomLink();
                userRoom.UserInfo = listOfUsers.Where(u => u.Email == userentity.Email).FirstOrDefault();

                FillSaveRoomUser(entity, userRoomLinkEntity, userRoom);
            }
        }


        public void SaveRoomForExistingDbUser(Room entity, IList<UserInfo> listOfExistingDbUsers)
        {
            var roomLinkList = _homeJsonEntity.UserRoomLink.FindAll(x => x.AppsRoomId == entity.AppsRoomId);
            foreach (var userRoomLinkEntity in roomLinkList)
            {
                foreach (var dbUser in listOfExistingDbUsers)
                {
                    UserRoomLink userRoom = new UserRoomLink();
                    userRoom.UserInfo = dbUser;

                    FillSaveRoomUser(entity, userRoomLinkEntity, userRoom);
                }

            }
        }

        public void DeleteRoomUser(UserInfo userInfo)
        {
            IList<UserRoomLink> dbRoomUser = _userRoomLinkRepository.Queryable().Where(p => p.UserInfo.UserInfoId == userInfo.UserInfoId).ToList();

            foreach (var roomUser in dbRoomUser)
            {
                roomUser.ObjectState = ObjectState.Deleted;
                _userRoomLinkRepository.Delete(roomUser);
            }
        }
        public void DeleteHomeUser(UserInfo userInfo)
        {
            IList<UserHomeLink> dbHomeUser = _userHomeRepository.Queryable().Where(p => p.UserInfo.UserInfoId == userInfo.UserInfoId).ToList();
            foreach (var usreHome in dbHomeUser)
            {
                usreHome.ObjectState = ObjectState.Deleted;
                _userHomeRepository.Delete(usreHome);
            }

        }
        public IList<UserInfo> DeleteDBRemainingUsersNotInJson(Home home, IList<UserInfo> listOfUsersFromJson)
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

        public void SaveHomeUsers(Home home, IList<UserInfo> listOfUsers, IList<UserInfo> listOfExistingDbUsers)
        {
            var homeUserList = _homeJsonEntity.UserHomeLink.FindAll(x => x.AppsHomeId == home.AppsHomeId);

            foreach (var userRoomLinkEntity in homeUserList)
            {
                UserInfoEntity userentity =
                    _homeJsonEntity.UserInfo.Find(x => x.AppsUserId == userRoomLinkEntity.AppsUserId);
                UserInfo UserInfo = listOfUsers.Where(u => u.Email == userentity.Email).FirstOrDefault();
                SaveHomeUser(home, UserInfo);
            }
            if (listOfExistingDbUsers.Count > 0)
            {
                SaveHomeForExistingDbUsers(home, listOfExistingDbUsers, homeUserList);
            }

        }


        public UserInfo SaveJsonData()
        {
            throw new NotImplementedException();
        }


    }
}
