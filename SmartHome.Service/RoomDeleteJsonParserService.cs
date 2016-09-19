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
    public class RoomDeleteJsonParserService : IHomeDeleteJsonParserService
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<Home> _homeRepository;
        private readonly IRepositoryAsync<Room> _roomRepository;
        private readonly IRepositoryAsync<UserRoomLink> _userRoomLinkRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }
        public MessageLog _messageLog { get; private set; }

        #endregion
        public RoomDeleteJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Model.Models.Home>();
            _roomRepository = _unitOfWorkAsync.RepositoryAsync<Room>();
            _userRoomLinkRepository = _unitOfWorkAsync.RepositoryAsync<UserRoomLink>();

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
                DeleteRoom();
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
        private void DeleteRoom()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            int appsRoomId = _homeJsonEntity.Room.FirstOrDefault().AppsRoomId;

            Room room = GetRoom(passPhrase, appsRoomId);
            if (room != null)
            {
                DeleteUserRooms(room);
                DeleteRoom(room);
            }
        }
        private void DeleteUserRooms(Room room)
        {
            IList<UserRoomLink> userRoomLinks = GetUserRoomLinks(room.RoomId);
            foreach (var userRoom in userRoomLinks)
            {
                userRoom.ObjectState = ObjectState.Deleted;
                _userRoomLinkRepository.Delete(userRoom);
            }
        }
        private IList<UserRoomLink> GetUserRoomLinks(long roomId)
        {
            return _userRoomLinkRepository.Queryable().Where(p => p.Room.RoomId == roomId).ToList();
        }
        private void DeleteRoom(Room room)
        {
            room.ObjectState = ObjectState.Deleted;
            _roomRepository.Delete(room);
        }
        private Room GetRoom(string passPhrase, int appsRoomId)
        {
            return _homeRepository.Queryable().Where(p => p.PassPhrase == passPhrase)
                                  .SelectMany(q => q.Rooms.Where(r => r.AppsRoomId == appsRoomId)).FirstOrDefault();
        }
    }
}
