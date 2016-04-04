﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class RoomEntity
    {
        #region Primitive Properties 
        public int Id { get; set; }
        [JsonProperty("Home")]
        public int HId { get; set; }

        public string Name { get; set; }

        public int RoomNumber { get; set; }

        public bool isActive { get; set; }
        public bool isSynced { get; set; }

        #endregion
    }
}