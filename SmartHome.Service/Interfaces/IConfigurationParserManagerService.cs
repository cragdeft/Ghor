using SmartHome.Entity;
using SmartHome.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service.Interfaces
{
    public interface IConfigurationParserManagerService
    {
        IEnumerable<Version> AddOrUpdateVersionGraphRange(IEnumerable<Version> model);

        IEnumerable<Device> AddOrUpdateDeviceGraphRange(IEnumerable<Device> model);
       // IEnumerable<DeviceInfoEntity> GetsDeviceAllInfo();
    }
}
