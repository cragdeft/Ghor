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

        public bool SaveJsonData()
        {
            IHomeJsonParserService service = null;
            bool isSuccess = false;
            try
            {
                string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;

                RoomEntity roomEntity = _homeJsonEntity.Room.FirstOrDefault();
                Room dbRoom = new CommonService(_unitOfWorkAsync).GetRoomByPassPhaseAndAppsRoomId(passPhrase, roomEntity.AppsRoomId);

                if (dbRoom != null)
                {
                    service = new RoomUpdateJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.UpdateRoom);
                }
                else
                {
                    service = new RoomNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.NewRoom);
                }
                isSuccess = service.SaveJsonData();

            }
            catch (Exception ex)
            {
                return false;
            }
            return isSuccess;
        }
    }
}
