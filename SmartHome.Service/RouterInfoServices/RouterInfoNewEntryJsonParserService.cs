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
    public class RouterInfoNewEntryJsonParserService : IHomeJsonParserService<RouterInfo>
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
        public RouterInfoNewEntryJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
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
        public RouterInfo SaveJsonData()
        {
            RouterInfo routerInfo = null;
            SetMapper();
            try
            {
                routerInfo = SaveNewSmartRouterInfo();
            }
            catch (Exception ex)
            {
                return null;
            }
            return routerInfo;
        }
        private RouterInfo SaveNewSmartRouterInfo()
        {
            string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
            string userEmail = _homeJsonEntity.UserInfo.FirstOrDefault().Email;

            Home home = null;
            UserInfo userInfo = null;

            home = new CommonService(_unitOfWorkAsync).GetHome(passPhrase);
            userInfo = new CommonService(_unitOfWorkAsync).GetUserByEmail(userEmail);

            if (home != null && userInfo != null)
            {
                InsertWebBrokerInfo(_homeJsonEntity.WebBrokerInfo[0], home);
                UpdateUserHomeLink(userInfo);
                return InsertRouter(_homeJsonEntity.RouterInfo[0], home);
            }
            return null;

        }

        private void InsertWebBrokerInfo(WebBrokerInfoEntity webBrokerInfo, Home home)
        {
            var entity = Mapper.Map<WebBrokerInfoEntity, WebBrokerInfo>(webBrokerInfo);
            entity.IsSynced = Convert.ToBoolean(webBrokerInfo.IsSynced);
            entity.Parent = home;
            entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            entity.ObjectState = ObjectState.Added;
            _webBrokerInfoRepository.Insert(entity);
        }

        private void UpdateUserHomeLink(UserInfo userInfo)
        {
            var entity = _userHomeRepository.Queryable().Where(p => p.UserInfo.UserInfoId == userInfo.UserInfoId).FirstOrDefault();
            entity.IsAdmin = true;
            entity.ObjectState = ObjectState.Modified;
            _userHomeRepository.Update(entity);
        }
        public RouterInfo InsertRouter(RouterInfoEntity router, Home home)
        {
            var entity = Mapper.Map<RouterInfoEntity, RouterInfo>(router);
            entity.IsSynced = Convert.ToBoolean(router.IsSynced);
            entity.IsExternal = Convert.ToBoolean(router.IsExternal);
            entity.Parent = home;
            entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            entity.ObjectState = ObjectState.Added;
            _routerInfoRepository.Insert(entity);
            return entity;
        }

        private void SetMapper()
        {
            Mapper.CreateMap<RouterInfoEntity, RouterInfo>();
            Mapper.CreateMap<WebBrokerInfoEntity, WebBrokerInfo>();
        }
    }
}
