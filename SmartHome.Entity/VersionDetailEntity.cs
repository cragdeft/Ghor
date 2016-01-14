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
        public int Id { get; set; }
        [JsonProperty("VersionID")]        
        public string VId { get; set; }
        public string HardwareVersion { get; set; }
        #endregion
    }
}
