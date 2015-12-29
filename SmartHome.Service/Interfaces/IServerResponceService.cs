using Service.Pattern;
using SmartHome.Model.Models;
using System.Collections.Generic;

namespace SmartHome.Service.Interfaces
{


    public interface IServerResponceService : IService<ServerResponce>
    {
  
        IEnumerable<ServerResponce> ResponcesByMessageId(string MessageId);
  
    }

}
