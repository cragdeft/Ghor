using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Mappers;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.Models;

namespace SmartHome.Service
{
    public class HomeJsonParserService : IHomeJsonParserService
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<SmartRouter> _routerRepository;
        private readonly IRepositoryAsync<Home> _homeRepository;
        private readonly IRepositoryAsync<Room> _roomRepository;
        private readonly IRepositoryAsync<UserInfo> _userRepository;
        private readonly IRepositoryAsync<SmartDevice> _deviceRepository;
        private readonly IRepositoryAsync<UserRoomLink> _userRoomRepository;
        private readonly IRepositoryAsync<DeviceStatus> _deviceStatusRepository;
        private readonly IRepositoryAsync<Channel> _channelRepository;
        private readonly IRepositoryAsync<ChannelStatus> _channelStatusRepository;
        private readonly IRepositoryAsync<UserRoomLink> _userRoomLinkRepository;
        private readonly IRepositoryAsync<NextAssociatedDevice> _nextAssociatedDeviceRepository;
        public HomeJsonEntity _homeJsonEntity { get; private set; }
        private string _email;
        #endregion

        public HomeJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _routerRepository = _unitOfWorkAsync.RepositoryAsync<SmartRouter>();
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();
            _roomRepository = _unitOfWorkAsync.RepositoryAsync<Room>();
            _userRepository = _unitOfWorkAsync.RepositoryAsync<UserInfo>();
            _deviceRepository = _unitOfWorkAsync.RepositoryAsync<SmartDevice>();
            _userRoomRepository = _unitOfWorkAsync.RepositoryAsync<UserRoomLink>();
            _deviceStatusRepository = _unitOfWorkAsync.RepositoryAsync<DeviceStatus>();
            _channelRepository = _unitOfWorkAsync.RepositoryAsync<Channel>();
            _channelStatusRepository = _unitOfWorkAsync.RepositoryAsync<ChannelStatus>();
            _userRoomLinkRepository = _unitOfWorkAsync.RepositoryAsync<UserRoomLink>();
            _nextAssociatedDeviceRepository = _unitOfWorkAsync.RepositoryAsync<NextAssociatedDevice>();
            _homeJsonEntity = homeJsonEntity;
        }
        public SmartRouterEntity GetRouter(string macAddress)
        {
            SmartRouter router = _routerRepository
                .Queryable().Include(x => x.Parent).Where(u => u.MacAddress == macAddress).FirstOrDefault();

            Mapper.CreateMap<Home, HomeEntity>();
            Mapper.CreateMap<SmartRouter, SmartRouterEntity>();
            return Mapper.Map<SmartRouter, SmartRouterEntity>(router);
        }

        public HomeEntity GetHome(int homeId)
        {
            Home home = _homeRepository
                .Queryable().Include(x => x.Rooms).Where(u => u.HomeId == homeId).FirstOrDefault();

            Mapper.CreateMap<Home, HomeEntity>();
            return Mapper.Map<Home, HomeEntity>(home);
        }

        public UserInfoEntity GetUser(string email)
        {
            UserInfo user = _userRepository
                .Queryable().Where(u => u.Email == email).FirstOrDefault();

            Mapper.CreateMap<UserInfo, UserInfoEntity>();
            return Mapper.Map<UserInfo, UserInfoEntity>(user);
        }

        public Home InsertHome(HomeEntity home)
        {
            Home model = Mapper.Map<HomeEntity, Home>(home);
            model.ObjectState = ObjectState.Added;
            model.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            _homeRepository.Insert(model);
            return model;
        }

        public void InsertDevice(SmartDeviceEntity device, Room room)
        {
            SmartDevice model = Mapper.Map<SmartDeviceEntity, SmartDevice>(device);

            List<DeviceStatusEntity> deviceStatuses =
                    _homeJsonEntity.DeviceStatus.FindAll(x => x.DId == device.AppsDeviceId.ToString());

            if (device.DeviceType == DeviceType.SmartSwitch6g)
            {
                SmartSwitch sswitch = MapToSmartSwitch(model);
                sswitch.Room = room;
                sswitch.ObjectState = ObjectState.Added;
                sswitch.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
                _deviceRepository.Insert(sswitch);

                foreach (var deviceStatusEntity in deviceStatuses)
                {
                    var entity = Mapper.Map<DeviceStatusEntity, DeviceStatus>(deviceStatusEntity);
                    entity.ObjectState = ObjectState.Added;
                    entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
                    _deviceStatusRepository.Insert(entity);
                    sswitch.DeviceStatus.Add(entity);
                }


                List<ChannelEntity> channelList = _homeJsonEntity.Channel.FindAll(x => x.AppsDeviceTableId == device.AppsDeviceId);
                InsertChannel(sswitch, channelList);
            }

        }

        private SmartSwitch MapToSmartSwitch(SmartDevice model)
        {
            SmartSwitch entity = new SmartSwitch();
            entity.DeviceId = model.DeviceId;
            entity.AppsDeviceId = model.AppsDeviceId;
            entity.AppsBleId = model.AppsBleId;
            entity.DeviceName = model.DeviceName;
            entity.DeviceHash = model.DeviceHash;
            entity.DeviceVersion = model.DeviceVersion;
            entity.IsDeleted = model.IsDeleted;
            entity.Watt = model.Watt;
            //entity.Mac = model.Mac;
            //entity.DType = model.DType;
            entity.DeviceType = model.DeviceType;
            entity.DeviceStatus = new List<DeviceStatus>();
            return entity;
        }

        public void InsertChannel(SmartSwitch model, List<ChannelEntity> channels)
        {

            SmartSwitch sswitch = model;
            sswitch.Channels = new List<Channel>();
            foreach (var channel in channels)
            {
                var entity = Mapper.Map<ChannelEntity, Channel>(channel);

                entity.ObjectState = ObjectState.Added;
                entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
                _channelRepository.Insert(entity);
                entity.ChannelStatuses = new List<ChannelStatus>();
                List<ChannelStatusEntity> channelStatuses =
                    _homeJsonEntity.ChannelStatus.FindAll(x => x.CId == channel.AppsChannelId);

                foreach (var channelStatusEntity in channelStatuses)
                {
                    var channelStatusModel = Mapper.Map<ChannelStatusEntity, ChannelStatus>(channelStatusEntity);
                    channelStatusModel.ObjectState = ObjectState.Added;
                    channelStatusModel.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
                    _channelStatusRepository.Insert(channelStatusModel);
                    entity.ChannelStatuses.Add(channelStatusModel);
                }

                sswitch.Channels.Add(entity);
            }

        }

        public Home UpdateHome(HomeEntity home)
        {
            Home model = MapHomeProperty(home);
            model.ObjectState = ObjectState.Modified;
            _homeRepository.Update(model);
            return model;

        }

        public void InsertRouter(SmartRouterEntity router, Home home)
        {
            var entity = Mapper.Map<SmartRouterEntity, SmartRouter>(router);
            entity.Parent = home;
            entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            entity.ObjectState = ObjectState.Added;
            _routerRepository.Insert(entity);
        }

        public void UpdateRouter(SmartRouterEntity router)
        {
            var entity = MapRouterProperty(router);
            entity.ObjectState = ObjectState.Modified;
            _routerRepository.Update(entity);
        }

        public bool SaveJsonData()
        {
            _unitOfWorkAsync.BeginTransaction();
            SetMapper();
            try
            {
                SaveHomeAndRouter();
                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWorkAsync.Rollback();
                return false;
            }

            return true;
        }

        private void SaveHomeAndRouter()
        {
            SmartRouterEntity router = GetRouter(_homeJsonEntity.RouterInfo[0].MacAddress);
            HomeEntity home = null;
            if (router != null)
            {
                home = GetHome(router.Parent.HomeId);
                _homeJsonEntity.Home[0].HomeId = router.Parent.HomeId;
            }

            Home model = SaveOrUpdateHome(home);
            SaveOrUpdateUser();

            SaveOrUpdateRouter(router, model);
            model = SaveOrUpdateRoom(model);
            //AddUserHomeLink();
            SaveOrUpdateDevice(model);
            SaveOrUpdateNextAssociatedDevice();
        }

        private void SaveOrUpdateNextAssociatedDevice()
        {
            var nextDevice = _nextAssociatedDeviceRepository
                .Queryable().FirstOrDefault();

            nextDevice.NextDeviceId = _homeJsonEntity.NextAssociatedDeviceId[0].NextDeviceId;

            if (nextDevice == null)
            {
                nextDevice.ObjectState = ObjectState.Added;
                _nextAssociatedDeviceRepository.Insert(nextDevice);
            }
            else
            {
                nextDevice.ObjectState = ObjectState.Added;
                _nextAssociatedDeviceRepository.Insert(nextDevice);
            }


        }

        private void AddUserRoomLink()
        {
            foreach (var userRoomLinkEntity in _homeJsonEntity.UserRoomLink)
            {
                UserInfoEntity userInfo = _homeJsonEntity.UserInfo.Find(x => x.Id == userRoomLinkEntity.User);
                UserInfo user = _userRepository
                .Queryable().Where(u => u.Email == userInfo.Email).FirstOrDefault();

                if (user.UserRoomLinks != null && user.UserRoomLinks.Count > 0)
                {
                    var userRoomLinkList = user.UserRoomLinks.ToList().FindAll(x => x.UserInfo.Email == userInfo.Email);
                    foreach (var b in userRoomLinkList)
                    {
                        b.UserInfo = null;
                        b.ObjectState = ObjectState.Modified;
                        _userRoomLinkRepository.Insert(b);

                    }
                }
            }
        }

        private void SaveOrUpdateUser()
        {
            foreach (var userInfoEntity in _homeJsonEntity.UserInfo)
            {
                var dbUserEntity = GetUser(userInfoEntity.Email);
                if (dbUserEntity == null)
                    InsertUser(userInfoEntity);
                else
                {
                    UpdateUser(userInfoEntity);
                }
            }
        }

        private UserInfo UpdateUser(UserInfoEntity userInfoEntity)
        {
            var entity = MapUserProperty(userInfoEntity);
            entity.ObjectState = ObjectState.Modified;
            _userRepository.Update(entity);
            DeleteRoomUser(entity);
            return entity;
        }

        private void DeleteRoomUser(UserInfo entity)
        {
            if (entity.UserRoomLinks != null && entity.UserRoomLinks.Count > 0)
                foreach (var userRoomLink in entity.UserRoomLinks)
                {
                    userRoomLink.UserInfo = null;
                    userRoomLink.ObjectState = ObjectState.Modified;
                    _userRoomRepository.Delete(userRoomLink);
                }
        }

        private UserInfo InsertUser(UserInfoEntity userInfoEntity)
        {
            UserInfo entity = Mapper.Map<UserInfoEntity, UserInfo>(userInfoEntity);
            entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            entity.ObjectState = ObjectState.Added;
            _userRepository.Insert(entity);
            return entity;
        }

        private void SaveOrUpdateDevice(Home model)
        {
            foreach (var room in model.Rooms)
            {
                List<SmartDeviceEntity> deviceList = _homeJsonEntity.Device.FindAll(x => x.AppsRoomId.ToString() == room.AppsRoomId);

                foreach (var smartDevice in deviceList)
                {
                    InsertDevice(smartDevice, room);
                }
            }
        }

        private void DeleteAllDevices()
        {
            throw new NotImplementedException();
        }

        private Home SaveOrUpdateRoom(Home model)
        {
            if (model.Rooms != null)
            {
                DeleteAllRooms(model.Rooms);
            }
            return InsertAllRooms(model);
        }

        private Home InsertAllRooms(Home home)
        {
            home.Rooms = new List<Room>();
            foreach (var room in _homeJsonEntity.Room)
            {
                var entity = Mapper.Map<RoomEntity, Room>(room);
                entity.Home = home;
                entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
                entity.ObjectState = ObjectState.Added;
                _roomRepository.Insert(entity);
                SaveRoomUser(entity);
                home.Rooms.Add(entity);
            }

            return home;
        }

        private void SaveRoomUser(Room entity)
        {
            var roomLinkList = _homeJsonEntity.UserRoomLink.FindAll(x => x.Room == entity.AppsRoomId);
            if (roomLinkList != null && roomLinkList.Count > 0)
            {
                foreach (var userRoomLinkEntity in roomLinkList)
                {
                    UserInfoEntity userentity =
                        _homeJsonEntity.UserInfo.Find(x => x.Id.ToString() == userRoomLinkEntity.User.ToString());
                    UserRoomLink userRoom = new UserRoomLink();
                    userRoom.UserInfo = _userRepository
                .Queryable().Where(u => u.Email == userentity.Email).FirstOrDefault();
                    userRoom.Room = entity;
                    userRoom.IsSynced = userRoomLinkEntity.IsSynced;
                    userRoom.AppsUserRoomLinkId = userRoomLinkEntity.AppsUserRoomLinkId;
                    userRoom.ObjectState = ObjectState.Added;
                    _userRoomLinkRepository.Insert(userRoom);
                }
            }


        }

        private void DeleteAllRooms(ICollection<Room> rooms)
        {
            foreach (var room in rooms)
            {
                room.ObjectState = ObjectState.Deleted;
                _roomRepository.Delete(room);
            }
        }


        private void SaveOrUpdateRouter(SmartRouterEntity router, Home home)
        {
            if (router == null)
            {
                InsertRouter(_homeJsonEntity.RouterInfo[0], home);
            }
            else
            {
                UpdateRouter(_homeJsonEntity.RouterInfo[0]);
            }
        }

        private Home SaveOrUpdateHome(HomeEntity home)
        {
            if (home == null)
            {
                return InsertHome(_homeJsonEntity.Home[0]);
            }
            else
            {
                return UpdateHome(_homeJsonEntity.Home[0]);
            }

        }

        #region Mapping
        private void SetMapper()
        {
            Mapper.CreateMap<HomeEntity, Home>();
            Mapper.CreateMap<SmartRouterEntity, SmartRouter>();
            Mapper.CreateMap<RoomEntity, Room>();
            Mapper.CreateMap<UserInfoEntity, UserInfo>();
            Mapper.CreateMap<SmartDeviceEntity, SmartDevice>();
            Mapper.CreateMap<ChannelStatusEntity, ChannelStatus>();
            Mapper.CreateMap<DeviceStatusEntity, DeviceStatus>();
            Mapper.CreateMap<ChannelEntity, Channel>();
        }

        private Home MapHomeProperty(HomeEntity home)
        {
            Home model = _homeRepository
               .Queryable().Where(u => u.HomeId == home.HomeId).FirstOrDefault();

            model.Address1 = home.Address1;
            model.Address2 = home.Address2;
            model.AppsHomeId = home.AppsHomeId;
            model.AuditField.LastUpdatedBy = "admin";
            model.AuditField.LastUpdatedDateTime = DateTime.Now;
            model.Block = home.Block;
            model.HomeId = home.HomeId;
            model.City = home.City;
            model.Country = home.Country;
            model.IsActive = home.IsActive;
            model.IsDefault = home.IsDefault;
            model.IsInternet = home.IsInternet;
            model.IsSynced = home.IsSynced;
            model.MeshMode = (MeshModeType)home.MeshMode;
            model.PassPhrase = home.PassPhrase;
            model.Phone = home.Phone;
            model.TimeZone = home.TimeZone;
            model.ZipCode = home.ZipCode;

            return model;
        }

        private SmartRouter MapRouterProperty(SmartRouterEntity router)
        {
            SmartRouter model = _routerRepository
               .Queryable().Where(u => u.MacAddress == router.MacAddress).FirstOrDefault();

            model.MacAddress = router.MacAddress;
            model.Ssid = router.Ssid;
            model.AppsRouterId = router.AppsRouterId;
            model.AuditField.LastUpdatedBy = "admin";
            model.AuditField.LastUpdatedDateTime = DateTime.Now;
            model.IsSynced = router.IsSynced;
            model.LocalBrokerIp = router.LocalBrokerIp;
            model.LocalBrokerPassword = router.LocalBrokerPassword;
            model.LocalBrokerPort = router.LocalBrokerPort;
            model.LocalBrokerUsername = router.LocalBrokerUsername;

            return model;
        }

        private UserInfo MapUserProperty(UserInfoEntity userEntity)
        {
            UserInfo model = _userRepository
               .Queryable().Where(u => u.Email == userEntity.Email).FirstOrDefault();

            model.Id = userEntity.Id.ToString();
            model.LoginStatus = userEntity.LoginStatus;
            model.Password = userEntity.Password;
            model.RegStatus = userEntity.RegStatus;
            model.UserName = userEntity.UserName;
            model.IsSynced = userEntity.IsSynced;
            model.Country = userEntity.Country;
            model.CellPhone = userEntity.CellPhone;
            model.Sex = userEntity.Sex;
            model.AuditField.LastUpdatedBy = "admin";
            model.AuditField.LastUpdatedDateTime = DateTime.Now;

            return model;
        }
        #endregion
    }
}
