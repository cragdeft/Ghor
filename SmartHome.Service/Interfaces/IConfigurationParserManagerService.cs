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

        IEnumerable<UserInfo> AddOrUpdateUserInfoGraphRange(IEnumerable<UserInfo> model);
        //IEnumerable<Home> AddOrUpdateHomeGraphRange(IEnumerable<Home> model);
        IEnumerable<UserHomeLink> AddOrUpdateHomeGraphRange(IEnumerable<UserHomeLink> model);
        IEnumerable<UserRoomLink> AddOrUpdateRoomGraphRange(IEnumerable<UserRoomLink> model);
        IEnumerable<Version> AddOrUpdateVersionGraphRange(IEnumerable<Version> model);
       // IEnumerable<Device> AddOrUpdateDeviceGraphRange(IEnumerable<Device> model, IEnumerable<DeviceEntity> modelEntity);
        IEnumerable<SmartDevice> AddOrUpdateDeviceGraphRange(IEnumerable<SmartDevice> model, IEnumerable<SmartDeviceEntity> modelEntity);
        //IEnumerable<Device> AddOrUpdateDeviceGraphRange(IEnumerable<SmartSwitch> model, IEnumerable<DeviceEntity> modelEntity);
        IEnumerable<DeviceInfoEntity> GetsDeviceAllInfo();
        IEnumerable<VersionInfoEntity> GetsAllVersion();

        List<UserHomeLink> GetsHomesAllInfo(int userInfoId,bool IsAdmin);
        List<Version> GetsAppVersionAllInfo();
    }
}
