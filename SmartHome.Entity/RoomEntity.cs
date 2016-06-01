using Newtonsoft.Json;
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
        public int RoomId { get; set; }
    
        public int AppsRoomId { get; set; }
        [JsonProperty("Home")]
        public int AppsHomeId { get; set; }

        public string Name { get; set; }

        public int RoomNumber { get; set; }

        public bool IsActive { get; set; }
        public bool IsSynced { get; set; }

        #endregion
    }
}
