using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    
    public class HomeJsonEntity
    {
        public List<VersionDetailEntity> VersionDetails { get; set; }
        public List<HomeEntity> Home { get; set; }
        public List<ChannelEntity> Channel { get; set; }
        public List<SmartDeviceEntity> Device { get; set; }
        public List<UserRoomLinkEntity> UserRoomLink { get; set; }
        public List<NextAssociatedDeviceEntity> NextAssociatedDeviceId { get; set; }
        public List<RouterInfoEntity> RouterInfo { get; set; }
        public List<WebBrokerInfoEntity> WebBrokerInfo { get; set; }
        public List<ChannelStatusEntity> ChannelStatus { get; set; }
        public List<RoomEntity> Room { get; set; }
        public int DatabaseVersion { get; set; }
        public List<UserHomeLinkEntity> UserHomeLink { get; set; }
        public List<UserInfoEntity> UserInfo { get; set; }
        public List<VersionEntity> Version { get; set; }
        public List<DeviceStatusEntity> DeviceStatus { get; set; }
        public List<RgbwStatusEntity> RgbwStatus { get; set; }
        public string Configuration { get; set; }
    }
}
