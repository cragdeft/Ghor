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
    public class RouterInfoUpdateJsonParserService : IHomeUpdateJsonParserService
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<Home> _homeRepository;
        private readonly IRepositoryAsync<RouterInfo> _routerInfoRepository;
        private readonly IRepositoryAsync<WebBrokerInfo> _webBrokerInfoRepository;
        private readonly IRepositoryAsync<UserHomeLink> _userHomeRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }
        public MessageLog _messageLog { get; private set; }

        #endregion
        public RouterInfoUpdateJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _routerInfoRepository = _unitOfWorkAsync.RepositoryAsync<RouterInfo>();
            _userHomeRepository = _unitOfWorkAsync.RepositoryAsync<UserHomeLink>();
            _webBrokerInfoRepository = _unitOfWorkAsync.RepositoryAsync<WebBrokerInfo>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
            _messageLog = new MessageLog();
        }
        public bool UpdateJsonData()
        {
            MessageLog messageLog = new CommonService(_unitOfWorkAsync).SaveMessageLog(_homeJsonMessage, _receivedFrom);

            _unitOfWorkAsync.BeginTransaction();
            SetMapper();

            try
            {
                UpdateSmartRouterInfo();
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
        private void UpdateSmartRouterInfo()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            string userEmail = _homeJsonEntity.UserInfo.FirstOrDefault().Email;
            RouterInfoEntity routerEntity = _homeJsonEntity.RouterInfo.FirstOrDefault();

            Home home = null;
            UserInfo userInfo = null;
            RouterInfo routerInfo = null;

            home = new CommonService(_unitOfWorkAsync).GetHome(passPhrase);
            userInfo = new CommonService(_unitOfWorkAsync).GetUserByEmail(userEmail);
            routerInfo = new CommonService(_unitOfWorkAsync).GetRouterInfoByPassPhraseAndAppsBleId(passPhrase, routerEntity.AppsBleId);

            if (routerInfo != null)
            {
                UpdateRouter(routerEntity, routerInfo);
                UpdateWebBrokerInfo(_homeJsonEntity.WebBrokerInfo[0], home);
                UpdateUserHomeLink(userInfo);
            }

        }

        private void UpdateWebBrokerInfo(WebBrokerInfoEntity webBrokerInfo, Home home)
        {
            WebBrokerInfo dbWebBrokerInfo = _webBrokerInfoRepository.Queryable().Where(p => p.Parent.HomeId == home.HomeId).FirstOrDefault();
            if (dbWebBrokerInfo != null)
            {
                WebBrokerInfo entity = SmartHomeTranslater.MapWebBrokerInfoProperties(webBrokerInfo, dbWebBrokerInfo);

                entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
                entity.ObjectState = ObjectState.Modified;
                _webBrokerInfoRepository.Update(entity);
            }
        }

        private void UpdateUserHomeLink(UserInfo userInfo)
        {
            var entity = _userHomeRepository.Queryable().Where(p => p.UserInfo.UserInfoId == userInfo.UserInfoId).FirstOrDefault();
            entity.IsAdmin = true;
            entity.ObjectState = ObjectState.Modified;
            _userHomeRepository.Update(entity);
        }
        private void UpdateRouter(RouterInfoEntity routerEntity, RouterInfo router)
        {
            var entity = SmartHomeTranslater.MapRouterInfoProperties(routerEntity, router);
            //entity.IsSynced = Convert.ToBoolean(routerEntity.IsSynced);
            //entity.IsExternal = Convert.ToBoolean(routerEntity.IsExternal);
            //entity.Parent = home;
            entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            entity.ObjectState = ObjectState.Modified;
            _routerInfoRepository.Update(entity);
        }

        private void SetMapper()
        {
            Mapper.CreateMap<RouterInfoEntity, RouterInfo>();
            Mapper.CreateMap<WebBrokerInfoEntity, WebBrokerInfo>();
        }
    }
}
