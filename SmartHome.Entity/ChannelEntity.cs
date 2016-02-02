using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class ChannelEntity
    {
        #region Primitive Properties
        
        
        //[JsonProperty("ChannelNo")]
        public int Id { get; set; }
        [JsonProperty("DeviceTableId")]        
        public int DId { get; set; }
        public int ChannelNo { get; set; }
        public string LoadName { get; set; }
        //public int Status { get; set; }
        //public int Value { get; set; }
        public int LoadType { get; set; }

        public int LoadWatt { get; set; }
        #endregion
        
    }
}
