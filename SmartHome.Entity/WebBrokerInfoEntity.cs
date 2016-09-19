using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class WebBrokerInfoEntity
    {
        [JsonProperty("Id")]
        public int AppsWebBrokerInfoId { get; set; }
        public string BrokerIp { get; set; }
        public int BrokerPort { get; set; }
        public string BrokerUsername { get; set; }
        public string BrokerPassword { get; set; }
        public string SslCertificatePassword { get; set; }
        public bool IsSynced { get; set; }
    }
}
