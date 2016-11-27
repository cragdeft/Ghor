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
    public HomeViewModel()
    {

      Homes = new List<Home>();
      Users = new List<UserInfo>();
      UserHomeLinks = new List<UserHomeLink>();
      Routers = new List<RouterInfo>();
      WebBrokerInfoes = new List<WebBrokerInfo>();
      Rooms = new List<Room>();
      UserRoomLinks = new List<UserRoomLink>();
      SmartDevices = new List<SmartDevice>();
      NextAssociatedDevice = new List<NextAssociatedDevice>();
      Channels = new List<Channel>();
      RgbwStatuses = new List<RgbwStatus>();
    }

    public IList<Home> Homes { get; set; }
    public IList<UserInfo> Users { get; set; }
    public IList<UserHomeLink> UserHomeLinks { get; set; }
    public IList<RouterInfo> Routers { get; set; }
    public IList<WebBrokerInfo> WebBrokerInfoes { get; set; }
    public IList<Room> Rooms { get; set; }
    public IList<UserRoomLink> UserRoomLinks { get; set; }
    public IList<SmartDevice> SmartDevices { get; set; }
    public IList<NextAssociatedDevice> NextAssociatedDevice { get; set; }
    public IList<Channel> Channels { get; set; }
    public IList<RgbwStatus> RgbwStatuses { get; set; }
  }
}
