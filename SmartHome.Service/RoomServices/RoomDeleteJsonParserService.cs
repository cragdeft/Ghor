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
    public class RoomDeleteJsonParserService : IHomeDeleteJsonParserService<Room>
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
        public Room DeleteJsonData()
        {
            Room room = null;
            try
            {
                room = DeleteRoom();

            }
            catch (Exception ex)
            {

                return null;
            }
            return room;
        }
        private Room DeleteRoom()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            int appsRoomId = _homeJsonEntity.Room.FirstOrDefault().AppsRoomId;

            Room room = GetRoom(passPhrase, appsRoomId);
            if (room != null)
            {
                DeleteUserRooms(room);
                return DeleteRoom(room);
            }
            return null;
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
        private Room DeleteRoom(Room room)
        {
            room.ObjectState = ObjectState.Deleted;
            _roomRepository.Delete(room);
            return room;
        }
        private Room GetRoom(string passPhrase, int appsRoomId)
        {
            return _homeRepository.Queryable().Where(p => p.PassPhrase == passPhrase)
                                  .SelectMany(q => q.Rooms.Where(r => r.AppsRoomId == appsRoomId)).FirstOrDefault();
        }
        public void DeleteAllRooms(Home home)
        {
            IList<long> roomIds = home.Rooms.Select(s => s.RoomId).ToList();
            home.Rooms = new List<Room>();
            foreach (var roomId in roomIds)
            {
                Room dbRoom = _roomRepository
                .Queryable().Where(u => u.RoomId == roomId).FirstOrDefault();
                dbRoom.ObjectState = ObjectState.Deleted;
                _roomRepository.Delete(dbRoom);
            }
        }
    }
}
