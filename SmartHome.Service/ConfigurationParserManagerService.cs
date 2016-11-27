using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using Service.Pattern;
using SmartHome.Entity;
using SmartHome.Model.Models;
using SmartHome.Service.Interfaces;
//using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using SmartHome.Model.ViewModels;

namespace SmartHome.Service
{
  public class ConfigurationParserManagerService : IConfigurationParserManagerService
  {
    #region PrivateProperty

    private readonly IRepositoryAsync<UserInfo> _userInfoRepository;
    private readonly IRepositoryAsync<UserHomeLink> _userHomeLinkRepository;
    private readonly IRepositoryAsync<UserRoomLink> _userRoomLinkRepository;
    private readonly IRepositoryAsync<Home> _homeRepository;
    private readonly IRepositoryAsync<Model.Models.Version> _versionRepository;
    private readonly IRepositoryAsync<SmartDevice> _deviceRepository;
    private readonly IRepositoryAsync<NextAssociatedDevice> _associatedDeviceRepository;
    private readonly IRepositoryAsync<RgbwStatus> _rgbStatusRepository;
    private readonly IRepositoryAsync<Room> _roomRepository;
    private readonly IRepositoryAsync<Channel> _channelRepository;
    private readonly IRepositoryAsync<RouterInfo> _smartRouterInfoRepository;
    private readonly IRepositoryAsync<WebPagesRole> _webPagesRoleRepository;
    private readonly IRepositoryAsync<UserRole> _userRoleRepository;

    public IList<UserHomeLink> _userHomeLinks { get; private set; }
    public HomeViewModel _homeViewModel { get; private set; }




    #endregion

    public ConfigurationParserManagerService(IUnitOfWorkAsync unitOfWork)
    {
      _userInfoRepository = unitOfWork.RepositoryAsync<UserInfo>();
      _userHomeLinkRepository = unitOfWork.RepositoryAsync<UserHomeLink>();
      _userRoomLinkRepository = unitOfWork.RepositoryAsync<UserRoomLink>();
      _homeRepository = unitOfWork.RepositoryAsync<Home>();
      _versionRepository = unitOfWork.RepositoryAsync<Model.Models.Version>();
      _deviceRepository = unitOfWork.RepositoryAsync<SmartDevice>();
      _associatedDeviceRepository = unitOfWork.RepositoryAsync<NextAssociatedDevice>();
      _rgbStatusRepository = unitOfWork.RepositoryAsync<RgbwStatus>();
      _roomRepository = unitOfWork.RepositoryAsync<Room>();
      _channelRepository = unitOfWork.RepositoryAsync<Channel>();
      _smartRouterInfoRepository = unitOfWork.RepositoryAsync<RouterInfo>();
      _webPagesRoleRepository = unitOfWork.RepositoryAsync<WebPagesRole>();
      _userRoleRepository = unitOfWork.RepositoryAsync<UserRole>();
    }


    #region MyRegion

    public IEnumerable<UserRoomLink> AddOrUpdateRoomGraphRange(IEnumerable<UserRoomLink> model)
    {
      List<UserRoomLink> userRoomLink = new List<UserRoomLink>();
      userRoomLink = FillRoomInformations(model, userRoomLink);
      _userRoomLinkRepository.InsertOrUpdateGraphRange(userRoomLink);
      return userRoomLink;
    }

    public List<UserRoomLink> FillRoomInformations(IEnumerable<UserRoomLink> model, List<UserRoomLink> roomModel)
    {
      foreach (var item in model)
      {
        //check already exist or not.
        IEnumerable<UserRoomLink> temp = IsRoomAndUserExists(item.AppsRoomId, item.AppsUserId);

        if (temp.Count() == 0)
        {
          //check for room unique

          IEnumerable<Room> tempRoomCheck = IsRoomExists(item.AppsRoomId.ToString());
          if (tempRoomCheck.Count() > 0)
          {
            item.Room = new Room();
            item.Room.ObjectState = ObjectState.Modified;
            item.Room = tempRoomCheck.FirstOrDefault();
          }

          //check for user unique

          IEnumerable<UserInfo> tempUserCheck = IsUserExists(item.UserInfo.Email.ToString(), item.UserInfo.Password.ToString());
          if (tempUserCheck.Count() > 0)
          {
            item.UserInfo = new UserInfo();
            item.UserInfo.ObjectState = ObjectState.Modified;
            item.UserInfo = tempUserCheck.FirstOrDefault();
          }

          //new item
          roomModel.Add(item);
          continue;
        }
        else
        {
          //existing item               
          // versionModel = temp;
          foreach (var existingItem in temp.ToList())
          {

            FillExistingRoomInfo(item.Room, existingItem.Room);
            FillExistingUserInfo(item.UserInfo, existingItem.UserInfo);



          }
        }
      }

      return roomModel;
    }

    #endregion



    #region Home-User-Room AddOrUpdateGraphRange
    public IEnumerable<UserHomeLink> AddOrUpdateHomeGraphRange(IEnumerable<UserHomeLink> model)
    {
      List<UserHomeLink> userHomeModel = new List<UserHomeLink>();
      userHomeModel = FillHomeInformations(model, userHomeModel);
      _userHomeLinkRepository.InsertOrUpdateGraphRange(userHomeModel);
      return userHomeModel;
    }

    public List<UserHomeLink> FillHomeInformations(IEnumerable<UserHomeLink> model, List<UserHomeLink> homeModel)
    {
      foreach (var nextHome in model)
      {
        //check already exist or not.
        IEnumerable<UserHomeLink> temp = IsHomeAndUserExists(nextHome.AppsHomeId, nextHome.AppsUserId);
        //new item
        if (temp.Count() == 0)
        {
          FillNewUserHomeLinksByInfomation(nextHome);
          homeModel.Add(nextHome);
          continue;
        }
        else
        {
          //existing item                                   
          FillUserHomeLinksByExistingInformation(nextHome, temp);
        }
      }


      return homeModel;
    }

    private void FillUserHomeLinksByExistingInformation(UserHomeLink nextHome, IEnumerable<UserHomeLink> temp)
    {
      foreach (var existingItem in temp.ToList())
      {
        FillExistingHomeInfo(nextHome.Home, existingItem.Home);
        FillExistingUserRoles(nextHome, existingItem);
        FillExistingUserInfo(nextHome.UserInfo, existingItem.UserInfo);
        FillExistingUserHomeLinks(nextHome, existingItem);
      }
    }

