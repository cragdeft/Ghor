using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Models;
using SmartHome.Service.Interfaces;

namespace SmartHome.Service
{
    public class CommandParserService: ICommandPerserService
    {
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<DeviceStatus> _deviceStatusRepository;
        private readonly IRepositoryAsync<ChannelStatus> _channelStatusRepository;
        private readonly IRepositoryAsync<Device> _deviceRepository;
        private readonly IRepositoryAsync<Channel> _channelRepository;
        private string _email;

        public CommandParserService(IUnitOfWorkAsync unitOfWorkAsync,string email)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _deviceRepository = _unitOfWorkAsync.RepositoryAsync<Device>();
            _deviceStatusRepository = _unitOfWorkAsync.RepositoryAsync<DeviceStatus>();
            _channelStatusRepository = _unitOfWorkAsync.RepositoryAsync<ChannelStatus>();
            _channelRepository = _unitOfWorkAsync.RepositoryAsync<Channel>();
            _email = email;
        }

        public DeviceStatus UpdateDeviceStatus(DeviceStatus deviceStatus)
        {
            
            _unitOfWorkAsync.BeginTransaction();
            try
            {

                deviceStatus.AuditField = new AuditFields(deviceStatus.AuditField.InsertedBy, deviceStatus.AuditField.InsertedDateTime, _email,DateTime.Now);
                deviceStatus.ObjectState = ObjectState.Modified;
                _deviceStatusRepository.Update(deviceStatus);
                var changes = _unitOfWorkAsync.SaveChangesAsync();
                _unitOfWorkAsync.Commit();
                return deviceStatus;
            }
            catch (Exception)
            {

                _unitOfWorkAsync.Rollback();
                return deviceStatus;
            }
        }

        public DeviceStatus AddDeviceStatus(DeviceStatus deviceStatus)
        {
            _unitOfWorkAsync.BeginTransaction();
            try
            {
                deviceStatus.AuditField = new AuditFields(_email, DateTime.Now,null,null);
                deviceStatus.ObjectState = ObjectState.Added;
                _deviceStatusRepository.Insert(deviceStatus);
                var changes = _unitOfWorkAsync.SaveChangesAsync();
                _unitOfWorkAsync.Commit();
                return deviceStatus;
            }
            catch (Exception)
            {

                _unitOfWorkAsync.Rollback();
                return deviceStatus;
            }
        }

        public bool UpdateChannelStatus(ChannelStatus channelStatus)
        {
            throw new NotImplementedException();
        }

        public ChannelStatus AddChannelStatus(ChannelStatus channelStatus)
        {
            throw new NotImplementedException();
        }

        public DeviceStatus FindDeviceStatus(int deviceid,int Id)
        {
            return _deviceStatusRepository
                .Queryable()
                .Where(u => u.DId == deviceid && u.StatusType == Id)
                .FirstOrDefault();
        }

        public Device FindDevice(int id)
        {
            return _deviceRepository
                .Queryable()
                .Where(u => u.DeviceId == id)
                .FirstOrDefault();
        }

        public ChannelStatus FindChannelStatus(int deviceid, int Id)
        {
            return _channelStatusRepository
                .Queryable()
                .Where(u => u.DId == deviceid && u.Id == Id)
                .FirstOrDefault();
        }

        public Channel FindChannel(int deviceId, int channelNo)
        {
            return _channelRepository
                .Queryable()
                .Where(u => u.DId == deviceId && u.ChannelNo == channelNo)
                .FirstOrDefault();
        }

        public List<ChannelStatus> GetAllChannelStatus(int deviceId)
        {
            return _channelStatusRepository
                .Queryable()
                .Where(u => u.DId == deviceId).ToList();
        }

        public Device AdddDevice(Device device)
        {
            _unitOfWorkAsync.BeginTransaction();

            try
            {

                device.AuditField = new AuditFields( _email, DateTime.Now,null,null);
                device.ObjectState = ObjectState.Added;
                _deviceRepository.Insert(device);
                var changes = _unitOfWorkAsync.SaveChangesAsync();
                _unitOfWorkAsync.Commit();
                return device;
            }
            catch (Exception ex)
            {

                _unitOfWorkAsync.Rollback();
                return device;
            }
        }
    }
}
