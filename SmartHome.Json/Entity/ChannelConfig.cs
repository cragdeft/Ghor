using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Json.Entity
{
    public class ChannelConfig
    {
        public int ID { get; set; }
        public int DeviceID { get; set; }
        public int ChannelNo { get; set; }
        public int LoadType { get; set; }
        public string LoadName { get; set; }
        public int Status { get; set; }
        public int Value { get; set; }
    }
}