    private void FillNewUserHomeLinksByInfomation(UserHomeLink nextHome)
    {
      //check for home unique
      IEnumerable<Home> tempHomeCheck = IsHomeExists(nextHome.AppsHomeId.ToString());
      FillNewUserHomeLInksByHomeInformation(nextHome, tempHomeCheck);

      //check for user unique
      IEnumerable<UserInfo> tempUserCheck = IsUserExists(nextHome.UserInfo.Email.ToString(), nextHome.UserInfo.Password.ToString());
      var wRole = GetWebPagesRole(nextHome.IsAdmin == true ? 1 : 2);
      FillNewUserHomeLinksByUserInfomation(nextHome, tempUserCheck, wRole);
    }

    private void FillNewUserHomeLinksByUserInfomation(UserHomeLink nextHome, IEnumerable<UserInfo> tempUserCheck, WebPagesRole wRole)
    {
      if (tempUserCheck.Count() == 0)
      {
        nextHome.UserInfo.UserRoles = new List<UserRole>();
        var userRole = new UserRole
        {
          WebPagesRole = wRole,
          UserInfo = nextHome.UserInfo,
          ObjectState = ObjectState.Added
        };
        nextHome.UserInfo.UserRoles.Add(userRole);
      }
      else
      {
        nextHome.UserInfo = new UserInfo();
        nextHome.UserInfo.ObjectState = ObjectState.Modified;
        nextHome.UserInfo = tempUserCheck.FirstOrDefault();
        var uRole = GetUserRole(nextHome.UserInfo.UserInfoId);

        //new
        if (uRole == null)
        {
          nextHome.UserInfo.UserRoles = new List<UserRole>();
          var userRole = new UserRole
          {
            WebPagesRole = wRole,
            UserInfo = nextHome.UserInfo,
            ObjectState = ObjectState.Added
          };
          nextHome.UserInfo.UserRoles.Add(userRole);
        }
        //existing
        else
        {
          uRole.ObjectState = ObjectState.Modified;
          uRole.UserInfo = nextHome.UserInfo;
          uRole.WebPagesRole = wRole;
        }

      }
    }

    private void FillNewUserHomeLInksByHomeInformation(UserHomeLink nextHome, IEnumerable<Home> tempHomeCheck)
    {
      if (tempHomeCheck.Count() > 0)
      {
        nextHome.Home = new Home();
        nextHome.Home.ObjectState = ObjectState.Modified;
        nextHome.Home = tempHomeCheck.FirstOrDefault();
      }
    }

    private void FillExistingUserHomeLinks(UserHomeLink nextHome, UserHomeLink existingItem)
    {
      existingItem.AppsHomeId = nextHome.AppsHomeId;
      existingItem.AppsUserId = nextHome.AppsUserId;
      existingItem.IsAdmin = nextHome.IsAdmin;
      existingItem.IsSynced = nextHome.IsSynced;
      //existingItem.Home = new Home();
      //existingItem.UserInfo = new UserInfo();
      existingItem.Home = existingItem.Home;
      existingItem.UserInfo = existingItem.UserInfo;
      existingItem.ObjectState = ObjectState.Modified;
    }

    private void FillExistingHomeInfo(Home item, Home existingItem)
    {

      existingItem.HomeId = item.HomeId;
      existingItem.Name = item.Name;
      existingItem.TimeZone = item.TimeZone;
      //existingItem.RegistrationKey = item.RegistrationKey;
      //existingItem.HardwareId = item.HardwareId;
      //existingItem.TrialCount = item.TrialCount;
      //existingItem.Comment = item.Comment;
      existingItem.IsActive = item.IsActive;
      existingItem.IsDefault = item.IsDefault;
      //existingItem.IsAdmin = item.IsAdmin;
      existingItem.MeshMode = item.MeshMode;
      existingItem.Phone = item.Phone;
      existingItem.PassPhrase = item.PassPhrase;
      existingItem.IsInternet = item.IsInternet;
      existingItem.ObjectState = ObjectState.Modified;

      AddOrEditExistingRoomItems(item, existingItem);
      AddOrEditExistingRourterItems(item, existingItem);
    }

    private void FillExistingUserRoles(UserHomeLink nextHome, UserHomeLink existingItem)
    {
      var wRole = GetWebPagesRole(nextHome.IsAdmin == true ? 1 : 2);
      var uRole = GetUserRole(existingItem.UserInfo.UserInfoId);
      uRole.ObjectState = ObjectState.Modified;
      uRole.WebPagesRole = wRole;

    }

    #region Smart router




    private void AddOrEditExistingRourterItems(Home item, Home existingItem)
    {
      //foreach (var nextRouter in item.SmartRouterInfoes)
      //{
      //    var tempRouter = _smartRouterInfoRepository.Query(p => p.Id == nextRouter.Id && p.HId == nextRouter.HId).Select();
      //    //var tempRouter = _smartRouterInfoRepository.Query(p => p.MacAddress==nextRouter.MacAddress).Select();
      //    if (tempRouter != null && tempRouter.Count() > 0)
      //    {
      //        var tempExistingRouter = existingItem.SmartRouterInfoes.Where(p => p.Id == nextRouter.Id && p.HId == nextRouter.HId).FirstOrDefault();
      //        //modify
      //        FillExistingRouterInfo(nextRouter, tempExistingRouter);
      //    }
      //    else
      //    {
      //        //add
      //        existingItem.SmartRouterInfoes.Add(nextRouter);
      //    }
      //}
    }


    private void FillExistingRouterInfo(RouterInfo nextRouterDetail, RouterInfo tempExistingRouterDetail)
    {
      tempExistingRouterDetail.ObjectState = ObjectState.Modified;
      //tempExistingRouterDetail.Id = nextRouterDetail.Id;
      //tempExistingRouterDetail.HId = nextRouterDetail.HId;
      tempExistingRouterDetail.LocalBrokerUsername = nextRouterDetail.LocalBrokerUsername;
      tempExistingRouterDetail.LocalBrokerPassword = nextRouterDetail.LocalBrokerPassword;
      tempExistingRouterDetail.Ssid = nextRouterDetail.Ssid;
      tempExistingRouterDetail.IsSynced = nextRouterDetail.IsSynced;
      //tempExistingRouterDetail.IsActive = nextRouterDetail.IsActive;
      tempExistingRouterDetail.AuditField = new AuditFields();
    }
    #endregion




