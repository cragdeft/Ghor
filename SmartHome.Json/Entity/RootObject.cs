using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Json.Entity
{
    public class RootObject
    {
        public IEnumerable<Version> Version { get; set; }
        public IEnumerable<VersionDetail> VersionDetails { get; set; }
        public IEnumerable<Device> Device { get; set; }
        public IEnumerable<DeviceStatus> DeviceStatus { get; set; }
        public IEnumerable<ChannelConfig> ChannelConfig { get; set; }
        public int NextDeviceID { get; set; }
    }
}
