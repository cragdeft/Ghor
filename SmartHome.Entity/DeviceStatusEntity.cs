using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class DeviceStatusEntity
    {
        #region Primitive Properties
        
        public int DeviceStatusId { get; set; }
        [JsonProperty("ID")]
        public int Id { get; set; }
        [JsonProperty("DeviceID")]
        public int DId { get; set; }
        public int StatusType { get; set; }
        public int Status { get; set; }
        #endregion
    }
}