    private void AddOrEditExistingRoomItems(Home item, Home existingItem)
    {
      foreach (var nextRoom in item.Rooms)
      {
        var tempExistingRoom = existingItem.Rooms.Where(p => p.AppsRoomId == nextRoom.AppsRoomId).FirstOrDefault();
        if (tempExistingRoom != null)
        {
          //modify
          FillExistingRoomInfo(nextRoom, tempExistingRoom);
        }
        else
        {
          //add
          existingItem.Rooms.Add(nextRoom);
        }
      }
    }

    private void FillExistingRoomInfo(Room nextRoomDetail, Room tempExistingRoomDetail)
    {
      tempExistingRoomDetail.ObjectState = ObjectState.Modified;
      tempExistingRoomDetail.AppsRoomId = nextRoomDetail.AppsRoomId;
      tempExistingRoomDetail.AppsHomeId = nextRoomDetail.AppsHomeId;
      tempExistingRoomDetail.Name = nextRoomDetail.Name;
      tempExistingRoomDetail.RoomNumber = nextRoomDetail.RoomNumber;
      tempExistingRoomDetail.Comment = nextRoomDetail.Comment;
      tempExistingRoomDetail.IsMasterRoom = nextRoomDetail.IsMasterRoom;
      tempExistingRoomDetail.IsActive = nextRoomDetail.IsActive;
      tempExistingRoomDetail.AuditField = new AuditFields();
    }

    private IEnumerable<Home> IsHomeExists(string HId)
    {
      //return _userHomeLinkRepository.Query(e => e.HId == HId).Include(x => x.Home).Include(x => x.Home.SmartRouterInfoes).Include(x => x.Home.Rooms).Select();
      return _homeRepository.Query(e => e.AppsHomeId.ToString() == HId).Include(x => x.SmartRouterInfos).Include(x => x.Rooms).Select();

    }

    private IEnumerable<Room> IsRoomExists(string RId)
    {
      return _roomRepository.Query(e => e.AppsRoomId.ToString() == RId).Select();

    }


    private IEnumerable<UserHomeLink> IsHomeAndUserExists(int HId, int UInfoId)
    {
      return _userHomeLinkRepository.Query(e => e.AppsHomeId == HId && e.AppsUserId == UInfoId).Include(x => x.Home).Include(x => x.Home.SmartRouterInfos).Include(x => x.Home.Rooms).Include(x => x.UserInfo).Select();
    }

    private IEnumerable<UserRoomLink> IsRoomAndUserExists(int RId, int UInfoId)
    {
      return _userRoomLinkRepository.Query(e => e.AppsRoomId == RId && e.AppsUserId == UInfoId).Include(x => x.Room).Include(x => x.UserInfo).Select();
    }





    #endregion


    #region Userinfo AddOrUpdateUserInfoGraphRange
    public IEnumerable<UserInfo> AddOrUpdateUserInfoGraphRange(IEnumerable<UserInfo> model)
    {
      List<UserInfo> userInfoModel = new List<UserInfo>();
      userInfoModel = FillUserInformations(model, userInfoModel);
      _userInfoRepository.InsertOrUpdateGraphRange(userInfoModel);
      return userInfoModel;
    }

    public List<UserInfo> FillUserInformations(IEnumerable<UserInfo> model, List<UserInfo> userInfoModel)
    {
      foreach (var item in model)
      {
        //check already exist or not.
        IEnumerable<UserInfo> temp = IsUserExists(item.Email, item.Password);
        if (temp.Count() == 0)
        {
          item.DateOfBirth = System.DateTime.Now;
          //new item
          userInfoModel.Add(item);
          continue;
        }
        else
        {
          //existing item               
          // versionModel = temp;
          foreach (var existingItem in temp.ToList())
          {
            //modify version                    
            FillExistingUserInfo(item, existingItem);

          }
        }
      }

      return userInfoModel;
    }


    private void FillExistingUserInfo(UserInfo item, UserInfo existingItem)
    {
      existingItem.AppsUserId = item.AppsUserId;
      existingItem.LocalId = item.LocalId;
      existingItem.Password = item.Password;
      existingItem.UserName = item.UserName;
      existingItem.FirstName = item.FirstName;
      existingItem.LastName = item.LastName;
      existingItem.MiddleName = item.MiddleName;
      //existingItem.FullName = item.FullName;
      existingItem.AccNo = item.AccNo;
      existingItem.CellPhone = item.CellPhone;
      existingItem.DateOfBirth = item.DateOfBirth;
      existingItem.Sex = item.Sex;
      existingItem.Email = item.Email;
      existingItem.ExpireDate = item.ExpireDate;
      existingItem.OldAcc = item.OldAcc;
      existingItem.SocialSecurityNumber = item.SocialSecurityNumber;
      existingItem.IsEmailRecipient = item.IsEmailRecipient;
      existingItem.IsLoggedIn = item.IsLoggedIn;
      existingItem.IsSMSRecipient = item.IsSMSRecipient;
      existingItem.LastLogIn = item.LastLogIn;
      existingItem.IsActive = item.IsActive;
      existingItem.Country = item.Country;
      existingItem.LoginStatus = item.LoginStatus;
      existingItem.RegStatus = item.RegStatus;
      existingItem.IsSynced = item.IsSynced;

      existingItem.ObjectState = ObjectState.Modified;
    }

    private IEnumerable<UserInfo> IsUserExists(string email, string password)
    {
      return _userInfoRepository.Query(e => e.Email == email && e.Password == password).Select();
    }

    //private IEnumerable<UserInfo> IsUserExists(string email, string password)
    //{
    //    return _userInfoRepository.Query(e => e.Id == key).Select();
    //}

    private WebPagesRole GetWebPagesRole(int roleId)
    {
      return _webPagesRoleRepository.Query(e => e.RoleId == roleId).Select().FirstOrDefault();
    }

    private UserRole GetUserRole(long userInfoId)
    {
      return _userRoleRepository.Query(e => e.UserInfo.UserInfoId == userInfoId).Select().FirstOrDefault();
    }



    #endregion



    #region Home AddOrUpdateGraphRange
    //public IEnumerable<Home> AddOrUpdateHomeGraphRange(IEnumerable<Home> model)
    //{
    //    List<Home> homeModel = new List<Home>();
    //    homeModel = FillHomeInformations(model, homeModel);
    //    _homeRepository.InsertOrUpdateGraphRange(homeModel);
    //    return homeModel;
    //}

