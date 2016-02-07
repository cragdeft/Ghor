using Newtonsoft.Json;
using SmartHome.Model.Enums;
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
        //public int DeviceId { get; set; }        
        public int Id { get; set; }
        [JsonProperty("DeviceId")]
        public int DId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceHash { get; set; }
        [JsonProperty("Version")]
        public string DeviceVersion { get; set; }
        public bool IsDeleted { get; set; }
        public string Mac { get; set; }
        //[JsonProperty("DeviceType")]
       // public string DeviceType { get; set; }
        public DeviceType? DeviceType { get; set; }
        public int Watt { get; set; }
        #endregion

    }
}
