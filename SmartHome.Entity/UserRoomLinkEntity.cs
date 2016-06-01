﻿using System;
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
        public int AppsUserRoomLinkId { get; set; }
        [JsonProperty("User")]
        public int AppsUserId { get; set; }
        [JsonProperty("Room")]
        public int AppsRoomId { get; set; }
        public bool IsSynced { get; set; }
    }
}
