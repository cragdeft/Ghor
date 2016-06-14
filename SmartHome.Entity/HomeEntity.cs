using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartHome.Entity
{
    public class HomeEntity
    {
        public HomeEntity()
        {
            SmartRouter = new List<RouterInfoEntity>();
        }
        #region Primitive Properties 
        public int HomeId { get; set; }
        [JsonProperty("Id")]
        public int AppsHomeId { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Block { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string TimeZone { get; set; }
        public string Phone { get; set; }
        public string PassPhrase { get; set; }
        public int MeshMode { get; set; }
        public string Zone { get; set; }
        public int IsInternet { get; set; }
        public int IsDefault { get; set; }
        public int IsActive { get; set; }
        //[JsonProperty("IsSynced")]
        public int IsSynced { get; set; }
        public List<RouterInfoEntity> SmartRouter { get; set; }

        #endregion
    }
}
