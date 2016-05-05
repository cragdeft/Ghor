using Service.Pattern;
using SmartHome.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Entity;

namespace SmartHome.Service.Interfaces
{
    public interface IDeviceService : IService<SmartDevice>
    {
        IEnumerable<SmartDevice> AddOrUpdateGraphRange(IEnumerable<SmartDevice> model);
        IEnumerable<DeviceInfoEntity> GetsDeviceAllInfo();
    }
}
