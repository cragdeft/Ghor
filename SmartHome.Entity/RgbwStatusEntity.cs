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
        public int Id { get; set; }
        [JsonProperty("DeviceId")]
        public int DId { get; set; }
        [JsonProperty("StatusType")]        
        public int RGBColorStatusType { get; set; }
        public bool IsPowerOn { get; set; }
        public int ColorR { get; set; }
        public int ColorG { get; set; }
        public int ColorB { get; set; }
        public bool IsWhiteEnabled { get; set; }
        public int DimmingValue { get; set; }


        #endregion
    }
}
