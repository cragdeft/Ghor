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
        public int AppsHomeId { get; set; }
        public int AppsUserId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSynced { get; set; }

    }
}
