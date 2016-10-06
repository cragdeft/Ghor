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
    public class ConfigurationJsonParserService : IHomeJsonParserService<Home>
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<Home> _homeRepository;
        private readonly IRepositoryAsync<NextAssociatedDevice> _nextAssociatedDeviceRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }

        #endregion


        public ConfigurationJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();
            _nextAssociatedDeviceRepository = _unitOfWorkAsync.RepositoryAsync<NextAssociatedDevice>();

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

                home = SaveHomeAndRouter();

                #endregion


            }
            catch (Exception ex)
            {
                return null;
            }
            return home;
        }

        private Home SaveHomeAndRouter()
        {
            string passPhrase = _homeJsonEntity.Home[0].PassPhrase;

            Home home = null;
            home = new CommonService(_unitOfWorkAsync).GetHomeByPassPhrase(passPhrase);
            HomeEntity homeEntity = _homeJsonEntity.Home.FirstOrDefault();

            home = InsertOrUpdateHome(home, homeEntity);

            InsertOrUpdateRouter(home);

            home = SaveUsersAndRelatedInfos(home);

            SaveOrUpdateDevice(home);
            SaveOrUpdateNextAssociatedDevice(home);
            new VersionInfoJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).SaveOrUpdateVersion(home);
            return home;
        }

        private Home SaveUsersAndRelatedInfos(Home home)
        {
            IList<UserInfo> listOfUsers = SaveOrUpdateUser();
            IList<UserInfo> listOfExistingDbUsers = new UserInfoNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).DeleteUser(home, listOfUsers);

            home = SaveOrUpdateRoom(home, listOfUsers, listOfExistingDbUsers);
            SaveHomeUser(home, listOfUsers, listOfExistingDbUsers);
            return home;
        }

        private Home InsertOrUpdateHome(Home home, HomeEntity homeEntity)
        {
            if (home == null)
            {
                home = new HomeInfoNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).InsertHome(homeEntity);
            }
            else
            {
                home = new HomeInfoUpdateJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).UpdateHome(homeEntity, home); 
            }

            return home;
        }

        private void SaveOrUpdateNextAssociatedDevice(Model.Models.Home home)
        {
            var nextDevice = _nextAssociatedDeviceRepository
                .Queryable().Where(w => w.Home.HomeId == home.HomeId).FirstOrDefault();

            if (nextDevice == null)
            {
                nextDevice = new NextAssociatedDevice();
                nextDevice.NextDeviceId = _homeJsonEntity.NextAssociatedDeviceId[0].NextDeviceId;
                nextDevice.Home = home;
                nextDevice.ObjectState = ObjectState.Added;
                _nextAssociatedDeviceRepository.Insert(nextDevice);
            }
            else
            {
                nextDevice.NextDeviceId = _homeJsonEntity.NextAssociatedDeviceId[0].NextDeviceId;
                nextDevice.ObjectState = ObjectState.Modified;
                _nextAssociatedDeviceRepository.Update(nextDevice);
            }
        }

        private void SaveOrUpdateDevice(Home model)
        {
            foreach (var room in model.Rooms)
            {
                List<SmartDeviceEntity> deviceList = _homeJsonEntity.Device.FindAll(x => x.AppsRoomId == room.AppsRoomId);

                foreach (var smartDevice in deviceList)
                {
                    new DeviceNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).InsertDevice(smartDevice, room);
                }
            }
        }

        private void SaveHomeUser(Home home, IList<UserInfo> listOfUsers, IList<UserInfo> listOfExistingDbUsers)
        {
            var homeUserList = _homeJsonEntity.UserHomeLink.FindAll(x => x.AppsHomeId == home.AppsHomeId);

            foreach (var userRoomLinkEntity in homeUserList)
            {
                UserInfoEntity userentity =
                    _homeJsonEntity.UserInfo.Find(x => x.AppsUserId == userRoomLinkEntity.AppsUserId);

                UserHomeLink userHome = new UserHomeLink();
                userHome.UserInfo = listOfUsers.Where(u => u.Email == userentity.Email).FirstOrDefault();

                new UserInfoNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).FillSaveHomeUser(home, userRoomLinkEntity, userHome);
            }
            if (listOfExistingDbUsers.Count > 0)
            {
                new UserInfoNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).SaveHomeForExistingDbUsers(home, listOfExistingDbUsers, homeUserList);
            }

        }

        private Home SaveOrUpdateRoom(Home model, IList<UserInfo> listOfUsers, IList<UserInfo> listOfExistingDbUsers)
        {
            if (model.Rooms != null)
            {
                new RoomDeleteJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).DeleteAllRooms(model);
            }
            return new RoomNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).InsertAllRooms(model, listOfUsers, listOfExistingDbUsers);
        }



        private void InsertOrUpdateRouter(Home home)
        {
            RouterInfo router = new CommonService(_unitOfWorkAsync).GetRouterByHomeId(home);

            if (_homeJsonEntity.RouterInfo.Count > 0)
            {
                if (router == null)
                {
                    new RouterInfoNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).InsertRouter(_homeJsonEntity.RouterInfo[0], home);
                }
                else
                {
                    new RouterInfoUpdateJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).UpdateRouter(_homeJsonEntity.RouterInfo[0], router);
                }
            }

            if (_homeJsonEntity.RouterInfo.Count == 0 && router != null)
            {
                new RouterInfoUpdateJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).DeleteRouter(router);
            }

        }

        private IList<UserInfo> SaveOrUpdateUser()
        {
            IList<UserInfo> listOfUsers = new List<UserInfo>();
            foreach (var userInfoEntity in _homeJsonEntity.UserInfo)
            {
                var dbUserEntity = new CommonService(_unitOfWorkAsync).GetUserByEmail(userInfoEntity.Email);

                if (dbUserEntity == null)
                {
                    listOfUsers.Add(new UserInfoNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).InsertUser(userInfoEntity));
                }
                else
                {
                    listOfUsers.Add(new UserInfoUpdateJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, _receivedFrom).UpdateUser(userInfoEntity, dbUserEntity));
                }
            }
            return listOfUsers;
        }
    }
}
