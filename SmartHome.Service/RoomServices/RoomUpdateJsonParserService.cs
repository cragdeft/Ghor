using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.Models;
using SmartHome.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{
    public class RoomUpdateJsonParserService : IHomeUpdateJsonParserService<Room>
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        //private readonly IRepositoryAsync<Home> _homeRepository;
        private readonly IRepositoryAsync<Room> _roomRepository;
        //private readonly IRepositoryAsync<UserRoomLink> _userRoomLinkRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }

        #endregion
        public RoomUpdateJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _roomRepository = _unitOfWorkAsync.RepositoryAsync<Room>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
        }
        public Room UpdateJsonData()
        {
            Room room = null;

            try
            {
                room = UpdateRoomInfo();
            }
            catch (Exception ex)
            {
                return null;
            }
            return room;
        }
        private Room UpdateRoomInfo()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            return UpdateRoom(passPhrase);

        }
        private Room UpdateRoom(string passPhrase)
        {
            RoomEntity roomEntity = _homeJsonEntity.Room[0];
            Room dbRoom = new CommonService(_unitOfWorkAsync).GetRoomByPassPhaseAndAppsRoomId(passPhrase, roomEntity.AppsRoomId);

            if (dbRoom != null)
            {
                var entity = SmartHomeTranslater.MapRoomInfoProperties(roomEntity, dbRoom);

                entity.AuditField = new AuditFields(dbRoom.AuditField.InsertedBy, dbRoom.AuditField.InsertedDateTime, "admin", DateTime.Now);
                entity.ObjectState = ObjectState.Modified;
                _roomRepository.Update(entity);
                return entity;
            }
            return null;
        }
    }
}
