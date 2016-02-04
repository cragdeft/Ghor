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
using SmartHome.Model.Enums;

namespace SmartHome.Service
{
    public class CommandParserService : ICommandPerserService
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<DeviceStatus> _deviceStatusRepository;
        private readonly IRepositoryAsync<ChannelStatus> _channelStatusRepository;
        private readonly IRepositoryAsync<Device> _deviceRepository;
        private readonly IRepositoryAsync<Channel> _channelRepository;
        private readonly IRepositoryAsync<CommandJson> _commandJsonRepository;
        private string _email;
        #endregion

        public CommandParserService(IUnitOfWorkAsync unitOfWorkAsync, string email)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _deviceRepository = _unitOfWorkAsync.RepositoryAsync<Device>();
            _deviceStatusRepository = _unitOfWorkAsync.RepositoryAsync<DeviceStatus>();
            _channelStatusRepository = _unitOfWorkAsync.RepositoryAsync<ChannelStatus>();
            _channelRepository = _unitOfWorkAsync.RepositoryAsync<Channel>();
            _commandJsonRepository = _unitOfWorkAsync.RepositoryAsync<CommandJson>();
            _email = email;
        }

        public DeviceStatus UpdateDeviceStatus(DeviceStatus deviceStatus)
        {

            _unitOfWorkAsync.BeginTransaction();
            try
            {

                deviceStatus.AuditField = new AuditFields(deviceStatus.AuditField.InsertedBy, deviceStatus.AuditField.InsertedDateTime, _email, DateTime.Now);
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
                deviceStatus.AuditField = new AuditFields(_email, DateTime.Now, null, null);
                deviceStatus.ObjectState = ObjectState.Added;
                _deviceStatusRepository.Insert(deviceStatus);
                var changes = _unitOfWorkAsync.SaveChangesAsync();
                _unitOfWorkAsync.Commit();
                return deviceStatus;
            }
            catch (Exception ex)
            {

                _unitOfWorkAsync.Rollback();
                return deviceStatus;
            }
        }

        public ChannelStatus UpdateChannelStatus(ChannelStatus channelStatus)
        {
            _unitOfWorkAsync.BeginTransaction();
            try
            {

                channelStatus.AuditField = new AuditFields(channelStatus.AuditField.InsertedBy, channelStatus.AuditField.InsertedDateTime, _email, DateTime.Now);
                channelStatus.ObjectState = ObjectState.Modified;
                _channelStatusRepository.Update(channelStatus);
                var changes = _unitOfWorkAsync.SaveChangesAsync();
                _unitOfWorkAsync.Commit();
                return channelStatus;
            }
            catch (Exception)
            {

                _unitOfWorkAsync.Rollback();
                return channelStatus;
            }
        }

        public ChannelStatus AddChannelStatus(ChannelStatus channelStatus)
        {
            _unitOfWorkAsync.BeginTransaction();
            try
            {
                channelStatus.AuditField = new AuditFields(_email, DateTime.Now, null, null);
                channelStatus.ObjectState = ObjectState.Added;
                _channelStatusRepository.Insert(channelStatus);
                var changes = _unitOfWorkAsync.SaveChangesAsync();
                _unitOfWorkAsync.Commit();
                return channelStatus;
            }
            catch (Exception ex)
            {

                _unitOfWorkAsync.Rollback();
                return channelStatus;
            }
        }

        public DeviceStatus FindDeviceStatus(int deviceid, int Id)
        {
            return _deviceStatusRepository
                .Queryable()
                .Where(u => u.DId == deviceid && u.StatusType == (StatusType)Id)
                .FirstOrDefault();
        }

        public Device FindDevice(int deviceHash)
        {
            return _deviceRepository
                .Queryable()
                .Where(u => u.DeviceHash == deviceHash.ToString())
                .FirstOrDefault();
        }

        public ChannelStatus FindChannelStatus(int deviceid, int Id)
        {
            return _channelStatusRepository
                .Queryable()
                .Where(u => u.DId == deviceid && u.ChannelNo == Id)
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

        public void LogCommand(CommandJsonEntity command)
        {
            //Mapper.CreateMap<CommandJsonEntity, CommandJson>();
            CommandJson commanD = Mapper.Map<CommandJsonEntity, CommandJson>(command);
            _unitOfWorkAsync.BeginTransaction();

            try
            {

                commanD.AuditField = new AuditFields(_email, DateTime.Now, null, null);
                commanD.ObjectState = ObjectState.Added;
                _commandJsonRepository.Insert(commanD);
                var changes = _unitOfWorkAsync.SaveChangesAsync();
                _unitOfWorkAsync.Commit();
            }
            catch (Exception ex)
            {

                _unitOfWorkAsync.Rollback();
            }
        }
    }
}
