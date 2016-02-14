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

       
        //[JsonProperty("ID")]
        public int Id { get; set; }
        [JsonProperty("DeviceTableId")]
        public string DId { get; set; }
        public int StatusType { get; set; }
        [JsonProperty("StatusValue")]
        public string Value { get; set; }
        #endregion
    }
}
