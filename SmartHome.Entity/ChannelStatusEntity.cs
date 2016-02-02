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
        public int CId { get; set; }
        //public int DeviceId { get; set; }
        public int Id { get; set; }
        public int ChannelNo { get; set; }
        [JsonProperty("StatusType")]
        public int Status { get; set; }
        [JsonProperty("StatusValue")]
        public string Value { get; set; }
    }
}
