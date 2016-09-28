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
    public class ChannelJsonParserService : IHomeJsonParserService<Channel>
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


        public ChannelJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();
            _channelRepository = _unitOfWorkAsync.RepositoryAsync<Channel>();
            _channelStatusRepository = _unitOfWorkAsync.RepositoryAsync<ChannelStatus>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
        }

        public Channel SaveJsonData()
        {
            //IHomeJsonParserService service = null;
            Channel channel = null;
            try
            {
                if (_homeJsonEntity.Channel.Count == 0)
                {
                    return channel;
                }

                string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
                string deviceHash = _homeJsonEntity.Device.FirstOrDefault().DeviceHash;
                int appsChannelId = _homeJsonEntity.Channel.FirstOrDefault().AppsChannelId;

                SmartSwitch sSwitch = null;
                Channel dbChannel = null;

                sSwitch = new CommonService(_unitOfWorkAsync).GetSmartSwitchByDeviceHashAndPassPhrase<SmartSwitch>(deviceHash, passPhrase);
                if (sSwitch != null)
                {
                    dbChannel = _channelRepository.Queryable().Where(p => p.SmartSwitch.DeviceId == sSwitch.DeviceId && p.AppsChannelId == appsChannelId).FirstOrDefault();

                    if (dbChannel != null)
                    {
                        IHomeUpdateJsonParserService<Channel> updateService = new ChannelUpdateJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.UpdateChannel);
                        channel = updateService.UpdateJsonData();
                    }
                    else
                    {
                        IHomeJsonParserService<Channel> service = new ChannelNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.NewChannel);
                        channel = service.SaveJsonData();
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }
            return channel;
        }


    }
}
