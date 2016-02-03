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
    public interface IDeviceService : IService<Device>
    {
        IEnumerable<Device> AddOrUpdateGraphRange(IEnumerable<Device> model);
        IEnumerable<DeviceInfoEntity> GetsDeviceAllInfo();

        

    }
}
