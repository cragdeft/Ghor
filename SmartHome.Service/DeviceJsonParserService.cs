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
    public class DeviceJsonParserService : IHomeJsonParserService
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<Home> _homeRepository;
        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }

        #endregion
        public DeviceJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();

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
                if (_homeJsonEntity.Device.Count == 0)
                {
                    return isSuccess;
                }

                string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
                string deviceHash = _homeJsonEntity.Device.FirstOrDefault().DeviceHash;

                SmartDevice dbDevice = new CommonService(_unitOfWorkAsync).GetSmartDeviceByDeviceHashAndPassPhrase(deviceHash, passPhrase);

                if (dbDevice != null)
                {
                    var updateService = new DeviceUpdateJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.UpdateDevice);
                    isSuccess = updateService.UpdateJsonData();
                }
                else
                {
                    service = new DeviceNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.NewDevice);
                    isSuccess = service.SaveJsonData();
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