    //public List<Home> FillHomeInformations(IEnumerable<Home> model, List<Home> homeModel)
    //{
    //    foreach (var item in model)
    //    {
    //        //check already exist or not.
    //        IEnumerable<Home> temp = IsHomeExists(item.Id);
    //        if (temp.Count() == 0)
    //        {
    //            //new item
    //            homeModel.Add(item);
    //            continue;
    //        }
    //        else
    //        {
    //            //existing item               
    //            // versionModel = temp;
    //            foreach (var existingItem in temp.ToList())
    //            {
    //                //modify version                    
    //                FillExistingHomeInfo(item, existingItem);

    //                if (item.Rooms != null && item.Rooms.Count > 0)
    //                {
    //                    AddOrEditExistingRoomItems(item, existingItem);
    //                }

    //            }
    //        }
    //    }

    //    return homeModel;
    //}



    //private void AddOrEditExistingRoomItems(Home item, Home existingItem)
    //{
    //    foreach (var nextRoom in item.Rooms)
    //    {
    //        var tempExistingRoom = existingItem.Rooms.Where(p => p.Id == nextRoom.Id).FirstOrDefault();
    //        if (tempExistingRoom != null)
    //        {
    //            //modify
    //            FillExistingRoomInfo(nextRoom, tempExistingRoom);
    //        }
    //        else
    //        {
    //            //add
    //            existingItem.Rooms.Add(nextRoom);
    //        }
    //    }
    //}

    //private void FillExistingRoomInfo(Room nextRoomDetail, Room tempExistingRoomDetail)
    //{
    //    tempExistingRoomDetail.ObjectState = ObjectState.Modified;
    //    tempExistingRoomDetail.Id = nextRoomDetail.Id;
    //    tempExistingRoomDetail.HId = nextRoomDetail.HId;
    //    tempExistingRoomDetail.Name = nextRoomDetail.Name;
    //    tempExistingRoomDetail.RoomNumber = nextRoomDetail.RoomNumber;
    //    tempExistingRoomDetail.Comment = nextRoomDetail.Comment;
    //    tempExistingRoomDetail.IsMasterRoom = nextRoomDetail.IsMasterRoom;
    //    tempExistingRoomDetail.IsActive = nextRoomDetail.IsActive;
    //    tempExistingRoomDetail.AuditField = new AuditFields();
    //}

    //private void FillExistingHomeInfo(Home item, Home existingItem)
    //{

    //    existingItem.Id = item.Id;
    //    existingItem.Name = item.Name;
    //    existingItem.TimeZone = item.TimeZone;
    //    //existingItem.RegistrationKey = item.RegistrationKey;
    //    //existingItem.HardwareId = item.HardwareId;
    //    //existingItem.TrialCount = item.TrialCount;
    //    existingItem.Comment = item.Comment;
    //    existingItem.IsActive = item.IsActive;
    //    existingItem.IsDefault = item.IsDefault;
    //    existingItem.IsAdmin = item.IsAdmin;
    //    existingItem.MeshMode = item.MeshMode;
    //    existingItem.Phone = item.Phone;
    //    existingItem.PassPhrase = item.PassPhrase;
    //    existingItem.IsInternet = item.IsInternet;

    //    existingItem.ObjectState = ObjectState.Modified;
    //}

    //private IEnumerable<Home> IsHomeExists(string key)
    //{
    //    return _homeRepository.Query(e => e.Id == key).Include(x => x.Rooms).Select();
    //}



    #endregion


    #region version AddOrUpdateGraphRange
    public IEnumerable<Model.Models.Version> AddOrUpdateVersionGraphRange(IEnumerable<Model.Models.Version> model)
    {
      List<Model.Models.Version> versionModel = new List<Model.Models.Version>();
      versionModel = FillVersionInformations(model, versionModel);
      _versionRepository.InsertOrUpdateGraphRange(versionModel);
      return versionModel;
    }

    public List<Model.Models.Version> FillVersionInformations(IEnumerable<Model.Models.Version> model, List<Model.Models.Version> versionModel)
    {
      foreach (var item in model)
      {
        //check already exist or not.
        IEnumerable<Model.Models.Version> temp = IsVersionExists(item.AppsVersionId, item.Mac);
        if (temp.Count() == 0)
        {
          //new item
          versionModel.Add(item);
          continue;
        }
        else
        {
          //existing item               
          // versionModel = temp;
          foreach (var existingItem in temp.ToList())
          {
            //modify version                    
            FillExistingVersionInfo(item, existingItem);

            if (item.VersionDetails != null && item.VersionDetails.Count > 0)
            {
              AddOrEditExistingVDetailItems(item, existingItem);
            }

          }
        }
      }

      return versionModel;
    }



    private void AddOrEditExistingVDetailItems(Model.Models.Version item, Model.Models.Version existingItem)
    {
      foreach (var nextVDetail in item.VersionDetails)
      {
        var tempExistingVDetail = existingItem.VersionDetails.Where(p => p.AppsVersionDetailId == nextVDetail.AppsVersionDetailId).FirstOrDefault();
        if (tempExistingVDetail != null)
        {
          //modify
          FillExistingVDetailInfo(nextVDetail, tempExistingVDetail);
        }
        else
        {
          //add
          existingItem.VersionDetails.Add(nextVDetail);
        }
      }
    }

    private void FillExistingVDetailInfo(VersionDetail nextVDetail, VersionDetail tempExistingVDetail)
    {
      tempExistingVDetail.ObjectState = ObjectState.Modified;
      tempExistingVDetail.AppsVersionDetailId = nextVDetail.AppsVersionDetailId;
      tempExistingVDetail.AppsVersionId = nextVDetail.AppsVersionId;
      tempExistingVDetail.HardwareVersion = nextVDetail.HardwareVersion;
      tempExistingVDetail.DeviceType = nextVDetail.DeviceType;
      tempExistingVDetail.AuditField = new AuditFields();
    }

    private void FillExistingVersionInfo(Model.Models.Version item, Model.Models.Version existingItem)
    {
      existingItem.AppName = item.AppName;
      existingItem.AppVersion = item.AppVersion;
      existingItem.AuditField = new AuditFields();
      existingItem.AuthCode = item.AuthCode;
      existingItem.Mac = item.Mac;
      existingItem.AppsVersionId = item.AppsVersionId;
      existingItem.ObjectState = ObjectState.Modified;
    }

    private IEnumerable<Model.Models.Version> IsVersionExists(int key, string Mac)
    {
      return _versionRepository.Query(e => e.AppsVersionId == key && e.Mac == Mac).Include(x => x.VersionDetails).Select();
    }



    #endregion


    #region device AddOrUpdateGraphRange


