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
        
        public int ChannelId { get; set; }
        [JsonProperty("ID")]
        public int Id { get; set; }
        [JsonProperty("DeviceID")]        
        public int DId { get; set; }
        public int ChannelNo { get; set; }
        public string LoadName { get; set; }
        public int Status { get; set; }
        public int Value { get; set; }
        public int LoadType { get; set; }
        #endregion
    }
}
