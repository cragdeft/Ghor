using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Json.Entity
{
    public class DeviceStatus
    {
        public int ID { get; set; }
        public int DeviceID { get; set; }
        public int StatusType { get; set; }
        public int Status { get; set; }
    }
}