    public IEnumerable<SmartDevice> AddOrUpdateDeviceGraphRange(IEnumerable<SmartDevice> model, IEnumerable<SmartDeviceEntity> modelEntity)
    {
      List<SmartDevice> deviceModel = new List<SmartDevice>();
      //#region MyRegion
      foreach (SmartDevice item in model)
      {
        item.Room = _roomRepository.Find(modelEntity.FirstOrDefault(p => p.AppsDeviceId == item.AppsDeviceId).AppsRoomId);
      }

      //#endregion

      deviceModel = FillDeviceInformations(model, deviceModel);
      _deviceRepository.InsertOrUpdateGraphRange(deviceModel);
      return deviceModel;
    }


    public List<SmartDevice> FillDeviceInformations(IEnumerable<SmartDevice> model, List<SmartDevice> deviceModel)
    {
      FillSmartSwitch(model, deviceModel);
      FillSmartRainbox(model, deviceModel);
      FillSmartRouter(model, deviceModel);
      return deviceModel;
    }

    #region SmartRouter

    private void FillSmartRouter(IEnumerable<SmartDevice> model, List<SmartDevice> deviceModel)
    {
      foreach (var item in model.Where(p => p.DeviceType == Model.Enums.DeviceType.SmartRouter))
      {
        //check already exist or not.
        IEnumerable<SmartDevice> temp = IsDeviceRouterExists(item.AppsDeviceId, item.DeviceHash);
        if (temp.Count() == 0)
        {
          //new item
          deviceModel.Add(item);
          continue;
        }
        else
        {
          foreach (var existingItem in temp.ToList())
          {
            //modify version                    
            FillExistingDeviceInfo(item, existingItem);

            if (item.DeviceStatus != null && item.DeviceStatus.Count > 0)
            {
              AddOrEditExistingDeviceStatus(item, existingItem);

            }

          }
        }
      }
    }


    #endregion



    private void FillSmartRainbox(IEnumerable<SmartDevice> model, List<SmartDevice> deviceModel)
    {
      foreach (var item in model.Where(p => p.DeviceType == Model.Enums.DeviceType.SmartRainbow12))
      {
        //check already exist or not.
        IEnumerable<SmartDevice> temp = IsDeviceRainboxExists(item.AppsDeviceId, item.DeviceHash);
        if (temp.Count() == 0)
        {
          //new item
          deviceModel.Add(item);
          continue;
        }
        else
        {
          foreach (var existingItem in temp.ToList())
          {
            //modify version                    
            FillExistingDeviceInfo(item, existingItem);

            if (item.DeviceStatus != null && item.DeviceStatus.Count > 0)
            {
              AddOrEditExistingDeviceStatus(item, existingItem);
              AddOrEditExistingRgbwStatus(item, existingItem);

            }

          }
        }
      }
    }

    private void FillSmartSwitch(IEnumerable<SmartDevice> model, List<SmartDevice> deviceModel)
    {
      foreach (var item in model.Where(p => p.DeviceType == Model.Enums.DeviceType.SmartSwitch6g))
      {
        //check already exist or not.
        IEnumerable<SmartDevice> temp = IsDeviceSwitchExists(item.AppsDeviceId, item.DeviceHash);
        if (temp.Count() == 0)
        {
          //new item
          deviceModel.Add(item);
          continue;
        }
        else
        {
          foreach (var existingItem in temp.ToList())
          {
            //modify version                    
            FillExistingDeviceInfo(item, existingItem);

            if (item.DeviceStatus != null && item.DeviceStatus.Count > 0)
            {
              AddOrEditExistingDeviceStatus(item, existingItem);
              //AddOrEditExistingRgbwStatus(item, existingItem);
              AddOrEditExistingChannels(item, existingItem);
            }

          }
        }
      }
    }

    private void AddOrEditExistingChannels(SmartDevice item, SmartDevice existingItem)
    {
      foreach (var nextChannel in ((SmartSwitch)item).Channels)
      {
        var tempExistingChannel = ((SmartSwitch)existingItem).Channels.Where(p => p.AppsChannelId == nextChannel.AppsChannelId).FirstOrDefault();
        if (tempExistingChannel != null)
        {
          //modify
          FillExistingChannelInfo(nextChannel, tempExistingChannel);
        }
        else
        {
          //add
          ((SmartSwitch)existingItem).Channels.Add(nextChannel);
        }
      }
    }

    private void FillExistingChannelInfo(Channel nextChannel, Channel tempExistingChannel)
    {
      tempExistingChannel.ObjectState = ObjectState.Modified;
      tempExistingChannel.AppsChannelId = nextChannel.AppsChannelId;
      tempExistingChannel.AppsDeviceTableId = nextChannel.AppsDeviceTableId;
      tempExistingChannel.ChannelNo = nextChannel.ChannelNo;
      tempExistingChannel.LoadName = nextChannel.LoadName;
      tempExistingChannel.LoadType = nextChannel.LoadType;
      tempExistingChannel.AuditField = new AuditFields();

      AddOrEditExistingChannelStatus(nextChannel, tempExistingChannel);

    }

    private void AddOrEditExistingChannelStatus(Channel nextChannel, Channel tempExistingChannel)
    {

      //channel status
      foreach (var nextChannelStatus in nextChannel.ChannelStatuses)
      {
        var tempExistingChannelStatus = tempExistingChannel.ChannelStatuses.Where(p => p.AppsChannelStatusId == nextChannelStatus.AppsChannelStatusId && p.AppsChannelId == nextChannelStatus.AppsChannelId).FirstOrDefault();
        if (tempExistingChannelStatus != null)
        {
          //modify
          FillExistingChannelStatusInfo(nextChannelStatus, tempExistingChannelStatus);
        }
        else
        {
          //add
          tempExistingChannel.ChannelStatuses.Add(nextChannelStatus);
        }
      }
    }

    private void FillExistingChannelStatusInfo(ChannelStatus nextChannelStatus, ChannelStatus tempExistingChannelStatus)
    {
      tempExistingChannelStatus.ObjectState = ObjectState.Modified;
      tempExistingChannelStatus.AppsChannelStatusId = nextChannelStatus.AppsChannelStatusId;
      tempExistingChannelStatus.AppsChannelId = nextChannelStatus.AppsChannelId;
      //tempExistingChannelStatus.DId = nextChannelStatus.DId;
      //tempExistingChannelStatus.ChannelNo = nextChannelStatus.ChannelNo;
      tempExistingChannelStatus.StatusType = nextChannelStatus.StatusType;
      tempExistingChannelStatus.StatusValue = nextChannelStatus.StatusValue;
      tempExistingChannelStatus.AuditField = new AuditFields();
    }


