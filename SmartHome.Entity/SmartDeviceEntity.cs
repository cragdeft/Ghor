using Newtonsoft.Json;
using SmartHome.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class SmartDeviceEntity
    {
        #region Primitive Properties        
        //public int DeviceId { get; set; }
        [JsonProperty("Id")]
        public int AppsDeviceId { get; set; }
        [JsonProperty("DeviceId")]
        public int AppsBleId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceHash { get; set; }
        [JsonProperty("Version")]
        public string FirmwareVersion { get; set; }
        public int IsDeleted { get; set; }
        public DeviceType? DeviceType { get; set; }
        public int Watt { get; set; }
        [JsonProperty("Room")]
        public int AppsRoomId { get; set; }
        //[JsonProperty("IsSynced")]
        public int IsSynced { get; set; }
        #endregion

    }
}
