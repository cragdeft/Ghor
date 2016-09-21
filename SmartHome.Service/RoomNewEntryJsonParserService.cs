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
    public class RoomNewEntryJsonParserService : IHomeJsonParserService
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

        public bool SaveJsonData()
        {
            bool isSuccess = false;

            SetMapper();
            try
            {
                isSuccess = SaveNewRoom();
            }
            catch (Exception ex)
            {
                return false;
            }

            return isSuccess;
        }
        private void SetMapper()
        {
            Mapper.CreateMap<RoomEntity, Room>();
        }
        private bool SaveNewRoom()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;

            Home home = null;
            Room room = null;
            UserInfo userInfo = null;

            bool isComplete = false;

            home = new CommonService(_unitOfWorkAsync).GetHome(passPhrase);
            userInfo = new CommonService(_unitOfWorkAsync).GetUser(_homeJsonEntity.UserInfo[0].Email);
            if (home != null && userInfo != null)
            {
                room = SaveNewRoom(home);

                IHomeJsonParserService service = new RoomUserNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, room);
                isComplete = service.SaveJsonData();
            }
            return isComplete;
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

            //SaveRoomUser(entity, userInfo);

            return entity;
        }
    }
}
