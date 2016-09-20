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
    public class RoomUserNewEntryJsonParserService : IHomeJsonParserService
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

        public bool SaveJsonData()
        {
            bool isSuccess = false;
            try
            {
                isSuccess = SaveNewRoomUser();
            }
            catch (Exception ex)
            {
                return false;
            }
            return isSuccess;
        }
        private bool SaveNewRoomUser()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            int appsRoomId = _homeJsonEntity.Room.FirstOrDefault().AppsRoomId;

            UserInfo userInfo = null;
            bool isComplete = false;

            userInfo = new CommonService(_unitOfWorkAsync).GetUser(_homeJsonEntity.UserInfo[0].Email);
            if (_dbRoom != null)
            {
                SaveRoomUser(userInfo);
                isComplete = true;
            }
            return isComplete;
        }
        private void SaveRoomUser(UserInfo userInfo)
        {
            Room entity = _dbRoom;
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
    }
}
