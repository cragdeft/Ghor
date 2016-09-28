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
    public class RoomNewEntryJsonParserService : IHomeJsonParserService<Room>
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<Room> _roomRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }

        #endregion
        public RoomNewEntryJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _roomRepository = _unitOfWorkAsync.RepositoryAsync<Room>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
        }

        public Room SaveJsonData()
        {
            Room room = null;

            SetMapper();
            try
            {
                room = SaveNewRoom();
            }
            catch (Exception ex)
            {
                return null;
            }

            return room;
        }
        private void SetMapper()
        {
            Mapper.CreateMap<RoomEntity, Room>();
        }
        private Room SaveNewRoom()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;

            Home home = null;
            Room room = null;
            UserInfo userInfo = null;

            home = new CommonService(_unitOfWorkAsync).GetHome(passPhrase);
            userInfo = new CommonService(_unitOfWorkAsync).GetUser(_homeJsonEntity.UserInfo[0].Email);
            if (home != null && userInfo != null)
            {
                room = SaveNewRoom(home);

                IHomeJsonParserService<UserRoomLink> service = new RoomUserNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, room);
                service.SaveJsonData();
                return room;
            }
            return null;
        }
        private Room SaveNewRoom(Home home)
        {
            RoomEntity room = _homeJsonEntity.Room[0];

            var entity = Mapper.Map<RoomEntity, Room>(room);
            entity.IsActive = Convert.ToBoolean(room.IsActive);
            entity.IsSynced = Convert.ToBoolean(room.IsSynced);
            entity.Home = home;
            entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            entity.ObjectState = ObjectState.Added;
            _roomRepository.Insert(entity);
            return entity;
        }
    }
}
