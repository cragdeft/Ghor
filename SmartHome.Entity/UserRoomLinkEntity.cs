using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartHome.Entity
{
    public class UserRoomLinkEntity
    {
        public int UserRoomLinkEntityId { get; set; }
        [JsonProperty("Id")]
        public int AppsUserRoomLinkId { get; set; }
        [JsonProperty("User")]
        public int AppsUserId { get; set; }
        [JsonProperty("Room")]
        public int AppsRoomId { get; set; }
        //[JsonProperty("IsSynced")]
        public int IsSynced { get; set; }
    }
}
