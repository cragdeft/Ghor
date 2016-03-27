using SmartHome.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class RootObjectEntity
    {
        //public IEnumerable<VersionEntity> Version { get; set; }
        //public IEnumerable<VersionDetailEntity> VersionDetails { get; set; }
        //public IEnumerable<DeviceEntity> Device { get; set; }
        //public IEnumerable<DeviceStatusEntity> DeviceStatus { get; set; }
        //public IEnumerable<ChannelEntity> Channel { get; set; }
        //public int NextDeviceID { get; set; }


        public IEnumerable<VersionEntity> Version { get; set; }
        public IEnumerable<VersionDetailEntity> VersionDetails { get; set; }
        public IEnumerable<DeviceEntity> Device { get; set; }

        public IEnumerable<RgbwStatusEntity> RgbwStatus { get; set; }
        public IEnumerable<DeviceStatusEntity> DeviceStatus { get; set; }
        public IEnumerable<ChannelEntity> Channel { get; set; }
        public IEnumerable<ChannelStatusEntity> ChannelStatus { get; set; }
        public IEnumerable<NextAssociatedDeviceEntity> NextAssociatedDeviceId { get; set; }

        public IEnumerable<HomeEntity> Home { get; set; }

        public IEnumerable<RoomEntity> Room { get; set; }

    }
}
