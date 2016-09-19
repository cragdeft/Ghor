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
    public class DeviceDeleteJsonParserService : IHomeDeleteJsonParserService
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

        public bool DeleteJsonData()
        {
            MessageLog messageLog = new CommonService(_unitOfWorkAsync).SaveMessageLog(_homeJsonMessage, _receivedFrom);

            _unitOfWorkAsync.BeginTransaction();
            try
            {
                DeleteSmartDevice();
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
        private void DeleteSmartDevice()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            string deviceHash = _homeJsonEntity.Device.FirstOrDefault().DeviceHash;

            SmartDevice smartDevice = null;
            Home home = null;

            smartDevice = GetSmartDeviceByDeviceHashAndPassPhrase(deviceHash, passPhrase);
            home = _homeRepository.Queryable().Where(p => p.PassPhrase == passPhrase).FirstOrDefault();

            if (smartDevice != null)
            {
                if (smartDevice.DeviceType == DeviceType.SmartRouter)
                {
                    DeleteRouterInfo(home, smartDevice.AppsBleId);
                    UpdateUserHomeLink(home);
                }
                DeleteDevice(smartDevice);
            }
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

        private void DeleteDevice(SmartDevice smartDevice)
        {
            smartDevice.ObjectState = ObjectState.Deleted;
            _deviceRepository.Delete(smartDevice);
        }
        private SmartDevice GetSmartDeviceByDeviceHashAndPassPhrase(string deviceHash, string passPhrase)
        {
            return _homeRepository.Queryable().Where(p => p.PassPhrase == passPhrase)
              .SelectMany(p => p.Rooms)
              .SelectMany(q => q.SmartDevices.Where(s => s.DeviceHash == deviceHash))
              .FirstOrDefault();
        }
    }
}