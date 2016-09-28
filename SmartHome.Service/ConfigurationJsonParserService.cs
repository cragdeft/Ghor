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
    public class ConfigurationJsonParserService : IHomeJsonParserService<Home>
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


        public ConfigurationJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();
            _channelRepository = _unitOfWorkAsync.RepositoryAsync<Channel>();
            _channelStatusRepository = _unitOfWorkAsync.RepositoryAsync<ChannelStatus>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
        }

        public Home SaveJsonData()
        {
            Home home = null;
            try
            {
                if (_homeJsonEntity.Home.Count == 0)
                {
                    return home;
                }

                #region MyRegion

            

                //IList<UserInfo> listOfUsers = SaveOrUpdateUser();
                //IList<UserInfo> listOfExistingDbUsers = DeleteUser(home, listOfUsers);
                //home = SaveOrUpdateRoom(home, listOfUsers, listOfExistingDbUsers);
                //SaveHomeUser(home, listOfUsers, listOfExistingDbUsers);
                //SaveOrUpdateDevice(home);
                //SaveOrUpdateNextAssociatedDevice(home);
                //SaveOrUpdateVersion(home);


                #endregion


            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }


    }
}
