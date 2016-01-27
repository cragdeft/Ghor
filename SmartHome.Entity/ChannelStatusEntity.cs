using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class ChannelStatusEntity
    {
        public int DeviceId { get; set; }
        public int ChannelNo { get; set; }
        public int StatusType { get; set; }
        public string StatusValue { get; set; }
    }
}
