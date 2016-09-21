using Newtonsoft.Json;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Pattern.DataContext;
namespace SmartHome.Json
{
    public class BaseJsonParser
    {
        public HomeJsonEntity _homeJsonEntity { get; set; }
        public string _homeJsonMessage { get; set; }
        public IUnitOfWorkAsync _unitOfWorkAsync { get; set; }
        public MessageReceivedFrom _receivedFrom { get; set; }

        public static T JsonDesrialized<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
