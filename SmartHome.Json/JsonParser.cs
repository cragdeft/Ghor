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
    public class JsonParser
    {
        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        private IUnitOfWorkAsync _unitOfWorkAsync;
        private IHomeJsonParserService _homeJsonParserService;
        private MessageReceivedFrom _receivedFrom;

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
                    SaveNewDevice();
                    break;
                case ConfigurationType.NewChannel:
                    SaveNewChannel();
                    break;
                case ConfigurationType.NewUser:
                    SaveNewUser();
                    break;
                case ConfigurationType.EditDevice:
                    DeviceRoomUpdate();
                    break;
                case ConfigurationType.DeleteChannel:
                    DeleteChannel();
                    break;
                case ConfigurationType.DeleteDevice:
                    DeleteDevice();
                    break;
                case ConfigurationType.DeleteRoom:
                    DeleteRoom();
                    break;
                case ConfigurationType.NewRouterInfo:
                    SaveNewRouter();
                    break;
            }
        }

        //public void Save()
        //{
        //    if (_homeJsonEntity == null)
        //        return;

        //    using (IDataContextAsync context = new SmartHomeDataContext())
        //    using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
        //    {

        //        IHomeJsonParserService service = new HomeJsonParserService(unitOfWork, _homeJsonEntity, _homeJsonMessage, _receivedFrom);
        //        try
        //        {
        //            service.SaveJsonData();
        //        }
        //        catch (Exception ex)
        //        {
        //            unitOfWork.Rollback();
        //        }
        //    }

        //}
        //public bool SaveNewRoom()
        //{
        //    if (_homeJsonEntity == null)
        //        return false;

        //    using (IDataContextAsync context = new SmartHomeDataContext())
        //    using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
        //    {
        //        var service = new RoomJsonParserService(unitOfWork, _homeJsonEntity, _homeJsonMessage, _receivedFrom);
        //        var transactionRunner = new UnitOfWorkTransactionRunner(unitOfWork);

        //        MessageLog messageLog = transactionRunner.RunTransaction(() => new CommonService(unitOfWork).SaveMessageLog(_homeJsonMessage, _receivedFrom));

        //        try
        //        {
        //            transactionRunner.RunTransaction(() => service.SaveJsonData());
        //        }
        //        catch (Exception ex)
        //        {
        //            return false;
        //        }
        //        finally
        //        {
        //            transactionRunner.RunTransaction(() => new CommonService(unitOfWork).UpdateMessageLog(messageLog, _homeJsonEntity.Home[0].PassPhrase));
        //        }
        //    }
        //    return true;
        //}
        public bool DeleteRoom()
        {
            if (_homeJsonEntity == null)
                return false;

            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IHomeDeleteJsonParserService service = new RoomDeleteJsonParserService(unitOfWork, _homeJsonEntity, _homeJsonMessage, _receivedFrom);
                try
                {
                    service.DeleteJsonData();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return false;
                }
            }
            return true;

        }
        public bool DeviceRoomUpdate()
        {
            if (_homeJsonEntity == null)
                return false;

            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IHomeUpdateJsonParserService service = new DeviceRoomUpdateJsonParserService(unitOfWork, _homeJsonEntity, _homeJsonMessage, _receivedFrom);
                try
                {
                    service.UpdateJsonData();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return false;
                }
            }
            return true;

        }
        public bool SaveNewUser()
        {
            if (_homeJsonEntity == null)
                return false;

            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {

                IHomeJsonParserService service = new UserInfoJsonParserService(unitOfWork, _homeJsonEntity, _homeJsonMessage, _receivedFrom);
                try
                {
                    service.SaveJsonData();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return false;
                }
            }

            return true;

        }
        public bool SaveNewDevice()
        {
            if (_homeJsonEntity == null)
                return false;

            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IHomeJsonParserService service = new DeviceJsonParserService(unitOfWork, _homeJsonEntity, _homeJsonMessage, _receivedFrom);
                try
                {
                    service.SaveJsonData();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return false;
                }
            }
            return true;
        }
        public bool DeleteDevice()
        {
            if (_homeJsonEntity == null)
                return false;

            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IHomeDeleteJsonParserService service = new DeviceDeleteJsonParserService(unitOfWork, _homeJsonEntity, _homeJsonMessage, _receivedFrom);
                try
                {
                    service.DeleteJsonData();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return false;
                }
            }
            return true;
        }
        public bool SaveNewChannel()
        {
            if (_homeJsonEntity == null)
                return false;

            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IHomeJsonParserService service = new ChannelJsonParserService(unitOfWork, _homeJsonEntity, _homeJsonMessage, _receivedFrom);
                try
                {
                    service.SaveJsonData();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return false;
                }
            }
            return true;
        }
        public bool DeleteChannel()
        {
            if (_homeJsonEntity == null)
                return false;

            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IHomeDeleteJsonParserService service = new ChannelDeleteJsonParserService(unitOfWork, _homeJsonEntity, _homeJsonMessage, _receivedFrom);
                try
                {
                    service.DeleteJsonData();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return false;
                }
            }
            return true;
        }
        public bool SaveNewRouter()
        {
            if (_homeJsonEntity == null)
                return false;

            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IHomeJsonParserService service = new RouterInfoJsonParserService(unitOfWork, _homeJsonEntity, _homeJsonMessage, _receivedFrom);
                try
                {
                    service.SaveJsonData();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return false;
                }
            }
            return true;
        }

        public static T JsonDesrialized<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
