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

        public int Id { get; set; }
        public string LocalBrokerUsername { get; set; }
        public string LocalBrokerPassword { get; set; }
        [JsonProperty("Home")]
        public int HId { get; set; }
        [JsonProperty("macAddress")]
        public string MacAddress { get; set; }
        public string Ssid { get; set; }
        public bool IsSynced { get; set; }        
        #endregion
    }
}
