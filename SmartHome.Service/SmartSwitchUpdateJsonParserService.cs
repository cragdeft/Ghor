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
    public class SmartSwitchUpdateJsonParserService : IHomeUpdateJsonParserService
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<SmartDevice> _deviceRepository;
        private readonly IRepositoryAsync<DeviceStatus> _deviceStatusRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public SmartSwitch _dbSmartSwitch { get; private set; }

        #endregion
        public SmartSwitchUpdateJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, SmartSwitch smartSwitch)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _deviceRepository = _unitOfWorkAsync.RepositoryAsync<SmartDevice>();
            _deviceStatusRepository = _unitOfWorkAsync.RepositoryAsync<DeviceStatus>();

            _homeJsonEntity = homeJsonEntity;
            _dbSmartSwitch = smartSwitch;
        }

        public bool UpdateJsonData()
        {
            try
            {
                UpdateSmartSwitch(_homeJsonEntity.Device.FirstOrDefault(), _dbSmartSwitch);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        private void UpdateSmartSwitch(SmartDeviceEntity entity, SmartSwitch device)
        {
            SmartSwitch sswitch = SmartHomeTranslater.MapSmartDeviceProperties<SmartSwitch>(entity, device);
            sswitch.ObjectState = ObjectState.Modified;
            sswitch.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            _deviceRepository.Update(sswitch);

            DeleteSwitchDeviceStatus(sswitch);

            List<DeviceStatusEntity> deviceStatuses = _homeJsonEntity.DeviceStatus.FindAll(x => x.AppsDeviceId == entity.AppsDeviceId.ToString());
            InsertSwitchDeviceStatus(sswitch, deviceStatuses);
        }
        private void InsertSwitchDeviceStatus(SmartSwitch sswitch, List<DeviceStatusEntity> deviceStatuses)
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
        private void DeleteSwitchDeviceStatus(SmartSwitch sswitch)
        {
            IList<DeviceStatus> dbDeviceStatuses = _deviceStatusRepository.Queryable().Where(p => p.SmartDevice.DeviceId == sswitch.DeviceId).ToList();

            foreach (var deviceStatus in dbDeviceStatuses)
            {
                deviceStatus.ObjectState = ObjectState.Deleted;
                _deviceStatusRepository.Delete(deviceStatus);

                sswitch.DeviceStatus.Remove(deviceStatus);
            }
        }
    }
}
