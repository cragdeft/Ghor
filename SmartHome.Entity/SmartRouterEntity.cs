using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class SmartRouterEntity
    {
        #region Primitive Properties 
        public int DeviceId { get; set; }
        [JsonProperty("id")]
        public int AppsRouterId { get; set; }
        [JsonProperty("localBrokerIp")]
        public string LocalBrokerIp { get; set; }
        [JsonProperty("localBrokerPort")]
        public string LocalBrokerPort { get; set; }
        public string LocalBrokerUsername { get; set; }
        public string LocalBrokerPassword { get; set; }
        [JsonProperty("Home")]
        public int HomeId { get; set; }
        [JsonProperty("macAddress")]
        public string MacAddress { get; set; }
        public string Ssid { get; set; }
        public string SsidPassword { get; set; }
        public bool IsSynced { get; set; }

        public virtual HomeEntity Parent { get; set; }
        #endregion
    }
}
