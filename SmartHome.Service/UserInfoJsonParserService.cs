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
    public class UserInfoJsonParserService : IHomeJsonParserService
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
        public bool SaveJsonData()
        {
            IHomeJsonParserService service = null;
            bool isSuccess = false;
            try
            {
                string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
                string email = _homeJsonEntity.UserInfo.FirstOrDefault().Email;

                UserInfo userinfo = new CommonService(_unitOfWorkAsync).GetUserByEmail(email);

                if (userinfo != null)
                {
                    var updateService = new UserInfoUpdateJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.UpdateUser);
                    isSuccess = updateService.UpdateJsonData();
                }
                else
                {
                    service = new UserInfoNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.NewUser);
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
