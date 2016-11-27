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
  public class RouterInfoJsonParserService : IHomeJsonParserService<RouterInfo>
  {
    #region PrivateProperty
    private readonly IUnitOfWorkAsync _unitOfWorkAsync;
    private readonly IRepositoryAsync<Home> _homeRepository;
    private readonly IRepositoryAsync<RouterInfo> _routerInfoRepository;
    private readonly IRepositoryAsync<WebBrokerInfo> _webBrokerInfoRepository;
    private readonly IRepositoryAsync<UserHomeLink> _userHomeRepository;

    public HomeJsonEntity _homeJsonEntity { get; private set; }
    public string _homeJsonMessage { get; private set; }
    public MessageReceivedFrom _receivedFrom { get; private set; }
    public MessageLog _messageLog { get; private set; }

    #endregion
    public RouterInfoJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
    {
      _unitOfWorkAsync = unitOfWorkAsync;
      _routerInfoRepository = _unitOfWorkAsync.RepositoryAsync<RouterInfo>();
      _userHomeRepository = _unitOfWorkAsync.RepositoryAsync<UserHomeLink>();
      _webBrokerInfoRepository = _unitOfWorkAsync.RepositoryAsync<WebBrokerInfo>();

      _homeJsonEntity = homeJsonEntity;
      _homeJsonMessage = homeJsonMessage;
      _receivedFrom = receivedFrom;
      _messageLog = new MessageLog();
    }


    public RouterInfo SaveJsonData()
    {
      RouterInfo routerInfo = null;
      try
      {
        string macAddress = _homeJsonEntity.RouterInfo.FirstOrDefault().MacAddress;
        string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;
        RouterInfo dbRouterInfo = null;
        Home home = null;

        home = new CommonService(_unitOfWorkAsync).GetHome(passPhrase);

        dbRouterInfo = new CommonService(_unitOfWorkAsync).GetRouterInfoByMacAddressAndHomeId(macAddress, home.HomeId);


        if (_homeJsonEntity.RouterInfo.Count == 0 && dbRouterInfo != null)
        {

          dbRouterInfo.ObjectState = ObjectState.Deleted;
          _routerInfoRepository.Delete(dbRouterInfo);
          return routerInfo;
        }


        if (dbRouterInfo != null)
        {
          IHomeUpdateJsonParserService<RouterInfo> updateService = new RouterInfoUpdateJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.UpdateRouter);
          routerInfo = updateService.UpdateJsonData();
        }
        else
        {
          IHomeJsonParserService<RouterInfo> service = new RouterInfoNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.NewRouter);
          routerInfo = service.SaveJsonData();
        }


      }
      catch (Exception ex)
      {
        return null;
      }
      return routerInfo;
    }


  }
}
