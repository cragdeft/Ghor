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
    public class RoomUpdateJsonParserService : IHomeUpdateJsonParserService
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
        public bool UpdateJsonData()
        {
            bool isSuccess = false;

            try
            {
                UpdateRoomInfo();
                isSuccess = true;
            }
            catch (Exception ex)
            {
                return false;
            }
            return isSuccess;
        }
        private void UpdateRoomInfo()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            UpdateRoom(passPhrase);

        }
        private void UpdateRoom(string passPhrase)
        {
            RoomEntity roomEntity = _homeJsonEntity.Room[0];
            Room dbRoom = new CommonService(_unitOfWorkAsync).GetRoomByPassPhaseAndAppsRoomId(passPhrase, roomEntity.AppsRoomId);

            if (dbRoom != null)
            {
                var entity = MapRoomProperties(roomEntity, dbRoom);

                entity.AuditField = new AuditFields(dbRoom.AuditField.InsertedBy, dbRoom.AuditField.InsertedDateTime, "admin", DateTime.Now);
                entity.ObjectState = ObjectState.Modified;
                _roomRepository.Update(entity);
            }
        }
        private Room MapRoomProperties(RoomEntity roomEntity, Room dbRoom)
        {
            Mapper.CreateMap<RoomEntity, Room>()
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == 1 ? true : false))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == 1 ? true : false))
                .ForMember(x => x.Home, y => y.Ignore())
                .ForMember(x => x.RoomId, y => y.Ignore());

            return Mapper.Map<RoomEntity, Room>(roomEntity, dbRoom);
        }
    }
}
