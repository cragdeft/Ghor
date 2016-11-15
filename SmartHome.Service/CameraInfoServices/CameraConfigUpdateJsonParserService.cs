using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.Models;
using SmartHome.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service.CameraInfoServices
{

  public class CameraConfigUpdateJsonParserService : IHomeUpdateJsonParserService<CameraConfigInfo>
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
    public CameraConfigUpdateJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
    {
      _unitOfWorkAsync = unitOfWorkAsync;
      _cameraConfigRepository = _unitOfWorkAsync.RepositoryAsync<CameraConfigInfo>();

      _homeJsonEntity = homeJsonEntity;
      _homeJsonMessage = homeJsonMessage;
      _receivedFrom = receivedFrom;
      _messageLog = new MessageLog();
      SetMapper();
    }
    public CameraConfigInfo UpdateJsonData()
    {
      CameraConfigInfo cameraConfig = null;

      try
      {
        cameraConfig = UpdateCameraConfigInfo();
      }
      catch (Exception ex)
      {
        return null;
      }
      return cameraConfig;
    }
    private CameraConfigInfo UpdateCameraConfigInfo()
    {
      string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
      int appsCameraConfigInfoId = _homeJsonEntity.CameraConfigInfo.FirstOrDefault().AppsCameraConfigInfoId;

      Home home = null;
      CameraConfigInfo cameraConfigInfo = null;


      home = new CommonService(_unitOfWorkAsync).GetHome(passPhrase);
      cameraConfigInfo = new CommonService(_unitOfWorkAsync).GetCameraConfigInfoByPassPhraseAndAppsCameraConfigInfoId(passPhrase, appsCameraConfigInfoId);

      if (cameraConfigInfo != null)
      {
        return UpdateCameraInfo(_homeJsonEntity.CameraConfigInfo[0], cameraConfigInfo);
      }
      return null;

    }

    public CameraConfigInfo UpdateCameraInfo(CameraConfigInfoEntity cameraConfigInfoEntity, CameraConfigInfo cameraConfigInfo)
    {

      CameraConfigInfo entity = SmartHomeTranslater.MapCameraConfigInfoProperties(cameraConfigInfoEntity, cameraConfigInfo);

      // entity.Parent = home;
      entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
      entity.ObjectState = ObjectState.Modified;
      _cameraConfigRepository.Update(entity);

      return entity;
    }
    private void SetMapper()
    {
      Mapper.CreateMap<CameraConfigInfoEntity, CameraConfigInfo>();
    }
  }


}
