using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class VersionDetailEntity
    {
        #region Primitive Properties
        public int VersionDetailId { get; set; }
        [JsonProperty("ID")]
        public int AppsVersionDetailId { get; set; }
        [JsonProperty("VersionId")]
        public int AppsVersionId { get; set; }
        public string HardwareVersion { get; set; }
        public int DeviceType { get; set; }
        [JsonProperty("IsSynced")]
        public int IsJsonSynced { get; set; }
        #endregion
    }
}
