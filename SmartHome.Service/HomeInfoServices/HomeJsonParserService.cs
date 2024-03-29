﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Sockets;
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
using SmartHome.Service.Interfaces;
using Version = SmartHome.Model.Models.Version;

namespace SmartHome.Service
{
  public class HomeJsonParserService : IHomeJsonParserService<Home>
  {
    #region PrivateProperty
    private readonly IUnitOfWorkAsync _unitOfWorkAsync;
    private readonly IRepositoryAsync<SmartRouter> _routerRepository;
    private readonly IRepositoryAsync<RouterInfo> _routerInfoRepository;
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
    private readonly IRepositoryAsync<RgbwStatus> _rgbwStatusRepository;
    private readonly IRepositoryAsync<UserHomeLink> _userHomeRepository;
    private readonly IRepositoryAsync<Version> _versionRepository;
    private readonly IRepositoryAsync<VersionDetail> _versionDetailRepository;
    private readonly IRepositoryAsync<MessageLog> _mqttMessageLogRepository;
    public HomeJsonEntity _homeJsonEntity { get; private set; }
    public string _homeJsonMessage { get; private set; }

    public MessageReceivedFrom _receivedFrom { get; private set; }

    public MessageLog _messageLog { get; private set; }

    private string _email;
    #endregion

    public HomeJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
    {
      _unitOfWorkAsync = unitOfWorkAsync;
      _routerRepository = _unitOfWorkAsync.RepositoryAsync<SmartRouter>();
      _routerInfoRepository = _unitOfWorkAsync.RepositoryAsync<RouterInfo>();
      _homeRepository = _unitOfWorkAsync.RepositoryAsync<Model.Models.Home>();
      _roomRepository = _unitOfWorkAsync.RepositoryAsync<Room>();
      _userRepository = _unitOfWorkAsync.RepositoryAsync<UserInfo>();
      _deviceRepository = _unitOfWorkAsync.RepositoryAsync<SmartDevice>();
      _userRoomRepository = _unitOfWorkAsync.RepositoryAsync<UserRoomLink>();
      _deviceStatusRepository = _unitOfWorkAsync.RepositoryAsync<DeviceStatus>();
      _channelRepository = _unitOfWorkAsync.RepositoryAsync<Channel>();
      _channelStatusRepository = _unitOfWorkAsync.RepositoryAsync<ChannelStatus>();
      _userRoomLinkRepository = _unitOfWorkAsync.RepositoryAsync<UserRoomLink>();
      _nextAssociatedDeviceRepository = _unitOfWorkAsync.RepositoryAsync<NextAssociatedDevice>();
      _rgbwStatusRepository = _unitOfWorkAsync.RepositoryAsync<RgbwStatus>();
      _userHomeRepository = _unitOfWorkAsync.RepositoryAsync<UserHomeLink>();
      _versionRepository = _unitOfWorkAsync.RepositoryAsync<SmartHome.Model.Models.Version>();
      _userHomeRepository = _unitOfWorkAsync.RepositoryAsync<UserHomeLink>();
      _versionDetailRepository = _unitOfWorkAsync.RepositoryAsync<SmartHome.Model.Models.VersionDetail>();
      _mqttMessageLogRepository = _unitOfWorkAsync.RepositoryAsync<MessageLog>();
      _homeJsonEntity = homeJsonEntity;
      _homeJsonMessage = homeJsonMessage;
      _receivedFrom = receivedFrom;
      _messageLog = new MessageLog();
      SetMapper();
    }
   

    public Home SaveJsonData()
    {
      Home home = null;
      //MessageLog messageLog = new CommonService(_unitOfWorkAsync).SaveMessageLog(_homeJsonMessage, _receivedFrom);

      //_unitOfWorkAsync.BeginTransaction();
      
      try
      {
        home = SaveHomeAndRouter();
        //var changes = _unitOfWorkAsync.SaveChanges();
        //_unitOfWorkAsync.Commit();
      }
      catch (Exception ex)
      {
        //_unitOfWorkAsync.Rollback();
        return null;
      }

      //new CommonService(_unitOfWorkAsync).UpdateMessageLog(messageLog, _homeJsonEntity.Home[0].PassPhrase);

      return home;
    }

