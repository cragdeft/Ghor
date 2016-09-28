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
    public class DeviceRoomUpdateJsonParserService : IHomeUpdateJsonParserService<SmartDevice>
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<Home> _homeRepository;
        private readonly IRepositoryAsync<SmartDevice> _deviceRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }

        #endregion


        public DeviceRoomUpdateJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();
            _deviceRepository = _unitOfWorkAsync.RepositoryAsync<SmartDevice>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
        }

        public SmartDevice UpdateJsonData()
        {
            SmartDevice smartDevice = null;

            try
            {
                smartDevice = UpdateSmartDeviceRoom();
            }
            catch (Exception ex)
            {
                return null;
            }
            return smartDevice;
        }
        private SmartDevice UpdateSmartDeviceRoom()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            string deviceHash = _homeJsonEntity.Device.FirstOrDefault().DeviceHash;
            int appsRoomId = _homeJsonEntity.Device.FirstOrDefault().AppsRoomId;

            SmartDevice smartDevice = null;

            smartDevice = GetSmartDeviceByDeviceHashAndPassPhrase(deviceHash, passPhrase);

            Room room = GetRoom(passPhrase, appsRoomId);
            if (smartDevice != null)
            {
                smartDevice.AppsRoomId = appsRoomId;
                return UpdateDevice(smartDevice, room);
            }
            return null;
        }

        private Room GetRoom(string passPhrase, int appsRoomId)
        {
            return _homeRepository.Queryable().Where(p => p.PassPhrase == passPhrase)
                                  .SelectMany(q => q.Rooms.Where(r => r.AppsRoomId == appsRoomId)).FirstOrDefault();
        }

        private SmartDevice UpdateDevice(SmartDevice smartDevice, Room room)
        {
            smartDevice.Room = room;
            smartDevice.ObjectState = ObjectState.Modified;
            _deviceRepository.Update(smartDevice);
            return smartDevice;
        }



        private SmartDevice GetSmartDeviceByDeviceHashAndPassPhrase(string deviceHash, string passPhrase)
        {
            return _homeRepository.Queryable().Where(p => p.PassPhrase == passPhrase)
              .SelectMany(p => p.Rooms)
              .SelectMany(q => q.SmartDevices.Where(s => s.DeviceHash == deviceHash))
              .FirstOrDefault();
        }


    }


}
