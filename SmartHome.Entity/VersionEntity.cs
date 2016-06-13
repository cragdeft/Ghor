﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{

    public class VersionEntity
    {
        #region Primitive Properties
        public int VersionId { get; set; }
        [JsonProperty("Id")]
        public int AppsVersionId { get; set; }
        public string AppName { get; set; }
        //[JsonProperty("Version")]
        public string AppVersion { get; set; }
        public string AuthCode { get; set; }
        public string PassPhrase { get; set; }
        public string Mac { get; set; }
        [JsonProperty("IsSynced")]
        public int IsJsonSynced { get; set; }
        #endregion

        public virtual ICollection<VersionDetailEntity> VersionDetails { get; set; }
    }
}
