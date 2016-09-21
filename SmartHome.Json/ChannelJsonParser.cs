using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.ModelDataContext;
using SmartHome.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Json
{
    public class ChannelJsonParser : BaseJsonParser
    {
        public ChannelJsonParser(string jsonString, MessageReceivedFrom receivedFrom)
        {
            _receivedFrom = receivedFrom;
            _homeJsonMessage = jsonString;
            _homeJsonEntity = JsonDesrialized<HomeJsonEntity>(jsonString);
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
    }
}
