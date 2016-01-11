using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Json.Entity
{
    public class RootObject
    {
        public List<Version> Version { get; set; }
        public List<VersionDetail> VersionDetails { get; set; }
        public List<Device> Device { get; set; }
        public List<DeviceStatus> DeviceStatus { get; set; }
        public List<ChannelConfig> ChannelConfig { get; set; }
        public int NextDeviceID { get; set; }
    }
}
