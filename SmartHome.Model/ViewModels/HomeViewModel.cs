using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Model.Models;

namespace SmartHome.Model.ViewModels
{
    public class HomeViewModel
    {
        public IList<Home> Homes { get; set; }
        public IList<UserInfo> Users { get; set; }
        public IList<UserHomeLink> UserHomeLinks { get; set; } 
        public IList<RouterInfo> Routers { get; set; }
        public IList<Room> Rooms { get; set; }
        public IList<UserRoomLink> UserRoomLinks { get; set; } 
        public IList<SmartDevice> SmartDevices { get; set; }
        public IList<Channel> Channels { get; set; }
        public IList<RgbwStatus> RgbwStatuses { get; set; }     
    }
}
