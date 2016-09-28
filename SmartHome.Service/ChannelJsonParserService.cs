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
    public class ChannelJsonParserService : IHomeJsonParserService
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

        public bool SaveJsonData()
        {
            IHomeJsonParserService service = null;
            bool isSuccess = false;
            try
            {
                if (_homeJsonEntity.Channel.Count == 0)
                {
                    return isSuccess;
                }

                string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
                string deviceHash = _homeJsonEntity.Device.FirstOrDefault().DeviceHash;
                int appsChannelId = _homeJsonEntity.Channel.FirstOrDefault().AppsChannelId;

                SmartSwitch sSwitch = null;
                Channel dbChannel = null;

                sSwitch = new CommonService(_unitOfWorkAsync).GetSmartSwitchByDeviceHashAndPassPhrase<SmartSwitch>(deviceHash, passPhrase);
                if (sSwitch != null)
                {
                    //    dbChannel = _channelRepository.Queryable().Where(p => p.SmartSwitch.DeviceId == sSwitch.DeviceId).FirstOrDefault();
                    dbChannel = _channelRepository.Queryable().Where(p => p.SmartSwitch.DeviceId == sSwitch.DeviceId && p.AppsChannelId == appsChannelId).FirstOrDefault();

                    if (dbChannel != null)
                    {
                        var updateService = new ChannelUpdateJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.UpdateChannel);
                        isSuccess = updateService.UpdateJsonData();
                    }
                    else
                    {
                        service = new ChannelNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.NewChannel);
                        isSuccess = service.SaveJsonData();
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }
            return isSuccess;
        }


    }
}
