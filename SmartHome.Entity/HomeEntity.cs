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
            SmartRouter = new List<SmartRouterEntity>();
        }
        #region Primitive Properties 
        public int HomeId { get; set; }
        [JsonProperty("id")]
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

        public bool IsInternet { get; set; }

        public bool IsDefault { get; set; }

        public bool IsActive { get; set; }

        public bool IsSynced { get; set; }

        public List<SmartRouterEntity> SmartRouter { get; set; }

        #endregion
    }
}
