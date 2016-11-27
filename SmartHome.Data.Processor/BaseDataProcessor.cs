using Newtonsoft.Json;
using Repository.Pattern.UnitOfWork;
using SmartHome.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Data.Processor
{
  public class BaseDataProcessor
  {
    public string _homeJsonMessage { get; set; }
    public IUnitOfWorkAsync _unitOfWorkAsync { get; set; }
    public MessageReceivedFrom _receivedFrom { get; set; }

    public static T JsonDesrialized<T>(string jsonString)
    {
      return JsonConvert.DeserializeObject<T>(jsonString);
    }
  }
}
