using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{
    public class RoomUserNewEntryJsonParserService : IHomeJsonParserService<UserRoomLink>
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        //private readonly IRepositoryAsync<Room> _roomRepository;
        private readonly IRepositoryAsync<UserRoomLink> _userRoomLinkRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public Room _dbRoom { get; private set; }

        #endregion      
        public RoomUserNewEntryJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, Room room)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            //_roomRepository = _unitOfWorkAsync.RepositoryAsync<Room>();
            _userRoomLinkRepository = _unitOfWorkAsync.RepositoryAsync<UserRoomLink>();

            _homeJsonEntity = homeJsonEntity;
            _dbRoom = room;

        }

        public UserRoomLink SaveJsonData()
        {
            UserRoomLink userRoomLink = null;
            try
            {
                userRoomLink = SaveNewRoomUser();
            }
            catch (Exception ex)
            {
                return null;
            }
            return userRoomLink;
        }
        private UserRoomLink SaveNewRoomUser()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            int appsRoomId = _homeJsonEntity.Room.FirstOrDefault().AppsRoomId;

            UserInfo userInfo = null;

            userInfo = new CommonService(_unitOfWorkAsync).GetUser(_homeJsonEntity.UserInfo[0].Email);
            if (_dbRoom != null)
            {
                return SaveRoomUser(userInfo);
            }
            return null;
        }
        private UserRoomLink SaveRoomUser(UserInfo userInfo)
        {
            Room entity = _dbRoom;
            var roomLinkList = _homeJsonEntity.UserRoomLink.FindAll(x => x.AppsRoomId == entity.AppsRoomId);

            foreach (var userRoomLinkEntity in roomLinkList)
            {
                UserRoomLink userRoom = new UserRoomLink();
                userRoom.UserInfo = userInfo;

                return FillSaveRoomUser(entity, userRoomLinkEntity, userRoom);
            }
            return null;
        }
        private UserRoomLink FillSaveRoomUser(Room entity, UserRoomLinkEntity userRoomLinkEntity, UserRoomLink userRoom)
        {
            userRoom.Room = entity;
            userRoom.IsSynced = Convert.ToBoolean(userRoomLinkEntity.IsSynced);
            userRoom.AppsUserRoomLinkId = userRoomLinkEntity.AppsUserRoomLinkId;
            userRoom.AppsRoomId = userRoomLinkEntity.AppsRoomId;
            userRoom.AppsUserId = userRoomLinkEntity.AppsUserId;
            userRoom.ObjectState = ObjectState.Added;
            _userRoomLinkRepository.Insert(userRoom);

            return userRoom;
        }
    }
}
