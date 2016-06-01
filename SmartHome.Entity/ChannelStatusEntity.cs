using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class ChannelStatusEntity
    {
        [JsonProperty("ChannelTableId")]
        public int AppsChannelId { get; set; }
        //public int DeviceId { get; set; }
        public int AppsChannelStatusId { get; set; }
        public int ChannelNo { get; set; }
        public int StatusType { get; set; }
        public int StatusValue { get; set; }
    }
}