    public RouterInfoEntity GetRouter(string macAddress)
    {
      RouterInfo router = _routerInfoRepository
          .Queryable().Include(x => x.Parent).Where(u => u.MacAddress == macAddress).FirstOrDefault();
      MapRouterInfo();
      return Mapper.Map<RouterInfo, RouterInfoEntity>(router);
    }

    private void MapRouterInfo()
    {
      //map parent
      Mapper.CreateMap<Home, HomeEntity>()
      .ForMember(dest => dest.IsInternet, opt => opt.MapFrom(a => a.IsInternet == true ? 1 : 0))
      .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(a => a.IsDefault == true ? 1 : 0))
      .ForMember(dest => dest.IsActive, opt => opt.MapFrom(a => a.IsActive == true ? 1 : 0))
      .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(a => a.IsSynced == true ? 1 : 0));
      //map router
      Mapper.CreateMap<RouterInfo, RouterInfoEntity>()
           .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(a => a.IsSynced == true ? 1 : 0))
           .ForMember(dest => dest.Parent, opt => opt.MapFrom(a => a.Parent));
    }


    public HomeEntity GetHome(int homeId)
    {
      Home home = _homeRepository
          .Queryable().Include(x => x.Rooms).Where(u => u.HomeId == homeId).FirstOrDefault();

      Mapper.CreateMap<HomeEntity, Home>();
      return Mapper.Map<Home, HomeEntity>(home);
    }

    public void InsertDevice(SmartDeviceEntity device, Room room)
    {
      SmartDevice model = Mapper.Map<SmartDeviceEntity, SmartDevice>(device);
      model.IsDeleted = Convert.ToBoolean(device.IsDeleted);
      model.IsSynced = Convert.ToBoolean(device.IsSynced);

      List<DeviceStatusEntity> deviceStatuses =
              _homeJsonEntity.DeviceStatus.FindAll(x => x.AppsDeviceId == device.AppsDeviceId.ToString());

      if (device.DeviceType == DeviceType.SmartSwitch6g)
      {
        InsertSmartDevices(device, room, model, deviceStatuses);
      }
      if (device.DeviceType == DeviceType.SmartRainbow12)
      {
        InsertSmartRainbow(device, model, room);
      }
      if (device.DeviceType == DeviceType.SmartRouter)
      {
        InsertSmartRouter(device, model, room, deviceStatuses);
      }
    }
    private void InsertSmartRouter(SmartDeviceEntity device, SmartDevice model, Room room, List<DeviceStatusEntity> deviceStatuses)
    {
      SmartRouter router = MapToSmartRouter(model);
      router.Room = room;
      router.ObjectState = ObjectState.Added;
      router.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
      _deviceRepository.Insert(router);

      InsertRouterDeviceStatus(router, deviceStatuses);
    }
    private void InsertRouterDeviceStatus(SmartRouter sswitch, List<DeviceStatusEntity> deviceStatuses)
    {
      foreach (var deviceStatusEntity in deviceStatuses)
      {
        var entity = Mapper.Map<DeviceStatusEntity, DeviceStatus>(deviceStatusEntity);
        entity.IsSynced = Convert.ToBoolean(deviceStatusEntity.IsSynced);
        entity.ObjectState = ObjectState.Added;
        entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
        _deviceStatusRepository.Insert(entity);
        sswitch.DeviceStatus.Add(entity);
      }
    }
    private void InsertSmartDevices(SmartDeviceEntity device, Room room, SmartDevice model, List<DeviceStatusEntity> deviceStatuses)
    {
      SmartSwitch sswitch = MapToSmartSwitch(model);
      sswitch.Room = room;
      sswitch.ObjectState = ObjectState.Added;
      sswitch.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
      _deviceRepository.Insert(sswitch);

      InsertDeviceStatus(sswitch, deviceStatuses);

      List<ChannelEntity> channelList = _homeJsonEntity.Channel.FindAll(x => x.AppsDeviceTableId == device.AppsDeviceId);
      InsertChannel(sswitch, channelList);
    }
    private void InsertDeviceStatus(SmartSwitch sswitch, List<DeviceStatusEntity> deviceStatuses)
    {
      foreach (var deviceStatusEntity in deviceStatuses)
      {
        var entity = Mapper.Map<DeviceStatusEntity, DeviceStatus>(deviceStatusEntity);
        entity.IsSynced = Convert.ToBoolean(deviceStatusEntity.IsSynced);
        entity.ObjectState = ObjectState.Added;
        entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
        _deviceStatusRepository.Insert(entity);
        sswitch.DeviceStatus.Add(entity);
      }
    }
    private void InsertSmartRainbow(SmartDeviceEntity device, SmartDevice model, Room room)
    {
      SmartRainbow rainbow = MapToSmartRainbow(model);
      rainbow.Room = room;
      rainbow.ObjectState = ObjectState.Added;
      rainbow.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
      _deviceRepository.Insert(rainbow);
      List<RgbwStatusEntity> rgbtStatusList = _homeJsonEntity.RgbwStatus.FindAll(x => x.AppsDeviceId == device.AppsDeviceId);
      InsertRgbwStatus(rainbow, rgbtStatusList);
    }
    private void InsertRgbwStatus(SmartRainbow sswitch, List<RgbwStatusEntity> rgbtStatusList)
    {
      foreach (var rgbtStatus in rgbtStatusList)
      {
        var entity = Mapper.Map<RgbwStatusEntity, RgbwStatus>(rgbtStatus);
        entity.SmartRainbow = sswitch;
        entity.ObjectState = ObjectState.Added;
        entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
        _rgbwStatusRepository.Insert(entity);
      }
    }

    private SmartSwitch MapToSmartSwitch(SmartDevice model)
    {
      SmartSwitch entity = new SmartSwitch();
      entity.DeviceId = model.DeviceId;
      entity.AppsRoomId = model.AppsRoomId;
      entity.AppsDeviceId = model.AppsDeviceId;
      entity.AppsBleId = model.AppsBleId;
      entity.DeviceName = model.DeviceName;
      entity.DeviceHash = model.DeviceHash;
      entity.FirmwareVersion = model.FirmwareVersion;
      entity.IsDeleted = model.IsDeleted;
      entity.Watt = model.Watt;
      entity.DeviceType = model.DeviceType;
      entity.DeviceStatus = new List<DeviceStatus>();
      return entity;
    }
    private SmartRouter MapToSmartRouter(SmartDevice model)
    {
      SmartRouter entity = new SmartRouter();
      entity.DeviceId = model.DeviceId;
      entity.AppsRoomId = model.AppsRoomId;
      entity.AppsDeviceId = model.AppsDeviceId;
      entity.AppsBleId = model.AppsBleId;
      entity.DeviceName = model.DeviceName;
      entity.DeviceHash = model.DeviceHash;
      entity.FirmwareVersion = model.FirmwareVersion;
      entity.IsDeleted = model.IsDeleted;
      entity.Watt = model.Watt;
      entity.DeviceType = model.DeviceType;
      entity.DeviceStatus = new List<DeviceStatus>();
      return entity;
    }
    private SmartRainbow MapToSmartRainbow(SmartDevice model)
    {
      SmartRainbow entity = new SmartRainbow();
      entity.DeviceId = model.DeviceId;
      entity.AppsRoomId = model.AppsRoomId;
      entity.AppsDeviceId = model.AppsDeviceId;
      entity.AppsBleId = model.AppsBleId;
      entity.DeviceName = model.DeviceName;
      entity.DeviceHash = model.DeviceHash;
      entity.FirmwareVersion = model.FirmwareVersion;
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
        entity.IsSynced = Convert.ToBoolean(channel.IsSynced);
        entity.ObjectState = ObjectState.Added;
        entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
        _channelRepository.Insert(entity);
        entity.ChannelStatuses = new List<ChannelStatus>();
        List<ChannelStatusEntity> channelStatuses =
            _homeJsonEntity.ChannelStatus.FindAll(x => x.AppsChannelId == channel.AppsChannelId);

        foreach (var channelStatusEntity in channelStatuses)
        {
          var channelStatusModel = Mapper.Map<ChannelStatusEntity, ChannelStatus>(channelStatusEntity);
          channelStatusModel.IsSynced = Convert.ToBoolean(channelStatusEntity.IsSynced);
          channelStatusModel.ObjectState = ObjectState.Added;
          channelStatusModel.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
          _channelStatusRepository.Insert(channelStatusModel);
          entity.ChannelStatuses.Add(channelStatusModel);
        }

        sswitch.Channels.Add(entity);
      }

    }
    

    private Home SaveHomeAndRouter()
    {
      string passPhrase = _homeJsonEntity.Home[0].PassPhrase;
      Home home = null;
      HomeEntity homeEntity = GetHomeByPassPhrase(passPhrase);
      if (homeEntity != null)
      {
        _homeJsonEntity.Home[0].HomeId = homeEntity.HomeId;
        home = UpdateHome(homeEntity);
        RouterInfo router = GetRouterByHomeId(home);
        InsertOrUpdateRouter(router, home);
      }
      else
      {
        _homeJsonEntity.Home[0].HomeId = 0;
        home = InsertHome(_homeJsonEntity.Home[0]);
        if (_homeJsonEntity.RouterInfo.Count > 0)
        {
          InsertRouter(_homeJsonEntity.RouterInfo[0], home);
        }
      }

      IList<UserInfo> listOfUsers = SaveOrUpdateUser();
      IList<UserInfo> listOfExistingDbUsers = DeleteUser(home, listOfUsers);
      home = SaveOrUpdateRoom(home, listOfUsers, listOfExistingDbUsers);
      SaveHomeUser(home, listOfUsers, listOfExistingDbUsers);
      SaveOrUpdateDevice(home);
      SaveOrUpdateNextAssociatedDevice(home);
      SaveOrUpdateVersion(home);
      return home;
    }
    private HomeEntity GetHomeByPassPhrase(string passPhrase)
    {
      Home home = _homeRepository
          .Queryable().Include(x => x.Rooms).Where(u => u.PassPhrase == passPhrase).FirstOrDefault();

      Mapper.CreateMap<Home, HomeEntity>()
      .ForMember(dest => dest.IsInternet, opt => opt.MapFrom(a => a.IsInternet == true ? 1 : 0))
      .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(a => a.IsDefault == true ? 1 : 0))
      .ForMember(dest => dest.IsActive, opt => opt.MapFrom(a => a.IsActive == true ? 1 : 0))
      .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(a => a.IsSynced == true ? 1 : 0));


      return Mapper.Map<Home, HomeEntity>(home);
    }
    private Home UpdateHome(HomeEntity homeEntity)
    {
      Home model = MapHomeProperty(homeEntity);
      model.ObjectState = ObjectState.Modified;
      _homeRepository.Update(model);
      return model;

    }
    private Home InsertHome(HomeEntity homeEntity)
    {
      Home model = Mapper.Map<HomeEntity, Home>(homeEntity);
      model.IsInternet = Convert.ToBoolean(homeEntity.IsInternet);
      model.IsDefault = Convert.ToBoolean(homeEntity.IsDefault);
      model.IsActive = Convert.ToBoolean(homeEntity.IsActive);
      model.IsSynced = Convert.ToBoolean(homeEntity.IsSynced);
      model.ObjectState = ObjectState.Added;
      model.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
      _homeRepository.Insert(model);
      return model;
    }
    private RouterInfo GetRouterByHomeId(Home home)
    {
      RouterInfo router = _routerInfoRepository
          .Queryable().Include(x => x.Parent).Where(u => u.Parent.HomeId == home.HomeId).FirstOrDefault();
      MapRouterInfo();
      return router;
    }
    private void InsertOrUpdateRouter(RouterInfo router, Home home)
    {
      if (_homeJsonEntity.RouterInfo.Count > 0)
      {
        if (router == null)
        {
          InsertRouter(_homeJsonEntity.RouterInfo[0], home);
        }
        else
        {
          UpdateRouter(_homeJsonEntity.RouterInfo[0], router);
        }
      }

      if (_homeJsonEntity.RouterInfo.Count == 0 && router != null)
      {
        DeleteRouter(router);
      }

    }

    private void DeleteRouter(RouterInfo router)
    {
      router.ObjectState = ObjectState.Deleted;
      _routerInfoRepository.Delete(router);
    }

    private void InsertRouter(RouterInfoEntity router, Home home)
    {
      var entity = Mapper.Map<RouterInfoEntity, RouterInfo>(router);
      entity.IsSynced = Convert.ToBoolean(router.IsSynced);
      entity.Parent = home;
      entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
      entity.ObjectState = ObjectState.Added;
      _routerInfoRepository.Insert(entity);
    }
    private void UpdateRouter(RouterInfoEntity router, RouterInfo routerFromDb)
    {
      var entity = MapRouterProperty(router, routerFromDb);
      entity.ObjectState = ObjectState.Modified;
      _routerInfoRepository.Update(entity);
    }
    private IList<UserInfo> SaveOrUpdateUser()
    {
      IList<UserInfo> listOfUsers = new List<UserInfo>();
      foreach (var userInfoEntity in _homeJsonEntity.UserInfo)
      {
        var dbUserEntity = GetUser(userInfoEntity.Email);
        if (dbUserEntity == null)
        {
          listOfUsers.Add(InsertUser(userInfoEntity));
        }
        else
        {
          listOfUsers.Add(UpdateUser(userInfoEntity, dbUserEntity));
        }
      }
      return listOfUsers;
    }
    public UserInfo GetUser(string email)
    {
      UserInfo user = _userRepository
          .Queryable().Include(x => x.UserRoomLinks).Include(x => x.UserHomeLinks).Where(u => u.Email == email).FirstOrDefault();

      return user;
    }
    public string GetUserInfosByHomePassphase(string passPhrase)
    {
      string userInfos = string.Empty;

      Home home = _homeRepository.Queryable().Where(p => p.PassPhrase == passPhrase).FirstOrDefault();
      if (home != null)
      {
        var temp = _userHomeRepository.Queryable().Where(p => p.Home.HomeId == home.HomeId).Select(q => q.UserInfo.UserInfoId);

        userInfos = string.Join(",", temp);
      }

      return userInfos;
    }

    private UserInfo InsertUser(UserInfoEntity userInfoEntity)
    {
      UserInfo entity = Mapper.Map<UserInfoEntity, UserInfo>(userInfoEntity);
      entity.LoginStatus = Convert.ToBoolean(userInfoEntity.LoginStatus);
      entity.RegStatus = Convert.ToBoolean(userInfoEntity.RegStatus);
      entity.IsSynced = Convert.ToBoolean(userInfoEntity.IsSynced);
      entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
      //entity.MessageLog = _messageLog;
      entity.ObjectState = ObjectState.Added;
      _userRepository.Insert(entity);
      return entity;
    }
    private UserInfo UpdateUser(UserInfoEntity userInfoEntity, UserInfo dbUserEntity)
    {
      var entity = MapUserProperty(userInfoEntity, dbUserEntity);
      // entity.MessageLog = _messageLog;
      entity.ObjectState = ObjectState.Modified;
      _userRepository.Update(entity);
      DeleteRoomUser(entity);
      DeleteHomeUser(entity);
      return entity;
    }
    private void DeleteHomeUser(UserInfo entity)
    {
      IList<long> userHomeLinkIds = entity.UserHomeLinks.Select(s => s.UserHomeLinkId).ToList();
      foreach (long id in userHomeLinkIds)
      {
        UserHomeLink userHomeLink = _userHomeRepository.Find(id);
        userHomeLink.ObjectState = ObjectState.Deleted;
        _userHomeRepository.Delete(userHomeLink);
      }
    }
    private void DeleteRoomUser(UserInfo entity)
    {
      IList<long> userHomeLinkIds = entity.UserRoomLinks.Select(s => s.UserRoomLinkId).ToList();
      foreach (long userRoomLinkid in userHomeLinkIds)
      {
        UserRoomLink userRoomLink = _userRoomRepository.Find(userRoomLinkid);
        userRoomLink.ObjectState = ObjectState.Deleted;
        _userRoomRepository.Delete(userRoomLink);
      }
    }
    private IList<UserInfo> DeleteUser(Home home, IList<UserInfo> listOfUsersFromJson)
    {

      IList<UserInfo> listOfExistingDbUser = new List<UserInfo>();

      IList<long> userIdsFromJson = (from u in listOfUsersFromJson
                                     select u.UserInfoId).ToList();

      IList<long> needToDeleteUserIds =
          _userHomeRepository.Queryable()
              .Where(w => w.Home.HomeId == home.HomeId && !userIdsFromJson.Contains(w.UserInfo.UserInfoId))
              .Select(s => s.UserInfo.UserInfoId)
              .ToList();

      foreach (long id in needToDeleteUserIds)
      {
        UserInfo user =
            _userRepository.Queryable()
                .Include(x => x.UserRoomLinks)
                .Include(x => x.UserHomeLinks)
                .Where(u => u.UserInfoId == id)
                .FirstOrDefault();

        if (user != null)
        {
          DeleteRoomUser(user);
          DeleteHomeUser(user);
          if (_receivedFrom == MessageReceivedFrom.MQTT)
          {
            user.ObjectState = ObjectState.Deleted;
            _userRepository.Delete(user);
          }
          else
          {
            listOfExistingDbUser.Add(user);
          }
        }
      }
      return listOfExistingDbUser;

    }
    private Home SaveOrUpdateRoom(Home model, IList<UserInfo> listOfUsers, IList<UserInfo> listOfExistingDbUsers)
    {
      if (model.Rooms != null)
      {
        IList<long> roomIds = model.Rooms.Select(s => s.RoomId).ToList();
        model.Rooms = new List<Room>();
        DeleteAllRooms(roomIds);
      }
      return InsertAllRooms(model, listOfUsers, listOfExistingDbUsers);
    }
    private void DeleteAllRooms(IList<long> roomIds)
    {
      foreach (var roomId in roomIds)
      {
        Room dbRoom = _roomRepository
        .Queryable().Where(u => u.RoomId == roomId).FirstOrDefault();
        dbRoom.ObjectState = ObjectState.Deleted;
        _roomRepository.Delete(dbRoom);
      }
    }
    private Home InsertAllRooms(Home home, IList<UserInfo> listOfUsers, IList<UserInfo> listOfExistingDbUsers)
    {
      home.Rooms = new List<Room>();
      foreach (var room in _homeJsonEntity.Room)
      {
        var entity = Mapper.Map<RoomEntity, Room>(room);
        entity.IsActive = Convert.ToBoolean(room.IsActive);
        entity.IsSynced = Convert.ToBoolean(room.IsSynced);
        entity.Home = home;
        entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
        entity.ObjectState = ObjectState.Added;
        _roomRepository.Insert(entity);

        SaveRoomUser(entity, listOfUsers);

        if (listOfExistingDbUsers.Count > 0)
        {
          SaveRoomForExistingDbUser(entity, listOfExistingDbUsers);
        }

      }

      return home;
    }

    private void SaveRoomForExistingDbUser(Room entity, IList<UserInfo> listOfExistingDbUsers)
    {
      var roomLinkList = _homeJsonEntity.UserRoomLink.FindAll(x => x.AppsRoomId == entity.AppsRoomId);
      foreach (var userRoomLinkEntity in roomLinkList)
      {
        foreach (var dbUser in listOfExistingDbUsers)
        {
          UserRoomLink userRoom = new UserRoomLink();
          userRoom.UserInfo = dbUser;

          FillSaveRoomUser(entity, userRoomLinkEntity, userRoom);
        }

      }
    }

    private void SaveRoomUser(Room entity, IList<UserInfo> listOfUsers)
    {
      var roomLinkList = _homeJsonEntity.UserRoomLink.FindAll(x => x.AppsRoomId == entity.AppsRoomId);
      foreach (var userRoomLinkEntity in roomLinkList)
      {
        UserInfoEntity userentity =
            _homeJsonEntity.UserInfo.Find(x => x.AppsUserId.ToString() == userRoomLinkEntity.AppsUserId.ToString());
        UserRoomLink userRoom = new UserRoomLink();
        userRoom.UserInfo = listOfUsers.Where(u => u.Email == userentity.Email).FirstOrDefault();

        FillSaveRoomUser(entity, userRoomLinkEntity, userRoom);
      }
    }

    private void FillSaveRoomUser(Room entity, UserRoomLinkEntity userRoomLinkEntity, UserRoomLink userRoom)
    {
      userRoom.Room = entity;
      userRoom.IsSynced = Convert.ToBoolean(userRoomLinkEntity.IsSynced);
      userRoom.AppsUserRoomLinkId = userRoomLinkEntity.AppsUserRoomLinkId;
      userRoom.AppsRoomId = userRoomLinkEntity.AppsRoomId;
      userRoom.AppsUserId = userRoomLinkEntity.AppsUserId;
      userRoom.ObjectState = ObjectState.Added;
      _userRoomLinkRepository.Insert(userRoom);
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

        FillSaveHomeUser(home, userRoomLinkEntity, userHome);
      }
      if (listOfExistingDbUsers.Count > 0)
      {
        SaveHomeForExistingDbUsers(home, listOfExistingDbUsers, homeUserList);
      }

    }

    private void FillSaveHomeUser(Home home, UserHomeLinkEntity userRoomLinkEntity, UserHomeLink userHome)
    {
      userHome.Home = home;
      userHome.IsSynced = Convert.ToBoolean(userRoomLinkEntity.IsSynced);
      userHome.AppsUserHomeLinkId = userRoomLinkEntity.AppsUserHomeLinkId;
      userHome.AppsHomeId = userRoomLinkEntity.AppsHomeId;
      userHome.AppsUserId = userRoomLinkEntity.AppsUserId;
      userHome.IsAdmin = Convert.ToBoolean(userRoomLinkEntity.IsAdmin);
      userHome.ObjectState = ObjectState.Added;
      _userHomeRepository.Insert(userHome);
    }

    private void SaveHomeForExistingDbUsers(Home home, IList<UserInfo> listOfExistingDbUsers, List<UserHomeLinkEntity> homeUserList)
    {
      foreach (var userRoomLinkEntity in homeUserList)
      {
        foreach (var dbUser in listOfExistingDbUsers)
        {
          UserHomeLink userHome = new UserHomeLink();
          userHome.UserInfo = dbUser;

          FillSaveHomeUser(home, userRoomLinkEntity, userHome);
        }
      }
    }

    private void DeleteHomeRouter(RouterInfoEntity router, Home home)
    {
      RouterInfo dbRouter = _routerInfoRepository.Queryable().Where(p => p.Parent.HomeId == home.HomeId).FirstOrDefault();
      if (dbRouter != null)
      {
        _routerInfoRepository.Delete(dbRouter);
      }

    }
    private void SaveOrUpdateVersion(Home home)
    {
      DeleteVersion(home);
      List<VersionEntity> versionEntityList = _homeJsonEntity.Version;

      foreach (var versionEntity in versionEntityList)
      {
        List<VersionDetailEntity> versionDetails =
            _homeJsonEntity.VersionDetails.FindAll(x => x.AppsVersionId == versionEntity.AppsVersionId);
        InsertVersion(home, versionEntity, versionDetails);
      }
    }
    private void InsertVersion(Home home, VersionEntity versionEntity, List<VersionDetailEntity> versionDetails)
    {
      Version version = Mapper.Map<VersionEntity, Version>(versionEntity);
      version.Home = home;
      version.IsSynced = Convert.ToBoolean(versionEntity.IsJsonSynced);
      version.ObjectState = ObjectState.Added;
      version.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
      _versionRepository.Insert(version);

      InsertVersionDetails(version, versionDetails);
    }

    private void UpdateVersion(Home home, VersionEntity versionEntity)
    {
      Version model = MapVersionProperty(home, versionEntity);
      model.ObjectState = ObjectState.Modified;
      _versionRepository.Update(model);
    }
    private Version MapVersionProperty(Home home, VersionEntity versionEntity)
    {
      Version model = _versionRepository
         .Queryable().Where(u => u.Home.HomeId == home.HomeId).FirstOrDefault();

      model.AuditField.LastUpdatedBy = "admin";
      model.AuditField.LastUpdatedDateTime = DateTime.Now;

      return model;
    }
    private void InsertVersionDetails(Version version, List<VersionDetailEntity> versionDetailsEntity)
    {
      foreach (var verDetail in versionDetailsEntity)
      {
        VersionDetail versionDetail = Mapper.Map<VersionDetailEntity, VersionDetail>(verDetail);
        versionDetail.IsSynced = Convert.ToBoolean(verDetail.IsJsonSynced);
        versionDetail.ObjectState = ObjectState.Added;
        versionDetail.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
        _versionDetailRepository.Insert(versionDetail);
        version.VersionDetails.Add(versionDetail);
      }
    }
    private void DeleteVersion(Home home)
    {
      Version version = _versionRepository.Queryable().Where(w => w.Home.HomeId == home.HomeId).FirstOrDefault();
      if (version != null)
      {
        version.ObjectState = ObjectState.Deleted;
        _versionRepository.Delete(version);
      }
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
          InsertDevice(smartDevice, room);
        }
      }
    }

    #region Mapping
    private void SetMapper()
    {
      Mapper.CreateMap<HomeEntity, Home>();
      Mapper.CreateMap<RouterInfoEntity, RouterInfo>();
      Mapper.CreateMap<RoomEntity, Room>();
      Mapper.CreateMap<UserInfoEntity, UserInfo>();
      Mapper.CreateMap<SmartDeviceEntity, SmartDevice>();
      Mapper.CreateMap<ChannelStatusEntity, ChannelStatus>();
      Mapper.CreateMap<DeviceStatusEntity, DeviceStatus>();
      Mapper.CreateMap<ChannelEntity, Channel>();
      Mapper.CreateMap<RgbwStatusEntity, RgbwStatus>();
      Mapper.CreateMap<VersionEntity, Version>();
      Mapper.CreateMap<VersionDetailEntity, VersionDetail>();
    }

    private Home MapHomeProperty(HomeEntity homeEntity)
    {
      Home model = _homeRepository
         .Queryable().Where(u => u.HomeId == homeEntity.HomeId).FirstOrDefault();

      model.Address1 = homeEntity.Address1;
      model.Address2 = homeEntity.Address2;
      model.AppsHomeId = homeEntity.AppsHomeId;
      model.AuditField.LastUpdatedBy = "admin";
      model.AuditField.LastUpdatedDateTime = DateTime.Now;
      model.Block = homeEntity.Block;
      model.HomeId = homeEntity.HomeId;
      model.City = homeEntity.City;
      model.Country = homeEntity.Country;
      model.IsActive = Convert.ToBoolean(homeEntity.IsActive);
      model.IsDefault = Convert.ToBoolean(homeEntity.IsDefault);
      model.IsInternet = Convert.ToBoolean(homeEntity.IsInternet);
      model.IsSynced = Convert.ToBoolean(homeEntity.IsSynced);
      model.MeshMode = (MeshModeType)homeEntity.MeshMode;
      model.PassPhrase = homeEntity.PassPhrase;
      model.Phone = homeEntity.Phone;
      model.TimeZone = homeEntity.TimeZone;
      model.ZipCode = homeEntity.ZipCode;

      return model;
    }

    private RouterInfo MapRouterProperty(RouterInfoEntity router, RouterInfo routerFromDb)
    {
      routerFromDb.MacAddress = router.MacAddress;
      routerFromDb.Ssid = router.Ssid;
      routerFromDb.AppsRouterInfoId = router.AppsRouterInfoId;
      routerFromDb.AuditField.LastUpdatedBy = "admin";
      routerFromDb.AuditField.LastUpdatedDateTime = DateTime.Now;
      routerFromDb.IsSynced = Convert.ToBoolean(router.IsSynced);
      routerFromDb.LocalBrokerIp = router.LocalBrokerIp;
      routerFromDb.LocalBrokerPassword = router.LocalBrokerPassword;
      routerFromDb.LocalBrokerPort = router.LocalBrokerPort;
      routerFromDb.LocalBrokerUsername = router.LocalBrokerUsername;

      return routerFromDb;
    }

    private UserInfo MapUserProperty(UserInfoEntity userEntity, UserInfo model)
    {

      model.AppsUserId = userEntity.AppsUserId;
      model.LoginStatus = Convert.ToBoolean(userEntity.LoginStatus);
      model.Password = userEntity.Password;
      model.RegStatus = Convert.ToBoolean(userEntity.RegStatus);
      model.UserName = userEntity.UserName;
      model.IsSynced = Convert.ToBoolean(userEntity.IsSynced);
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
