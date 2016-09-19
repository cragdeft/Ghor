﻿using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SmartHome.Service
{
    public class ChannelDeleteJsonParserService : IHomeDeleteJsonParserService
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<Home> _homeRepository;
        private readonly IRepositoryAsync<Channel> _channelRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }

        #endregion


        public ChannelDeleteJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();
            _channelRepository = _unitOfWorkAsync.RepositoryAsync<Channel>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
        }

        public bool DeleteJsonData()
        {
            MessageLog messageLog = new CommonService(_unitOfWorkAsync).SaveMessageLog(_homeJsonMessage, _receivedFrom);

            _unitOfWorkAsync.BeginTransaction();
            try
            {
                DeleteSmartSwitchCannel();
                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWorkAsync.Rollback();
                return false;
            }

            new CommonService(_unitOfWorkAsync).UpdateMessageLog(messageLog, _homeJsonEntity.Home[0].PassPhrase);

            return true;
        }

        private void DeleteSmartSwitchCannel()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            string deviceHash = _homeJsonEntity.Device.FirstOrDefault().DeviceHash;
            //int appsChannelId = _homeJsonEntity.Channel.FirstOrDefault().AppsChannelId;
            int channelNo = _homeJsonEntity.Channel.FirstOrDefault().ChannelNo;

            Channel channel = GetChannel(passPhrase, deviceHash, channelNo);

            if (channel != null)
            {
                DeleteChannel(channel);
            }
        }

        private Channel GetChannel(string passPhrase, string deviceHash, int channelNo)
        {
            return _homeRepository.Queryable().Where(p => p.PassPhrase == passPhrase)
                .SelectMany(p => p.Rooms)
                .SelectMany(q => q.SmartDevices.OfType<SmartSwitch>().Where(s => s.DeviceHash == deviceHash))
                .SelectMany(d => d.Channels.Where(c => c.ChannelNo == channelNo))
                .FirstOrDefault();
        }

        private void DeleteChannel(Channel channel)
        {
            channel.ObjectState = ObjectState.Deleted;
            _channelRepository.Delete(channel);

        }


    }
}
