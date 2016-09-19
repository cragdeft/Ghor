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
        [JsonProperty("DeviceId")]
        public int AppsBleId { get; set; }
        [JsonProperty("LocalBrokerIp")]
        public string LocalBrokerIp { get; set; }
        [JsonProperty("LocalBrokerPort")]
        public string LocalBrokerPort { get; set; }
        public string LocalBrokerUsername { get; set; }
        public string LocalBrokerPassword { get; set; }
        [JsonProperty("LocalBrokerExternalIp")]
        public string LocalBrokerExternalIp { get; set; }
        [JsonProperty("LocalBrokerExternalPort")]
        public string LocalBrokerExternalPort { get; set; }
        public int IsExternal { get; set; }
        public string Ssid { get; set; }
        public string SsidPassword { get; set; }
        [JsonProperty("MacAddress")]
        public string MacAddress { get; set; }
        [JsonProperty("HttpdUsername")]
        public string HttpdUsername { get; set; }
        [JsonProperty("HttpdPassword")]
        public string HttpdPassword { get; set; }
        [JsonProperty("HttpdIp")]
        public string HttpdIp { get; set; }
        [JsonProperty("HttpdPort")]
        public string HttpdPort { get; set; }
        [JsonProperty("Home")]
        public int AppsHomeId { get; set; }
        public int IsSynced { get; set; }
        public virtual HomeEntity Parent { get; set; }
        #endregion
    }
}
