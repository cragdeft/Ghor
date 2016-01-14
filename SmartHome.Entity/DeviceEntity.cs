using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class DeviceEntity
    {
        #region Primitive Properties
        
        public int DeviceId { get; set; }
        [JsonProperty("ID")]
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string DeviceHash { get; set; }
        [JsonProperty("Version")]
        public string DeviceVersion { get; set; }
        public int IsDeleted { get; set; }
        public string Mac { get; set; }
        [JsonProperty("DeviceType")]
        public string DType { get; set; }
       // public DeviceType? DeviceType { get; set; }
        #endregion
    }
}