    #region AddOrEditExistingRgbwStatus
    private void AddOrEditExistingRgbwStatus(SmartDevice item, SmartDevice existingItem)
    {
      foreach (var nextStatus in ((SmartRainbow)item).RgbwStatuses)
      {
        var tempExistingDStatus = ((SmartRainbow)existingItem).RgbwStatuses.Where(p => p.AppsRgbtStatusId == nextStatus.AppsRgbtStatusId).FirstOrDefault();
        if (tempExistingDStatus != null)
        {
          //modify
          FillExistingRgbInfo(nextStatus, tempExistingDStatus);
        }
        else
        {
          //add
          ((SmartRainbow)existingItem).RgbwStatuses.Add(nextStatus);
        }
      }
    }

    private void FillExistingRgbInfo(RgbwStatus nextStatus, RgbwStatus tempExistingStatus)
    {
      tempExistingStatus.ObjectState = ObjectState.Modified;
      tempExistingStatus.AppsRgbtStatusId = nextStatus.AppsRgbtStatusId;
      tempExistingStatus.AppsDeviceId = nextStatus.AppsDeviceId;
      tempExistingStatus.RGBColorStatusType = nextStatus.RGBColorStatusType;
      tempExistingStatus.IsPowerOn = nextStatus.IsPowerOn;
      tempExistingStatus.ColorR = nextStatus.ColorR;
      tempExistingStatus.ColorG = nextStatus.ColorG;
      tempExistingStatus.ColorB = nextStatus.ColorB;
      tempExistingStatus.IsWhiteEnabled = nextStatus.IsWhiteEnabled;
      tempExistingStatus.DimmingValue = nextStatus.DimmingValue;
      tempExistingStatus.AuditField = new AuditFields();
    }
    #endregion

    #region AddOrEditExistingDeviceStatus
    private void AddOrEditExistingDeviceStatus(SmartDevice item, SmartDevice existingItem)
    {
      foreach (var nextDStatus in item.DeviceStatus)
      {
        var tempExistingDStatus = existingItem.DeviceStatus.Where(p => p.AppsDeviceStatusId == nextDStatus.AppsDeviceStatusId).FirstOrDefault();
        if (tempExistingDStatus != null)
        {
          //modify
          FillExistingVDetailInfo(nextDStatus, tempExistingDStatus);
        }
        else
        {
          //add
          existingItem.DeviceStatus.Add(nextDStatus);
        }
      }
    }

    private void FillExistingVDetailInfo(DeviceStatus nextDStatus, DeviceStatus tempExistingDStatus)
    {
      tempExistingDStatus.ObjectState = ObjectState.Modified;
      tempExistingDStatus.AppsDeviceStatusId = nextDStatus.AppsDeviceStatusId;
      tempExistingDStatus.AppsDeviceId = nextDStatus.AppsDeviceId;
      tempExistingDStatus.StatusType = nextDStatus.StatusType;
      tempExistingDStatus.Status = nextDStatus.Status;
      tempExistingDStatus.AuditField = new AuditFields();
    }
    #endregion

    private void FillExistingDeviceInfo(SmartDevice item, SmartDevice existingItem)
    {
      existingItem.ObjectState = ObjectState.Modified;

      existingItem.AppsDeviceId = item.AppsDeviceId;
      existingItem.AppsBleId = item.AppsBleId;
      existingItem.DeviceName = item.DeviceName;
      existingItem.DeviceHash = item.DeviceHash;
      existingItem.FirmwareVersion = item.FirmwareVersion;
      existingItem.IsDeleted = item.IsDeleted;
      existingItem.Watt = item.Watt;
      //existingItem.Mac = item.Mac;
      existingItem.DeviceType = item.DeviceType;
      existingItem.AuditField = new AuditFields();
      existingItem.Room = item.Room;

    }


    private IEnumerable<SmartDevice> IsDeviceSwitchExists(int key, string deviceHash)
    {
      return _deviceRepository.Queryable().Include(x => x.DeviceStatus).OfType<SmartSwitch>().Where(e => e.AppsDeviceId == key && e.DeviceHash == deviceHash).Include(x => x.Channels).Include(x => x.Room).Include(x => x.Channels.Select(y => y.ChannelStatuses)).ToList();

    }

    private IEnumerable<SmartDevice> IsDeviceRainboxExists(int key, string deviceHash)
    {
      return _deviceRepository.Queryable().Include(x => x.DeviceStatus).OfType<SmartRainbow>().Where(e => e.AppsDeviceId == key && e.DeviceHash == deviceHash).Include(x => x.RgbwStatuses).Include(x => x.Room).ToList();

    }

    private IEnumerable<SmartDevice> IsDeviceRouterExists(int key, string deviceHash)
    {
      return _deviceRepository.Queryable().Include(x => x.DeviceStatus).OfType<SmartRouter>().Where(e => e.AppsDeviceId == key && e.DeviceHash == deviceHash).ToList();

    }



    private IEnumerable<SmartDevice> IsDeviceExists(int key, string deviceHash)
    {

      return _deviceRepository.Query(e => e.AppsDeviceId == key && e.DeviceHash == deviceHash).Include(x => x.DeviceStatus).Include(x => x.Room).Include(x => ((SmartRainbow)x).RgbwStatuses).Include(x => ((SmartSwitch)x).Channels.Select(y => y.ChannelStatuses)).Select();
    }


    #endregion


