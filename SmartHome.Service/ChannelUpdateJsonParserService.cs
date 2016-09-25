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
    public class ChannelUpdateJsonParserService : IHomeUpdateJsonParserService
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


        public ChannelUpdateJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();
            _channelRepository = _unitOfWorkAsync.RepositoryAsync<Channel>();
            _channelStatusRepository = _unitOfWorkAsync.RepositoryAsync<ChannelStatus>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
        }

        public bool UpdateJsonData()
        {
            SetMapper();
            try
            {
                UpdateSmartSwitchCannel();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        private void UpdateSmartSwitchCannel()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            string deviceHash = _homeJsonEntity.Device.FirstOrDefault().DeviceHash;
            ChannelEntity channelEntity = _homeJsonEntity.Channel.FirstOrDefault();

            SmartSwitch sSwitch = null;


            sSwitch = new CommonService(_unitOfWorkAsync).GetSmartSwitchByDeviceHashAndPassPhrase<SmartSwitch>(deviceHash, passPhrase);
            sSwitch.Channels = new List<Channel>();

            if (sSwitch != null)
            {
                Channel channel = UpdateChannel(channelEntity, sSwitch);
                DeleteChannelStatus(channel);
                SaveChannelStatus(channelEntity, channel);
                sSwitch.Channels.Add(channel);
            }
        }
        private Channel UpdateChannel(ChannelEntity channelEntity, SmartSwitch dbSmartSwitch)
        {

            Channel dbChannel = _channelRepository.Queryable().Where(p => p.SmartSwitch.DeviceId == dbSmartSwitch.DeviceId && p.AppsChannelId == channelEntity.AppsChannelId).FirstOrDefault();

            var channel = SmartHomeTranslater.MapChannelInfoProperties(channelEntity, dbChannel);

            channel.ObjectState = ObjectState.Modified;
            channel.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            _channelRepository.Update(channel);

            return channel;
        }
        private void DeleteChannelStatus(Channel channel)
        {
            IList<ChannelStatus> dbChannelStatus = _channelStatusRepository.Queryable().Where(p => p.Channel.ChannelId == channel.ChannelId).ToList();

            foreach (var channelStatus in dbChannelStatus)
            {
                channelStatus.ObjectState = ObjectState.Deleted;
                _channelStatusRepository.Delete(channelStatus);
            }
        }
        private void SaveChannelStatus(ChannelEntity channelEntity, Channel channel)
        {
            try
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
            catch (Exception ex)
            {

                throw;
            }
        }
        private void SetMapper()
        {
            Mapper.CreateMap<ChannelEntity, Channel>();
            Mapper.CreateMap<ChannelStatusEntity, ChannelStatus>();
        }
    }
}
