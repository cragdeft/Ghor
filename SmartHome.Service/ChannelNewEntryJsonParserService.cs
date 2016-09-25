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
    public class ChannelNewEntryJsonParserService : IHomeJsonParserService
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<Home> _homeRepository;
        private readonly IRepositoryAsync<Channel> _channelRepository;
        private readonly IRepositoryAsync<ChannelStatus> _channelStatusRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }

        #endregion


        public ChannelNewEntryJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();
            _channelRepository = _unitOfWorkAsync.RepositoryAsync<Channel>();
            _channelStatusRepository = _unitOfWorkAsync.RepositoryAsync<ChannelStatus>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
        }

        public bool SaveJsonData()
        {
            SetMapper();
            try
            {
                SaveNewSmartSwitchCannel();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private void SaveNewSmartSwitchCannel()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            string deviceHash = _homeJsonEntity.Device.FirstOrDefault().DeviceHash;

            SmartSwitch sSwitch = null;

            sSwitch = new CommonService(_unitOfWorkAsync).GetSmartSwitchByDeviceHashAndPassPhrase<SmartSwitch>(deviceHash, passPhrase);

            if (sSwitch != null)
            {
                SaveNewChannel(sSwitch);
            }
        }

        private void SaveNewChannel(SmartSwitch device)
        {
            List<ChannelEntity> channelList = _homeJsonEntity.Channel.FindAll(x => x.AppsDeviceTableId == device.AppsDeviceId);
            if (channelList.Count > 0)
            {
                InsertChannel(device, channelList.First());
            }

        }
        public void InsertChannel(SmartSwitch model, ChannelEntity channelEntity)
        {
            SmartSwitch sswitch = model;
            sswitch.Channels = new List<Channel>();

            Channel dbChannel = _channelRepository.Queryable().Where(p => p.SmartSwitch.DeviceId == model.DeviceId && p.AppsChannelId == channelEntity.AppsChannelId).FirstOrDefault();

            if (dbChannel == null)
            {
                Channel channel = SaveChannel(channelEntity);
                SaveChannelStatus(channelEntity, channel);
                sswitch.Channels.Add(channel);
            }
        }

        private void SaveChannelStatus(ChannelEntity channelEntity, Channel channel)
        {
            channel.ChannelStatuses = new List<ChannelStatus>();
            List<ChannelStatusEntity> channelStatusEntities = _homeJsonEntity.ChannelStatus.FindAll(x => x.AppsChannelId == channelEntity.AppsChannelId);

            foreach (var channelStatusEntity in channelStatusEntities)
            {
                var channelStatus = Mapper.Map<ChannelStatusEntity, ChannelStatus>(channelStatusEntity);

                channelStatus.IsSynced = Convert.ToBoolean(channelStatusEntity.IsSynced);
                channelStatus.ObjectState = ObjectState.Added;
                channelStatus.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);

                _channelStatusRepository.Insert(channelStatus);
                channel.ChannelStatuses.Add(channelStatus);
            }
        }

        private Channel SaveChannel(ChannelEntity entity)
        {
            var channel = Mapper.Map<ChannelEntity, Channel>(entity);

            channel.IsSynced = Convert.ToBoolean(channel.IsSynced);
            channel.ObjectState = ObjectState.Added;
            channel.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            _channelRepository.Insert(channel);

            return channel;
        }

        private void SetMapper()
        {
            Mapper.CreateMap<ChannelEntity, Channel>();
            Mapper.CreateMap<ChannelStatusEntity, ChannelStatus>();
        }
    }
}
