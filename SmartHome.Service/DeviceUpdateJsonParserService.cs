﻿using AutoMapper;
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
    public class DeviceUpdateJsonParserService : IHomeUpdateJsonParserService
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
        public DeviceUpdateJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _deviceRepository = _unitOfWorkAsync.RepositoryAsync<SmartDevice>();
            _deviceStatusRepository = _unitOfWorkAsync.RepositoryAsync<DeviceStatus>();
            _rgbwStatusRepository = _unitOfWorkAsync.RepositoryAsync<RgbwStatus>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
        }

        public bool UpdateJsonData()
        {
            SetMapper();
            try
            {
                UpdateSmartDeviceInfos();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        private void UpdateSmartDeviceInfos()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            string deviceHash = _homeJsonEntity.Device.FirstOrDefault().DeviceHash;

            UpdateDevice(_homeJsonEntity.Device.FirstOrDefault(), passPhrase, deviceHash);
        }

        public void UpdateDevice(SmartDeviceEntity entity, string passPhrase, string deviceHash)
        {
            switch (entity.DeviceType)
            {
                case DeviceType.SmartSwitch6g:
                    SmartSwitch dbswitch = new CommonService(_unitOfWorkAsync).GetSmartSwitchByDeviceHashAndPassPhrase<SmartSwitch>(deviceHash, passPhrase);

                    IHomeUpdateJsonParserService updateSwitch = new SmartSwitchUpdateJsonParserService(_unitOfWorkAsync, _homeJsonEntity, dbswitch);
                    updateSwitch.UpdateJsonData();
                    break;
                case DeviceType.SmartRainbow12:

                    SmartRainbow dbRainbow = new CommonService(_unitOfWorkAsync).GetSmartSwitchByDeviceHashAndPassPhrase<SmartRainbow>(deviceHash, passPhrase);
                    UpdateSmartRainbow(entity, dbRainbow);

                    break;
                case DeviceType.CurtainV:
                    break;
                case DeviceType.CurtainH:
                    break;
                case DeviceType.Camera:
                    break;
                case DeviceType.SmartRouter:
                    SmartRouter dbRouter = new CommonService(_unitOfWorkAsync).GetSmartSwitchByDeviceHashAndPassPhrase<SmartRouter>(deviceHash, passPhrase);
                    UpdateSmartRouter(entity, dbRouter);
                    break;
            }
        }

        private T MapSmartDeviceProperties<T>(SmartDeviceEntity entity, T smartDevice) where T : SmartDevice
        {
            Mapper.CreateMap<SmartDeviceEntity, T>()
              .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == 1 ? true : false))
              .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted == 1 ? true : false))
              .ForMember(x => x.Room, y => y.Ignore())
              .ForMember(x => x.DeviceId, y => y.Ignore());

            return Mapper.Map<SmartDeviceEntity, T>(entity, smartDevice);
        }

        private void UpdateSmartRouter(SmartDeviceEntity entity, SmartRouter device)
        {
            SmartRouter router = MapSmartDeviceProperties<SmartRouter>(entity, device);
            router.ObjectState = ObjectState.Modified;
            router.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);

            _deviceRepository.Update(router);
        }
        private void UpdateSmartRainbow(SmartDeviceEntity entity, SmartRainbow device)
        {
            SmartRainbow rainbow = MapSmartDeviceProperties<SmartRainbow>(entity, device);//MapToSmartRainbow(device);
            rainbow.ObjectState = ObjectState.Modified;
            rainbow.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            _deviceRepository.Update(rainbow);

            DeleteRgbStatus(rainbow);

            List<RgbwStatusEntity> rgbtStatusList = _homeJsonEntity.RgbwStatus.FindAll(x => x.AppsDeviceId == entity.AppsDeviceId);
            InsertRgbwStatus(rainbow, rgbtStatusList);
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
                rainbow.RgbwStatuses.Add(rgbStatus);
            }
        }

        private void DeleteRgbStatus(SmartRainbow rainbow)
        {
            IList<RgbwStatus> dbRgbStatus = _rgbwStatusRepository.Queryable().Where(p => p.SmartRainbow.DeviceId == rainbow.DeviceId).ToList();

            foreach (var rgbStatus in dbRgbStatus)
            {
                rgbStatus.ObjectState = ObjectState.Deleted;
                _rgbwStatusRepository.Delete(rgbStatus);

                rainbow.RgbwStatuses.Remove(rgbStatus);
            }
        }





        private void SetMapper()
        {
            Mapper.CreateMap<SmartDeviceEntity, SmartDevice>();
            Mapper.CreateMap<DeviceStatusEntity, DeviceStatus>();
        }

    }
}
