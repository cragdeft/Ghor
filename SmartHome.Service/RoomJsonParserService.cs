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
    public class RoomJsonParserService : IHomeJsonParserService
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<Home> _homeRepository;
        private readonly IRepositoryAsync<Room> _roomRepository;
        private readonly IRepositoryAsync<UserInfo> _userRepository;
        private readonly IRepositoryAsync<UserRoomLink> _userRoomLinkRepository;
        private readonly IRepositoryAsync<MessageLog> _mqttMessageLogRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }
        public MessageLog _messageLog { get; private set; }

        #endregion

        public RoomJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Model.Models.Home>();
            _roomRepository = _unitOfWorkAsync.RepositoryAsync<Room>();
            _userRepository = _unitOfWorkAsync.RepositoryAsync<UserInfo>();
            _userRoomLinkRepository = _unitOfWorkAsync.RepositoryAsync<UserRoomLink>();
            _mqttMessageLogRepository = _unitOfWorkAsync.RepositoryAsync<MessageLog>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
            _messageLog = new MessageLog();
        }

        public bool SaveJsonData()
        {
            new HomeJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).SaveMessageLog();

            _unitOfWorkAsync.BeginTransaction();
            SetMapper();
            try
            {
                SaveNewRoom();
                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWorkAsync.Rollback();
                return false;
            }

            new HomeJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).UpdateMessageLog();


            return true;
        }
        private void SetMapper()
        {
            Mapper.CreateMap<RoomEntity, Room>();
        }
        private void SaveNewRoom()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            Home home = null;

            home = GetHome(passPhrase);
            if (home != null)
            {
                UserInfo userInfo = GetUser(_homeJsonEntity.UserInfo[0].Email);
                home = SaveOrUpdateNewRoom(home, userInfo);
            }
        }
        private Home GetHome(string passPhrase)
        {
            Home home = _homeRepository
                .Queryable().Include(x => x.Rooms).Where(u => u.PassPhrase == passPhrase).FirstOrDefault();
            return home;
        }
        private Home SaveOrUpdateNewRoom(Home home, UserInfo userInfo)
        {
            RoomEntity room = _homeJsonEntity.Room[0];

            var entity = Mapper.Map<RoomEntity, Room>(room);
            entity.IsActive = Convert.ToBoolean(room.IsActive);
            entity.IsSynced = Convert.ToBoolean(room.IsSynced);
            entity.Home = home;
            entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            entity.ObjectState = ObjectState.Added;
            _roomRepository.Insert(entity);

            SaveRoomUser(entity, userInfo);

            return home;
        }
        private void SaveRoomUser(Room entity, UserInfo userInfo)
        {
            var roomLinkList = _homeJsonEntity.UserRoomLink.FindAll(x => x.AppsRoomId == entity.AppsRoomId);

            foreach (var userRoomLinkEntity in roomLinkList)
            {
                UserRoomLink userRoom = new UserRoomLink();
                userRoom.UserInfo = userInfo;

                FillSaveRoomUser(entity, userRoomLinkEntity, userRoom);
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
        public UserInfo GetUser(string email)
        {
            UserInfo user = _userRepository
                .Queryable().Include(x => x.UserRoomLinks).Include(x => x.UserHomeLinks).Where(u => u.Email == email).FirstOrDefault();

            return user;
        }

       
    }
}
