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
        private readonly IRepositoryAsync<SmartDevice> _deviceRepository;
        private readonly IRepositoryAsync<Channel> _channelRepository;
        private readonly IRepositoryAsync<CommandJson> _commandJsonRepository;
        private string _email;
        #endregion

        public CommandParserService(IUnitOfWorkAsync unitOfWorkAsync, string email)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _deviceRepository = _unitOfWorkAsync.RepositoryAsync<SmartDevice>();
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
                var changes = _unitOfWorkAsync.SaveChanges();
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
                var changes = _unitOfWorkAsync.SaveChanges();
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
                var changes = _unitOfWorkAsync.SaveChanges();
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
                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();
                return channelStatus;
            }
            catch (Exception ex)
            {

                _unitOfWorkAsync.Rollback();
                return channelStatus;
            }
        }

        public SmartDevice FindDevice(int deviceHash)
        {
            return _deviceRepository
                .Query().Include(x => x.DeviceStatus).Include(x => ((SmartSwitch)x).Channels.Select(y => y.ChannelStatuses)).Select().ToList()
                .Where(u => u.DeviceHash == deviceHash.ToString())
                .FirstOrDefault();
        }



        public void LogCommand(CommandJsonEntity command)
        {
            Mapper.CreateMap<CommandJsonEntity, CommandJson>();
            CommandJson commanD = Mapper.Map<CommandJsonEntity, CommandJson>(command);
            _unitOfWorkAsync.BeginTransaction();

            try
            {

                commanD.AuditField = new AuditFields(_email, DateTime.Now, null, null);
                commanD.ObjectState = ObjectState.Added;
                _commandJsonRepository.Insert(commanD);
                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();
            }
            catch (Exception ex)
            {

                _unitOfWorkAsync.Rollback();
            }
        }

        public void UpdateChannel(Channel channel)
        {
            _unitOfWorkAsync.BeginTransaction();
            try
            {

                channel.AuditField = new AuditFields(channel.AuditField.InsertedBy, channel.AuditField.InsertedDateTime, _email, DateTime.Now);
                channel.ObjectState = ObjectState.Modified;
                _channelRepository.Update(channel);
                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();
            }
            catch (Exception)
            {

                _unitOfWorkAsync.Rollback();
            }
        }
    }
}
