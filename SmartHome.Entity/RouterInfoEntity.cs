using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class RouterInfoEntity
    {
        #region Primitive Properties 
        [JsonProperty("id")]
        public int AppsRouterInfoId { get; set; }
        [JsonProperty("Home")]
        public int AppsHomeId { get; set; }
        [JsonProperty("localBrokerIp")]
        public string LocalBrokerIp { get; set; }
        [JsonProperty("localBrokerPort")]
        public string LocalBrokerPort { get; set; }
        public string LocalBrokerUsername { get; set; }
        public string LocalBrokerPassword { get; set; }
        [JsonProperty("macAddress")]
        public string MacAddress { get; set; }
        public string Ssid { get; set; }
        public string SsidPassword { get; set; }
        public int IsSynced { get; set; }
        public virtual HomeEntity Parent { get; set; }
        #endregion
    }
}
