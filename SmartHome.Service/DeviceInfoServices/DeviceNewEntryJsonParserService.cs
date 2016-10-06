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
    public class DeviceNewEntryJsonParserService : IHomeJsonParserService<SmartDevice>
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<SmartDevice> _deviceRepository;
        private readonly IRepositoryAsync<DeviceStatus> _deviceStatusRepository;
        private readonly IRepositoryAsync<RgbwStatus> _rgbwStatusRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }

        #endregion
        public DeviceNewEntryJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _deviceRepository = _unitOfWorkAsync.RepositoryAsync<SmartDevice>();
            _deviceStatusRepository = _unitOfWorkAsync.RepositoryAsync<DeviceStatus>();
            _rgbwStatusRepository = _unitOfWorkAsync.RepositoryAsync<RgbwStatus>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
            SetMapper();
        }

        public SmartDevice SaveJsonData()
        {
            SmartDevice smartDevice = null;
          
            try
            {
                smartDevice=SaveNewSmartDevice();
            }
            catch (Exception ex)
            {
                return null;
            }
            return smartDevice;
        }
        private SmartDevice SaveNewSmartDevice()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            Home home = null;
            home = new CommonService(_unitOfWorkAsync).GetHomeWithRooms(passPhrase);
            if (home != null)
            {
                return SaveNewDevice(home);
            }
            return null;
        }
        private SmartDevice SaveNewDevice(Home home)
        {
            SmartDeviceEntity smartDevice = _homeJsonEntity.Device.FirstOrDefault();

            Room room = home.Rooms.Where(p => p.AppsRoomId == smartDevice.AppsRoomId).FirstOrDefault();
            return InsertDevice(smartDevice, room);
        }
        public SmartDevice InsertDevice(SmartDeviceEntity entity, Room room)
        {
            SmartDevice device = Mapper.Map<SmartDeviceEntity, SmartDevice>(entity);
            device.IsDeleted = Convert.ToBoolean(entity.IsDeleted);
            device.IsSynced = Convert.ToBoolean(entity.IsSynced);

            if (entity.DeviceType == DeviceType.SmartSwitch6g)
            {
                InsertSmartDevices(entity, room, device);
            }
            if (entity.DeviceType == DeviceType.SmartRainbow12)
            {
                InsertSmartRainbow(entity, device, room);
            }
            if (entity.DeviceType == DeviceType.SmartRouter)
            {
                InsertSmartRouter(entity, device, room);
            }
            return device;
        }
        private void InsertSmartRouter(SmartDeviceEntity entity,SmartDevice device, Room room)
        {
            SmartRouter router = MapToSmartRouter(device);
            router.Room = room;
            router.ObjectState = ObjectState.Added;
            router.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);

            _deviceRepository.Insert(router);

            List<DeviceStatusEntity> deviceStatuses = _homeJsonEntity.DeviceStatus.FindAll(x => x.AppsDeviceId == entity.AppsDeviceId.ToString());
            InsertRouterDeviceStatus(router, deviceStatuses);
        }

        private void InsertRouterDeviceStatus(SmartRouter router, List<DeviceStatusEntity> deviceStatuses)
        {
            foreach (var deviceStatusEntity in deviceStatuses)
            {
                var deviceStatus = Mapper.Map<DeviceStatusEntity, DeviceStatus>(deviceStatusEntity);
                deviceStatus.IsSynced = Convert.ToBoolean(deviceStatusEntity.IsSynced);
                deviceStatus.ObjectState = ObjectState.Added;
                deviceStatus.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
                _deviceStatusRepository.Insert(deviceStatus);
                router.DeviceStatus.Add(deviceStatus);
            }
        }
        private SmartRouter MapToSmartRouter(SmartDevice model)
        {
            SmartRouter entity = new SmartRouter();
            entity.DeviceId = model.DeviceId;
            entity.AppsRoomId = model.AppsRoomId;
            entity.AppsDeviceId = model.AppsDeviceId;
            entity.AppsBleId = model.AppsBleId;
            entity.DeviceName = model.DeviceName;
            entity.DeviceHash = model.DeviceHash;
            entity.FirmwareVersion = model.FirmwareVersion;
            entity.IsDeleted = model.IsDeleted;
            entity.Watt = model.Watt;
            entity.DeviceType = model.DeviceType;
            entity.DeviceStatus = new List<DeviceStatus>();
            return entity;
        }
        private void InsertSmartDevices(SmartDeviceEntity entity, Room room, SmartDevice device)
        {
            SmartSwitch sswitch = MapToSmartSwitch(device);
            sswitch.Room = room;
            sswitch.ObjectState = ObjectState.Added;
            sswitch.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            _deviceRepository.Insert(sswitch);

            List<DeviceStatusEntity> deviceStatuses = _homeJsonEntity.DeviceStatus.FindAll(x => x.AppsDeviceId == entity.AppsDeviceId.ToString());
            InsertDeviceStatus(sswitch, deviceStatuses);
        }
        private void InsertSmartRainbow(SmartDeviceEntity entity, SmartDevice device, Room room)
        {
            SmartRainbow rainbow = MapToSmartRainbow(device);
            rainbow.Room = room;
            rainbow.ObjectState = ObjectState.Added;
            rainbow.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            _deviceRepository.Insert(rainbow);

            List<RgbwStatusEntity> rgbtStatusList = _homeJsonEntity.RgbwStatus.FindAll(x => x.AppsDeviceId == entity.AppsDeviceId);
            InsertRgbwStatus(rainbow, rgbtStatusList);
        }
        private SmartRainbow MapToSmartRainbow(SmartDevice model)
        {
            SmartRainbow entity = new SmartRainbow();
            entity.DeviceId = model.DeviceId;
            entity.AppsRoomId = model.AppsRoomId;
            entity.AppsDeviceId = model.AppsDeviceId;
            entity.AppsBleId = model.AppsBleId;
            entity.DeviceName = model.DeviceName;
            entity.DeviceHash = model.DeviceHash;
            entity.FirmwareVersion = model.FirmwareVersion;
            entity.IsDeleted = model.IsDeleted;
            entity.Watt = model.Watt;
            //entity.Mac = model.Mac;
            //entity.DType = model.DType;
            entity.DeviceType = model.DeviceType;
            entity.DeviceStatus = new List<DeviceStatus>();
            return entity;
        }
        private void InsertRgbwStatus(SmartRainbow rainbow, List<RgbwStatusEntity> rgbtStatusList)
        {
            foreach (var rgbStatusEntity in rgbtStatusList)
            {
                var rgbStatus = Mapper.Map<RgbwStatusEntity, RgbwStatus>(rgbStatusEntity);
                rgbStatus.SmartRainbow = rainbow;
                rgbStatus.ObjectState = ObjectState.Added;
                rgbStatus.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
                _rgbwStatusRepository.Insert(rgbStatus);
            }
        }
        private SmartSwitch MapToSmartSwitch(SmartDevice model)
        {
            SmartSwitch entity = new SmartSwitch();
            entity.DeviceId = model.DeviceId;
            entity.AppsRoomId = model.AppsRoomId;
            entity.AppsDeviceId = model.AppsDeviceId;
            entity.AppsBleId = model.AppsBleId;
            entity.DeviceName = model.DeviceName;
            entity.DeviceHash = model.DeviceHash;
            entity.FirmwareVersion = model.FirmwareVersion;
            entity.IsDeleted = model.IsDeleted;
            entity.Watt = model.Watt;
            entity.DeviceType = model.DeviceType;
            entity.DeviceStatus = new List<DeviceStatus>();
            return entity;
        }
        private void InsertDeviceStatus(SmartSwitch sswitch, List<DeviceStatusEntity> deviceStatuses)
        {
            foreach (var deviceStatusEntity in deviceStatuses)
            {
                var deviceStatus = Mapper.Map<DeviceStatusEntity, DeviceStatus>(deviceStatusEntity);
                deviceStatus.IsSynced = Convert.ToBoolean(deviceStatusEntity.IsSynced);
                deviceStatus.ObjectState = ObjectState.Added;
                deviceStatus.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
                _deviceStatusRepository.Insert(deviceStatus);
                sswitch.DeviceStatus.Add(deviceStatus);
            }
        }
        private void SetMapper()
        {
            Mapper.CreateMap<SmartDeviceEntity, SmartDevice>();
            Mapper.CreateMap<DeviceStatusEntity, DeviceStatus>();
        }
    }
}
