using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{
  public class CommonService
  {
    private readonly IUnitOfWorkAsync _unitOfWorkAsync;
    private readonly IRepositoryAsync<Home> _homeRepository;
    private readonly IRepositoryAsync<Room> _roomRepository;
    private readonly IRepositoryAsync<UserInfo> _userRepository;
    private readonly IRepositoryAsync<UserRoomLink> _userRoomLinkRepository;
    private readonly IRepositoryAsync<UserHomeLink> _userHomeRepository;
    private readonly IRepositoryAsync<RouterInfo> _routerRepository;
    private readonly IRepositoryAsync<MessageLog> _mqttMessageLogRepository;

    public CommonService(IUnitOfWorkAsync unitOfWorkAsync)
    {
      _unitOfWorkAsync = unitOfWorkAsync;
      _homeRepository = _unitOfWorkAsync.RepositoryAsync<Model.Models.Home>();
      _roomRepository = _unitOfWorkAsync.RepositoryAsync<Room>();
      _userRepository = _unitOfWorkAsync.RepositoryAsync<UserInfo>();
      _userRoomLinkRepository = _unitOfWorkAsync.RepositoryAsync<UserRoomLink>();
      _userHomeRepository = _unitOfWorkAsync.RepositoryAsync<UserHomeLink>();
      _routerRepository = _unitOfWorkAsync.RepositoryAsync<RouterInfo>();

      _mqttMessageLogRepository = _unitOfWorkAsync.RepositoryAsync<MessageLog>();
    }

    public bool IsLoginIdUnique(string email)
    {
      return _userRepository.Queryable().Any(p => p.Email == email);
    }

    public Home GetHome(string passPhrase)
    {
      return _homeRepository.Queryable().Where(p => p.PassPhrase == passPhrase).FirstOrDefault();
    }

    public Room GetRoomByPassPhaseAndAppsRoomId(string passPhrase, int appsRoomId)
    {
      return _homeRepository.Queryable().Where(p => p.PassPhrase == passPhrase).SelectMany(x => x.Rooms.Where(q => q.AppsRoomId == appsRoomId)).FirstOrDefault();
    }

    public Home GetHomeWithRooms(string passPhrase)
    {
      return _homeRepository.Queryable().Include(x => x.Rooms).Where(p => p.PassPhrase == passPhrase).FirstOrDefault();
    }

    public UserInfo GetUser(string email)
    {
      UserInfo user = _userRepository
          .Queryable().Include(x => x.UserRoomLinks).Include(x => x.UserHomeLinks).Where(u => u.Email == email).FirstOrDefault();

      return user;
    }

    public UserInfo GetUserByEmail(string email)
    {
      UserInfo user = _userRepository
          .Queryable().Where(u => u.Email == email).FirstOrDefault();

      return user;
    }


    public MessageLog SaveMessageLog(string homeJsonMessage, MessageReceivedFrom receivedFrom)
    {
      MessageLog messageLog = new MessageLog();

      try
      {
        DateTime processTime = DateTime.Now;
        var entity = new MessageLog();
        entity.Message = homeJsonMessage;
        entity.ReceivedFrom = receivedFrom;
        entity.UserInfoIds = string.Empty;
        entity.AuditField = new AuditFields("admin", processTime, "admin", processTime);
        entity.ObjectState = ObjectState.Added;
        _mqttMessageLogRepository.Insert(entity);


        messageLog = entity;

      }

      catch (Exception ex)
      {
      }

      return messageLog;
    }


    public MessageLog UpdateMessageLog(MessageLog entity, string passPhrase)
    {

      MessageLog messageLog = new MessageLog();

      try
      {
        DateTime processTime = DateTime.Now;
        //var entity = _messageLog;
        //entity.Message = _homeJsonMessage;
        //entity.ReceivedFrom = _receivedFrom;
        entity.UserInfoIds = GetUserInfosByHomePassphase(passPhrase);
        entity.AuditField = new AuditFields("admin", entity.AuditField.InsertedDateTime, "admin", processTime);
        entity.ObjectState = ObjectState.Modified;
        _mqttMessageLogRepository.Update(entity);

        messageLog = entity;

      }
      catch (Exception ex)
      {
      }
      return messageLog;
    }

    public string GetUserInfosByHomePassphase(string passPhrase)
    {
      string userInfos = string.Empty;

      Home home = _homeRepository.Queryable().Where(p => p.PassPhrase == passPhrase).FirstOrDefault();
      if (home != null)
      {
        var temp = _userHomeRepository.Queryable().Where(p => p.Home.HomeId == home.HomeId).DefaultIfEmpty().Select(q => q.UserInfo.UserInfoId);
        if (temp != null)
        {
          userInfos = string.Join(",", temp);
        }
      }
      return userInfos;
    }

    public bool PasswordUpdate(string email, string password)
    {
      try
      {
        var entity = MapUserInfoProperty(email, password);
        entity.ObjectState = ObjectState.Modified;
        _userRepository.Update(entity);
      }
      catch (Exception ex)
      {
        return false;
      }
      return true;
    }

    private UserInfo MapUserInfoProperty(string email, string password)
    {
      UserInfo model = _userRepository.Queryable().Where(p => p.Email == email).FirstOrDefault();
      model.AuditField.LastUpdatedBy = "admin";
      model.AuditField.LastUpdatedDateTime = DateTime.Now;
      model.Password = password;
      return model;
    }

    public SmartDevice GetSmartDeviceByDeviceHashAndPassPhrase(string deviceHash, string passPhrase)
    {
      return _homeRepository.Queryable().Where(p => p.PassPhrase == passPhrase)
        .SelectMany(p => p.Rooms)
        .SelectMany(q => q.SmartDevices.Where(s => s.DeviceHash == deviceHash))
        .FirstOrDefault();
    }

    public T GetSmartSwitchByDeviceHashAndPassPhrase<T>(string deviceHash, string passPhrase) where T : SmartDevice
    {
      return _homeRepository.Queryable().Where(p => p.PassPhrase == passPhrase)
             .SelectMany(p => p.Rooms)
             .SelectMany(q => q.SmartDevices.OfType<T>().Where(s => s.DeviceHash == deviceHash))
             .FirstOrDefault();
    }


    public RouterInfo GetRouterInfoByPassPhraseAndAppsBleId(string passPhrase, int appsBleId)
    {
      return _homeRepository.Queryable().Where(p => p.PassPhrase == passPhrase)
        .SelectMany(x => x.SmartRouterInfos.Where(q => q.AppsBleId == appsBleId)).FirstOrDefault();
    }

    public RouterInfo GetRouterInfoByMacAddressAndHomeId(string macAddress, long HomeId)
    {
      return _routerRepository.Queryable().Where(p => p.MacAddress == macAddress && p.Parent.HomeId == HomeId).FirstOrDefault();
    }


    public Home GetHomeByPassPhrase(string passPhrase)
    {
      Home home = _homeRepository
          .Queryable().Include(x => x.Rooms).Where(u => u.PassPhrase == passPhrase).FirstOrDefault();
      return (home);
    }

    public RouterInfo GetRouterByHomeId(Home home)
    {
      RouterInfo router = _routerRepository
          .Queryable().Include(x => x.Parent).Where(u => u.Parent.HomeId == home.HomeId).FirstOrDefault();
      MapRouterInfo();
      return router;
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
  }
}
