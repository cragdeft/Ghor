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
    //class DeviceDeleteJsonParserService
    //{
    //}

    public class DeviceDeleteJsonParserService : IHomeJsonParserService
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<Home> _homeRepository;
        private readonly IRepositoryAsync<Room> _roomRepository;
        private readonly IRepositoryAsync<SmartDevice> _deviceRepository;
        private readonly IRepositoryAsync<DeviceStatus> _deviceStatusRepository;
        private readonly IRepositoryAsync<RgbwStatus> _rgbwStatusRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }
        public MessageLog _messageLog { get; private set; }

        #endregion


        public DeviceDeleteJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();
            _roomRepository = _unitOfWorkAsync.RepositoryAsync<Room>();
            _deviceRepository = _unitOfWorkAsync.RepositoryAsync<SmartDevice>();
            _deviceStatusRepository = _unitOfWorkAsync.RepositoryAsync<DeviceStatus>();
            _rgbwStatusRepository = _unitOfWorkAsync.RepositoryAsync<RgbwStatus>();


            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
            _messageLog = new MessageLog();
        }

        public bool SaveJsonData()
        {
            MessageLog messageLog = new CommonService(_unitOfWorkAsync).SaveMessageLog(_homeJsonMessage, _receivedFrom);

            _unitOfWorkAsync.BeginTransaction();
            SetMapper();
            try
            {
                DeleteSmartDevice();
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
        private void DeleteSmartDevice()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            string deviceHash = _homeJsonEntity.Device.FirstOrDefault().DeviceHash;

            SmartDevice smartDevice = null;

            smartDevice = GetSmartSwitchByDeviceHashAndPassPhrase(deviceHash, passPhrase);
            if (smartDevice != null)
            {
                DeleteDevice(smartDevice);
            }
        }

        private void DeleteDevice(SmartDevice smartDevice)
        {
            smartDevice.ObjectState = ObjectState.Deleted;
            _deviceRepository.Delete(smartDevice);
        }

        private SmartDevice GetSmartSwitchByDeviceHashAndPassPhrase(string deviceHash, string passPhrase)
        {
            return _homeRepository.Queryable().Where(p => p.PassPhrase == passPhrase)
              .SelectMany(p => p.Rooms)
              .SelectMany(q => q.SmartDevices.Where(s => s.DeviceHash == deviceHash))
              .FirstOrDefault();
        }

        private void SetMapper()
        {
            Mapper.CreateMap<SmartDeviceEntity, SmartDevice>();
            Mapper.CreateMap<DeviceStatusEntity, DeviceStatus>();
        }
    }
}
