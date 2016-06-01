using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class ChannelEntity
    {
        #region Primitive Properties
        [JsonProperty("Id")]
        public int AppsChannelId { get; set; }
        [JsonProperty("DeviceTableId")]        
        public int AppsDeviceTableId { get; set; }
        public int ChannelNo { get; set; }
        public string LoadName { get; set; }
        public int LoadType { get; set; }
        public int LoadWatt { get; set; }
        public bool IsSynced { get; set; }
        #endregion
    }
}
