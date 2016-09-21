using SmartHome.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Data.Processor
{
    public class UserDatatProcessor : BaseDataProcessor
    {
        public UserDatatProcessor(string jsonString, MessageReceivedFrom receivedFrom)
        {
            _receivedFrom = receivedFrom;
            _homeJsonMessage = jsonString;
           // _homeJsonEntity = JsonDesrialized<HomeJsonEntity>(jsonString);
        }
    }
}
