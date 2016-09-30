using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.ModelDataContext;
using SmartHome.Service;
using SmartHome.Utility;
using SmartHome.Service.Interfaces;
using SmartHome.Model.Models;

namespace SmartHome.Json
{
    public class JsonParser : BaseJsonParser
    {
        //public HomeJsonEntity _homeJsonEntity { get; private set; }
        //public string _homeJsonMessage { get; private set; }
        //private IUnitOfWorkAsync _unitOfWorkAsync;
        //private IHomeJsonParserService _homeJsonParserService;
        //private MessageReceivedFrom _receivedFrom;

        public JsonParser(string jsonString, MessageReceivedFrom receivedFrom)
        {
            _receivedFrom = receivedFrom;
            _homeJsonMessage = jsonString;
            _homeJsonEntity = JsonDesrialized<HomeJsonEntity>(jsonString);
            InitializeParameters();
        }

        private void InitializeParameters()
        {
            //IDataContextAsync context = new SmartHomeDataContext();
            //_unitOfWorkAsync = new UnitOfWork(context);
            //_homeJsonParserService = new HomeJsonParserService(_unitOfWorkAsync,_homeJsonEntity, _homeJsonMessage,_receivedFrom);
        }

        public void ProcessJsonData()
        {
            switch ((ConfigurationType)Enum.Parse(typeof(ConfigurationType), _homeJsonEntity.Configuration))
            {
                case ConfigurationType.All:
                    new HomeJsonParser(_homeJsonMessage, _receivedFrom).Save();
                    break;
                case ConfigurationType.NewRoom:
                    new RoomJsonParser(_homeJsonMessage, _receivedFrom).SaveNewRoom();
                    break;
                case ConfigurationType.NewDevice:
                    new DeviceJsonParser(_homeJsonMessage, _receivedFrom).SaveNewDevice();
                    break;
                case ConfigurationType.NewChannel:
                    new ChannelJsonParser(_homeJsonMessage, _receivedFrom).SaveNewChannel();
                    break;
                case ConfigurationType.NewUser:
                    new UserJsonParser(_homeJsonMessage, _receivedFrom).SaveNewUser();
                    break;
                case ConfigurationType.EditDevice:
                    new DeviceRoomJsonParser(_homeJsonMessage, _receivedFrom).DeviceRoomUpdate();
                    break;
                case ConfigurationType.DeleteChannel:
                    new ChannelDeleteJsonParser(_homeJsonMessage, _receivedFrom).DeleteChannel();
                    break;
                case ConfigurationType.DeleteDevice:
                    new DeviceDeleteJsonParser(_homeJsonMessage, _receivedFrom).DeleteDevice();
                    break;
                case ConfigurationType.DeleteRoom:
                    new RoomDeleteJsonParser(_homeJsonMessage, _receivedFrom).DeleteRoom();
                    break;
                case ConfigurationType.DeleteUser:
                    new UserDeleteJsonParser(_homeJsonMessage, _receivedFrom).DeleteUser();
                    break;
                case ConfigurationType.NewRouter:
                    new RouterJsonParser(_homeJsonMessage, _receivedFrom).SaveNewRouter();
                    break;
            }
        }
    }
}