    public IEnumerable<DeviceInfoEntity> GetsDeviceAllInfo()
    {
      // add business logic here
      // return _repository.GetsAllDevice();

      int parentSequence = 0;
      int channelMasterID = 0;
      string displayCaption = string.Empty;
      List<DeviceInfoEntity> dInfoEntity = new List<DeviceInfoEntity>();
      var device = _deviceRepository.Query().Include(x => x.DeviceStatus).Include(x => ((SmartRainbow)x).RgbwStatuses).Include(x => ((SmartSwitch)x).Channels.Select(y => y.ChannelStatuses)).Select().ToList();

      foreach (SmartDevice nextDeviceInfo in device)
      {

        DeviceInfoEntity deviceInfo = new DeviceInfoEntity();
        FillDeviceInfo(out parentSequence, out displayCaption, dInfoEntity, nextDeviceInfo, out deviceInfo);

        deviceInfo = AddCaption(parentSequence, "DeviceStatus", dInfoEntity);
        channelMasterID = deviceInfo.SequenceId;
        foreach (DeviceStatus nextDStatus in nextDeviceInfo.DeviceStatus)
        {
          AddCaption(channelMasterID, "StatusType--" + nextDStatus.StatusType.ToString() + ", Value--" + nextDStatus.Value.ToString(), dInfoEntity);
        }

        deviceInfo = AddCaption(parentSequence, "RgbwStatuses", dInfoEntity);
        channelMasterID = deviceInfo.SequenceId;
        foreach (RgbwStatus nextStatus in ((SmartRainbow)nextDeviceInfo).RgbwStatuses)
        {
          displayCaption = " StatusType--" + nextStatus.RGBColorStatusType.ToString() + ", IsPowerOn--" + nextStatus.IsPowerOn + ", ColorR--" + nextStatus.ColorR.ToString() + ", ColorG--" + nextStatus.ColorG.ToString() + ", ColorB--" + nextStatus.ColorB.ToString() + ", DimmingValue--" + nextStatus.DimmingValue.ToString() + ", IsWhiteEnabled--" + nextStatus.IsWhiteEnabled.ToString();
          AddCaption(channelMasterID, displayCaption, dInfoEntity);

        }
        deviceInfo = AddCaption(parentSequence, "Channel", dInfoEntity);
        channelMasterID = deviceInfo.SequenceId;

        foreach (Channel nextChannelInfo in ((SmartSwitch)nextDeviceInfo).Channels)
        {
          displayCaption = " ChannelNo--" + nextChannelInfo.ChannelNo.ToString() + ", LoadName--" + nextChannelInfo.LoadName + ", LoadType--" + nextChannelInfo.LoadType.ToString();
          deviceInfo = AddCaption(channelMasterID, displayCaption, dInfoEntity);

          foreach (ChannelStatus nextCStatus in nextChannelInfo.ChannelStatuses)
          {
            displayCaption = " Status--" + nextCStatus.StatusType.ToString() + ", Value--" + nextCStatus.StatusValue.ToString();//nextChannelInfo.ChannelNo.ToString();
            AddCaption(deviceInfo.SequenceId, displayCaption, dInfoEntity);
          }
        }


      }

      return dInfoEntity;
    }

    private void FillDeviceInfo(out int parentSequence, out string displayCaption, List<DeviceInfoEntity> dInfoEntity, SmartDevice nextDeviceInfo, out DeviceInfoEntity deviceInfo)
    {
      displayCaption = "DId--" + nextDeviceInfo.AppsBleId + ", DeviceName--" + nextDeviceInfo.DeviceName + ", DeviceHash--" + nextDeviceInfo.DeviceHash + ", DType--" + nextDeviceInfo.DeviceType.ToString();
      deviceInfo = AddCaption(0, displayCaption, dInfoEntity);
      parentSequence = deviceInfo.SequenceId;
    }

    private DeviceInfoEntity AddCaption(int parentId, string caption, List<DeviceInfoEntity> dInfoEntity)
    {
      DeviceInfoEntity deviceInfo = new DeviceInfoEntity();
      deviceInfo.DisplayName = caption;
      deviceInfo.SequenceId = dInfoEntity.Count() + 1;
      deviceInfo.ParentId = parentId;
      dInfoEntity.Add(deviceInfo);
      return deviceInfo;
    }

    public IEnumerable<VersionInfoEntity> GetsAllVersion()
    {
      // add business logic here
      // return _repository.GetsAllVersion();

      int parentSequence = 0;
      List<VersionInfoEntity> vInfoEntity = new List<VersionInfoEntity>();
      var version = _versionRepository.Query().Include(x => x.VersionDetails).Select().ToList();
      foreach (Model.Models.Version nextVersion in version)
      {
        //AppName,AuthCode,PassPhrase
        VersionInfoEntity versionInfo = new VersionInfoEntity();
        versionInfo.DisplayName = "AppName--" + nextVersion.AppName + ", AuthCode--" + nextVersion.AuthCode + ", PassPhrase--" + nextVersion.PassPhrase;
        versionInfo.ParentId = 0;
        versionInfo.SequenceId = vInfoEntity.Count() + 1;
        parentSequence = versionInfo.SequenceId;
        vInfoEntity.Add(versionInfo);
        foreach (VersionDetail nextVDetail in nextVersion.VersionDetails)
        {
          versionInfo = new VersionInfoEntity();
          versionInfo.DisplayName = "DeviceType--" + nextVDetail.DeviceType;
          versionInfo.ParentId = parentSequence;
          versionInfo.SequenceId = vInfoEntity.Count() + 1;
          vInfoEntity.Add(versionInfo);
        }
      }
      return vInfoEntity;
    }

    #region Gets homes infos

    public HomeViewModel GetsHomesAllInfo(long userInfoId)
    {
      try
      {
        _userHomeLinks = new List<UserHomeLink>();
        _homeViewModel = new HomeViewModel();
        UserHomeLink userHomeLink = GetUserHomeLink(userInfoId);

        if (userHomeLink == null)
        {
          GetHomeLessUserInfos(userInfoId);
        }
        else
        {
          GetHomeRelatedUserInfos(userInfoId, userHomeLink);
        }
        return _homeViewModel;
      }
      catch (System.Exception ex)
      {
        return null;
      }
    }

    private void GetHomeRelatedUserInfos(long userInfoId, UserHomeLink userHomeLink)
    {
      IList<Home> Homes = new List<Home>();
      IList<UserInfo> userInfos = GetUserInfosByUserId(userInfoId);
      IList<long> userIds = userInfos.Select(p => p.UserInfoId).ToList();
      IList<long> roomIds = GetRoomIds(userInfoId, userHomeLink, userIds);
      _userHomeLinks = new List<UserHomeLink>();

      IList<UserInfo> users = userInfos;
      Home home = userHomeLink.Home;

      IList<RouterInfo> routers = home.SmartRouterInfos.ToList();
      IList<WebBrokerInfo> webBrokerInfoes = home.WebBrokerInfoes.ToList();      
      IList<Room> rooms = home.Rooms.Where(r => roomIds.Contains(r.RoomId)).ToList();
      IList<UserRoomLink> userrooms = GetUserRoomLinks(roomIds, userIds);
      IList<SmartDevice> devices = GetDevices(roomIds);
      IList<NextAssociatedDevice> aDevices = GetNextAssociatedDevices(home.HomeId);
      IList<long> deviceIds = devices.Select(s => s.DeviceId).ToList();
      IList<Channel> channels = GetChannels(deviceIds);
      IList<RgbwStatus> rgbStatuses = GetRgbStatuses(deviceIds);
      _userHomeLinks = GetUserHomeLinks(userHomeLink);
      Homes.Add(home);

      _homeViewModel = new HomeViewModel();
      _homeViewModel.Homes = Homes;
      _homeViewModel.Users = users;
      _homeViewModel.UserHomeLinks = _userHomeLinks;
      _homeViewModel.Routers = routers;
      _homeViewModel.WebBrokerInfoes = webBrokerInfoes;      
      _homeViewModel.Rooms = rooms;
      _homeViewModel.UserRoomLinks = userrooms;
      _homeViewModel.SmartDevices = devices;
      _homeViewModel.NextAssociatedDevice = aDevices;
      _homeViewModel.Channels = channels;
      _homeViewModel.RgbwStatuses = rgbStatuses;
    }

