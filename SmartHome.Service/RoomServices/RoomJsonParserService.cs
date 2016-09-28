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
    public class RoomJsonParserService : IHomeJsonParserService<Room>
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }

        #endregion
        public RoomJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
        }
        public Room SaveJsonData()
        {
            Room room = null;
            try
            {
                if (_homeJsonEntity.Room.Count == 0)
                {
                    return room;
                }

                string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;

                RoomEntity roomEntity = _homeJsonEntity.Room.FirstOrDefault();
                Room dbRoom = new CommonService(_unitOfWorkAsync).GetRoomByPassPhaseAndAppsRoomId(passPhrase, roomEntity.AppsRoomId);

                if (dbRoom != null)
                {
                    IHomeUpdateJsonParserService<Room> updateService = new RoomUpdateJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.UpdateRoom);
                    room = updateService.UpdateJsonData();

                }
                else
                {
                    IHomeJsonParserService<Room> service = new RoomNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.NewRoom);
                    room = service.SaveJsonData();
                }


            }
            catch (Exception ex)
            {
                return null;
            }
            return room;
        }
    }
}
