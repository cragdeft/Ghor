using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class RgbwStatusEntity
    {
        #region Primitive Properties  
        [JsonProperty("Id")]
        public int AppsRgbtStatusId { get; set; }
        [JsonProperty("DeviceTableId")]
        public int AppsDeviceId { get; set; }
        [JsonProperty("StatusType")]        
        public int RGBColorStatusType { get; set; }
        public bool IsPowerOn { get; set; }
        public int ColorR { get; set; }
        public int ColorG { get; set; }
        public int ColorB { get; set; }
        public bool IsWhiteEnabled { get; set; }
        public int DimmingValue { get; set; }
        public bool IsSynced { get; set; }

        #endregion
    }
}
