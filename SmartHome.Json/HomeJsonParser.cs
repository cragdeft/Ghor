
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.ModelDataContext;
using SmartHome.Model.Models;
using SmartHome.Service;
using SmartHome.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Json
{
    public class HomeJsonParser : BaseJsonParser
    {
        public HomeJsonParser(string jsonString, MessageReceivedFrom receivedFrom)
        {
            _receivedFrom = receivedFrom;
            _homeJsonMessage = jsonString;
            _homeJsonEntity = JsonDesrialized<HomeJsonEntity>(jsonString);
        }

        public void Save()
        {
            if (_homeJsonEntity == null)
                return;

            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IHomeJsonParserService<Home> service = new HomeJsonParserService(unitOfWork, _homeJsonEntity, _homeJsonMessage, _receivedFrom);

                try
                {
                    service.SaveJsonData();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                }
            }
        }

        public Home SaveNewHome()
        {
            Home home = null;

            if (_homeJsonEntity == null)
                return home;

            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                var service = new HomeInfoJsonParserService(unitOfWork, _homeJsonEntity, _homeJsonMessage, _receivedFrom);
                var transactionRunner = new UnitOfWorkTransactionRunner(unitOfWork);

                MessageLog messageLog = transactionRunner.RunTransaction(() => new CommonService(unitOfWork).SaveMessageLog(_homeJsonMessage, _receivedFrom));

                try
                {
                    home = transactionRunner.RunTransaction(() => service.SaveJsonData());
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    transactionRunner.RunTransaction(() => new CommonService(unitOfWork).UpdateMessageLog(messageLog, _homeJsonEntity.Home[0].PassPhrase));
                }
            }
            return home;
        }
    }
}
