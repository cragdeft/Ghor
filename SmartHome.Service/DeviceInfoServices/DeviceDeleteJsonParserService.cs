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
    public class DeviceDeleteJsonParserService : IHomeDeleteJsonParserService<SmartDevice>
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<Home> _homeRepository;
        private readonly IRepositoryAsync<SmartDevice> _deviceRepository;
        private readonly IRepositoryAsync<RouterInfo> _routerInfoRepository;
        private readonly IRepositoryAsync<UserHomeLink> _userHomeRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }

        #endregion

        public DeviceDeleteJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();
            _deviceRepository = _unitOfWorkAsync.RepositoryAsync<SmartDevice>();
            _routerInfoRepository = _unitOfWorkAsync.RepositoryAsync<RouterInfo>();
            _userHomeRepository = _unitOfWorkAsync.RepositoryAsync<UserHomeLink>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
        }

        public SmartDevice DeleteJsonData()
        {
            SmartDevice smartDevice = null;
            try
            {
                smartDevice = DeleteSmartDevice();
            }
            catch (Exception ex)
            {
                return null;
            }
            return smartDevice;
        }
        private SmartDevice DeleteSmartDevice()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            string deviceHash = _homeJsonEntity.Device.FirstOrDefault().DeviceHash;

            SmartDevice smartDevice = null;
            Home home = null;

            smartDevice = new CommonService(_unitOfWorkAsync).GetSmartDeviceByDeviceHashAndPassPhrase(deviceHash, passPhrase);
            home = new CommonService(_unitOfWorkAsync).GetHome(passPhrase);

            if (smartDevice != null)
            {
                if (smartDevice.DeviceType == DeviceType.SmartRouter)
                {
                    DeleteRouterInfo(home, smartDevice.AppsBleId);
                    UpdateUserHomeLink(home);
                }
                return DeleteDevice(smartDevice);

            }
            return null;
        }

        private void UpdateUserHomeLink(Home home)
        {
            IList<UserHomeLink> userHomeLinks = _userHomeRepository.Queryable().Where(p => p.Home.HomeId == home.HomeId).ToList();
            foreach (var userHomeLink in userHomeLinks)
            {
                userHomeLink.IsAdmin = false;
                userHomeLink.ObjectState = ObjectState.Modified;
                _userHomeRepository.Update(userHomeLink);
            }
        }

        private void DeleteRouterInfo(Home home, int appsBleId)
        {

            RouterInfo router = _routerInfoRepository.Queryable().Where(p => p.Parent.HomeId == home.HomeId && p.AppsBleId == appsBleId).FirstOrDefault();

            router.ObjectState = ObjectState.Deleted;
            _routerInfoRepository.Delete(router);
        }

        private SmartDevice DeleteDevice(SmartDevice smartDevice)
        {
            smartDevice.ObjectState = ObjectState.Deleted;
            _deviceRepository.Delete(smartDevice);
            return smartDevice;
        }

    }
}