    private void GetHomeLessUserInfos(long userInfoId)
    {
      IList<UserInfo> users = GetUserInfoList(userInfoId);
      _homeViewModel = new HomeViewModel();
      _homeViewModel.Users = users;
    }

    private IList<UserHomeLink> GetUserHomeLinks(UserHomeLink userHomeLink)
    {
      IList<UserHomeLink> userHomeLinks = new List<UserHomeLink>();
      if (userHomeLink.IsAdmin)
      {
        userHomeLinks = _userHomeLinkRepository.Queryable().Where(p => p.Home.HomeId == userHomeLink.Home.HomeId).ToList();
      }
      else
      {
        userHomeLinks.Add(userHomeLink);
      }
      return userHomeLinks;
    }

    private IList<long> GetRoomIds(long userInfoId, UserHomeLink userHomeLink, IList<long> userIds)
    {
      IList<long> roomIds = new List<long>();
      if (userHomeLink.IsAdmin)
      {
        roomIds = _userRoomLinkRepository.Queryable()
            .Where(p => userIds.Contains(p.UserInfo.UserInfoId)).Select(s => s.Room.RoomId).ToList();
      }
      else
      {
        roomIds = _userRoomLinkRepository.Queryable()
            .Where(p => p.UserInfo.UserInfoId == userInfoId).Select(s => s.Room.RoomId).ToList();
      }


      return roomIds;
    }

    private IList<UserInfo> GetUserInfosByUserId(long userInfoId)
    {
      IList<UserInfo> users = new List<UserInfo>();
      UserHomeLink userHomeLink = GetUserHomeLinksByUserInfo(userInfoId);
      if (userHomeLink.IsAdmin)
      {
        users = _userInfoRepository.Queryable().ToList();
        var tempUserHomeLink = _userHomeLinkRepository.Queryable().Where(p => p.Home.HomeId == userHomeLink.Home.HomeId).Select(p => p.UserInfo.UserInfoId);
        var query = users.Where(p => tempUserHomeLink.Contains(p.UserInfoId));
        users = query.ToList();
      }
      else
      {
        users.Add(userHomeLink.UserInfo);
      }
      return users;
    }

    private UserHomeLink GetUserHomeLinksByUserInfo(long userInfoId)
    {
      return _userHomeLinkRepository.Queryable()
          .Where(p => p.UserInfo.UserInfoId == userInfoId)
          .Include(x => x.UserInfo)
          .FirstOrDefault();
    }


    private UserHomeLink GetUserHomeLink(long userInfoId)
    {
      try
      {
        UserHomeLink userHomeLink =
          _userHomeLinkRepository.Queryable()
              .Where(p => p.UserInfo.UserInfoId == userInfoId)
              .Include(x => x.UserInfo)
              .Include(x => x.Home)
              .Include(x => x.Home.SmartRouterInfos)
              .Include(x => x.Home.WebBrokerInfoes)
              .Include(x => x.Home.Rooms)
              .Include(x => x.Home.Rooms.Select(y => y.SmartDevices.Select(z => z.DeviceStatus)))
              .FirstOrDefault();
        return userHomeLink;
      }
      catch (Exception ex)
      {

        throw;
      }
    }
    private IList<UserInfo> GetUserInfoList(UserHomeLink userHomeLink)
    {
      IList<UserInfo> users = new List<UserInfo>();
      if (userHomeLink.IsAdmin)
      {
        users = _userInfoRepository.Queryable().ToList();
        var tempUserHomeLink = _userHomeLinkRepository.Queryable().Where(p => p.Home.HomeId == userHomeLink.Home.HomeId).Select(p => p.UserInfo.UserInfoId);
        var query = users.Where(p => tempUserHomeLink.Contains(p.UserInfoId));
        users = query.ToList();
      }
      else
      {
        users.Add(userHomeLink.UserInfo);
      }
      return users;
    }
    private IList<UserInfo> GetUserInfoList(long userInfoId)
    {
      IList<UserInfo> users = new List<UserInfo>();
      users = _userInfoRepository.Queryable().Where(p => p.UserInfoId == userInfoId).ToList();
      return users;
    }

    private IList<UserRoomLink> GetUserRoomLinks(IList<long> roomIds, IList<long> userIds)
    {
      return _userRoomLinkRepository
              .Queryable()
              .Where(w => roomIds.Contains(w.Room.RoomId) && userIds.Contains(w.UserInfo.UserInfoId))
              .ToList();
    }
    private IList<NextAssociatedDevice> GetNextAssociatedDevices(long homeId)
    {
      return _associatedDeviceRepository
     .Queryable()
     .Where(w => homeId == w.Home.HomeId)
     .ToList();
    }

    private IList<SmartDevice> GetDevices(IList<long> roomIds)
    {
      return _deviceRepository
                  .Queryable()
                  .Where(w => roomIds.Contains(w.Room.RoomId))
                  .Include(i => i.DeviceStatus)
                  .ToList();
    }
    private IList<Channel> GetChannels(IList<long> deviceIds)
    {
      return _channelRepository
                  .Queryable()
                  .Where(w => deviceIds.Contains(w.SmartSwitch.DeviceId))
                  .Include(i => i.ChannelStatuses)
                  .ToList();
    }
    private IList<RgbwStatus> GetRgbStatuses(IList<long> deviceIds)
    {
      return _rgbStatusRepository
                  .Queryable()
                  .Where(w => deviceIds.Contains(w.SmartRainbow.DeviceId))
                  .ToList();
    }
    #endregion

    #region Gets App version infos

    public List<Model.Models.Version> GetsAppVersionAllInfo()
    {

      var temp = _versionRepository.Query()
          .Include(x => x.VersionDetails).Select().ToList();
      return temp;
    }

    #endregion
  }
}
