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
    public class DeviceRoomJsonParser : BaseJsonParser
    {
        public DeviceRoomJsonParser(string jsonString, MessageReceivedFrom receivedFrom)
        {
            _receivedFrom = receivedFrom;
            _homeJsonMessage = jsonString;
            _homeJsonEntity = JsonDesrialized<HomeJsonEntity>(jsonString);
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
    }
}
