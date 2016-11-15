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

  public class CameraConfigJsonParserService : IHomeJsonParserService<CameraConfigInfo>
  {
    #region PrivateProperty
    private readonly IUnitOfWorkAsync _unitOfWorkAsync;
    private readonly IRepositoryAsync<Home> _homeRepository;
    private readonly IRepositoryAsync<CameraConfigInfo> _cameraConfigRepository;

    public HomeJsonEntity _homeJsonEntity { get; private set; }
    public string _homeJsonMessage { get; private set; }
    public MessageReceivedFrom _receivedFrom { get; private set; }

    #endregion


    public CameraConfigJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
    {
      _unitOfWorkAsync = unitOfWorkAsync;
      _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();
      _cameraConfigRepository = _unitOfWorkAsync.RepositoryAsync<CameraConfigInfo>();

      _homeJsonEntity = homeJsonEntity;
      _homeJsonMessage = homeJsonMessage;
      _receivedFrom = receivedFrom;
    }

    public CameraConfigInfo SaveJsonData()
    {
      CameraConfigInfo cameraConfig = null;
      try
      {
        if (_homeJsonEntity.CameraConfigInfo.Count == 0)
        {
          return cameraConfig;
        }

        string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
        int appsCameraConfigInfoId = _homeJsonEntity.CameraConfigInfo.FirstOrDefault().AppsCameraConfigInfoId;

        //SmartSwitch sSwitch = null;


        cameraConfig = new CommonService(_unitOfWorkAsync).GetCameraConfigInfoByPassPhraseAndAppsCameraConfigInfoId(passPhrase, appsCameraConfigInfoId);

        if (cameraConfig != null)
        {
          IHomeUpdateJsonParserService<CameraConfigInfo> updateService = new CameraConfigUpdateJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.UpdateCameraConfigInfo);
          cameraConfig = updateService.UpdateJsonData();
        }
        else
        {
          IHomeJsonParserService<CameraConfigInfo> service = new CameraConfigNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.NewCameraConfigInfo);
          cameraConfig = service.SaveJsonData();
        }


      }
      catch (Exception ex)
      {
        return null;
      }
      return cameraConfig;
    }


  }
}
