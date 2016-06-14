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
        [JsonProperty("Id")]
        public int AppsRouterInfoId { get; set; }
        [JsonProperty("Home")]
        public int AppsHomeId { get; set; }
        [JsonProperty("LocalBrokerIp")]
        public string LocalBrokerIp { get; set; }
        [JsonProperty("LocalBrokerPort")]
        public string LocalBrokerPort { get; set; }
        public string LocalBrokerUsername { get; set; }
        public string LocalBrokerPassword { get; set; }
        [JsonProperty("MacAddress")]
        public string MacAddress { get; set; }
        public string Ssid { get; set; }
        public string SsidPassword { get; set; }
        public int IsSynced { get; set; }
        public virtual HomeEntity Parent { get; set; }
        #endregion
    }
}
