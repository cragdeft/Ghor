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
    public class UserInfoJsonParserService : IHomeJsonParserService<UserInfo>
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<UserInfo> _userRepository;
        private readonly IRepositoryAsync<UserRoomLink> _userRoomLinkRepository;
        private readonly IRepositoryAsync<UserHomeLink> _userHomeRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }

        #endregion

        public UserInfoJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _userRepository = _unitOfWorkAsync.RepositoryAsync<UserInfo>();
            _userRoomLinkRepository = _unitOfWorkAsync.RepositoryAsync<UserRoomLink>();
            _userHomeRepository = _unitOfWorkAsync.RepositoryAsync<UserHomeLink>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
        }
        public UserInfo SaveJsonData()
        {
            UserInfo userInfo = null;
            try
            {
                if (_homeJsonEntity.UserInfo.Count == 0)
                {
                    return userInfo;
                }

                string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
                string email = _homeJsonEntity.UserInfo.FirstOrDefault().Email;

                UserInfo userinfo = new CommonService(_unitOfWorkAsync).GetUserByEmail(email);

                if (userinfo != null)
                {
                    IHomeUpdateJsonParserService<UserInfo> updateService = new UserInfoUpdateJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.UpdateUser);
                    userInfo = updateService.UpdateJsonData();
                }
                else
                {
                    IHomeJsonParserService<UserInfo> service = new UserInfoNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.NewUser);
                    userInfo = service.SaveJsonData();
                }


            }
            catch (Exception ex)
            {
                return null;
            }
            return userInfo;
        }
    }
}
