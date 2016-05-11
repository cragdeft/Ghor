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

        public IEnumerable<UserInfoEntity> UserInfo { get; set; }
        public IEnumerable<UserHomeLinkEntity> UserHomeLink { get; set; }

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


    public class LoginRootObjectEntity
    {
        public LoginMessage MESSAGE { get; set; }
        public LoginObjectEntity data { get; set; }

    }


    public class LoginMessage
    {
        public int HTTP_STATUS { get; set; }
        public string HTTP_MESSAGE { get; set; }
    }


    public class LoginObjectEntity
    {


        public List<UserInfoEntity> UserInfo { get; set; }
        public List<UserHomeLinkEntity> UserHomeLink { get; set; }

        public List<UserRoomLinkEntity> UserRoomLink { get; set; }

        

        public List<HomeEntity> Home { get; set; }

        public List<RoomEntity> Room { get; set; }

        public List<DeviceEntity> Device { get; set; }

        public List<DeviceStatusEntity> DeviceStatus { get; set; }
        public List<ChannelEntity> Channel { get; set; }

        public List<ChannelStatusEntity> ChannelStatus { get; set; }


        public List<RgbwStatusEntity> RgbwStatus { get; set; }
        public List<RouterInfoEntity> RouterInfo { get; set; }

        




    }
}
