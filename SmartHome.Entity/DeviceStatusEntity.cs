using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class DeviceStatusEntity
    {
        #region Primitive Properties      
        [JsonProperty("Id")]
        public int AppsDeviceStatusId { get; set; }
        [JsonProperty("DeviceTableId")]
        public string AppsDeviceId { get; set; }
        public int StatusType { get; set; }
        [JsonProperty("StatusValue")]
        public string Value { get; set; }
        [JsonProperty("IsSynced")]
        public int IsJsonSynced { get; set; }
        #endregion
    }
}
