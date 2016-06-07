using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartHome.Entity
{
    public class UserHomeLinkEntity
    {
        public int UserHomeLinkEntityId { get; set; }
        [JsonProperty("Id")]
        public int AppsUserHomeLinkId { get; set; }
        [JsonProperty("Home")]
        public int AppsHomeId { get; set; }
        [JsonProperty("User")]
        public int AppsUserId { get; set; }
        public int IsAdmin { get; set; }
        public int IsSynced { get; set; }

    }
}
