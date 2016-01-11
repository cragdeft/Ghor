using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Json.Entity
{
    public class Device
    {
        public int ID { get; set; }
        public int DeviceHash { get; set; }
        public string DeviceType { get; set; }
        public string DeviceName { get; set; }
        public string Version { get; set; }
        public int IsDeleted { get; set; }
    }
}
