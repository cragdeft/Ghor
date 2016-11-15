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

namespace SmartHome.Service.CameraInfoServices
{
  public class CameraConfigNewEntryJsonParserService : IHomeJsonParserService<CameraConfigInfo>
  {
    #region PrivateProperty
    private readonly IUnitOfWorkAsync _unitOfWorkAsync;
    private readonly IRepositoryAsync<Home> _homeRepository;
    private readonly IRepositoryAsync<CameraConfigInfo> _cameraConfigRepository;


    public HomeJsonEntity _homeJsonEntity { get; private set; }
    public string _homeJsonMessage { get; private set; }
    public MessageReceivedFrom _receivedFrom { get; private set; }
    public MessageLog _messageLog { get; private set; }

    #endregion
    public CameraConfigNewEntryJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
    {
      _unitOfWorkAsync = unitOfWorkAsync;
      _cameraConfigRepository = _unitOfWorkAsync.RepositoryAsync<CameraConfigInfo>();

      _homeJsonEntity = homeJsonEntity;
      _homeJsonMessage = homeJsonMessage;
      _receivedFrom = receivedFrom;
      _messageLog = new MessageLog();
      SetMapper();
    }
    public CameraConfigInfo SaveJsonData()
    {
      CameraConfigInfo cameraConfig = null;
      try
      {
        cameraConfig = SaveNewCameraInfoConfig();
      }
      catch (Exception ex)
      {
        return null;
      }
      return cameraConfig;
    }
    public CameraConfigInfo SaveNewCameraInfoConfig()
    {
      string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
      //string cameraIp = _homeJsonEntity.CameraConfigInfo.FirstOrDefault().CameraIp;

      Home home = null;

      home = new CommonService(_unitOfWorkAsync).GetHome(passPhrase);

      if (home != null)
      {
        return InsertCameraInfo(_homeJsonEntity.CameraConfigInfo[0], home);
      }
      return null;

    }
    
    public CameraConfigInfo InsertCameraInfo(CameraConfigInfoEntity cameraEntity, Home home)
    {
      var entity = Mapper.Map<CameraConfigInfoEntity, CameraConfigInfo>(cameraEntity);
      entity.IsSynced = Convert.ToBoolean(cameraEntity.IsSynced);
      entity.Parent = home;
      entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
      entity.ObjectState = ObjectState.Added;
      _cameraConfigRepository.Insert(entity);
      return entity;
    }

    private void SetMapper()
    {
      Mapper.CreateMap<CameraConfigInfoEntity, CameraConfigInfo>();
    }

  
  }
}
