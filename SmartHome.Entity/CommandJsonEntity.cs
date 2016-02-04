using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartHome.Model.Enums;
using CommandType = SmartHome.Model.Enums.CommandType;

namespace SmartHome.Entity
{
    public class CommandJsonEntity
    {
        [JsonProperty("CommandJsonId")]
        public int CommandJsonId { get; set; }
        public int CommandID { get; set; }
        public String Command { get; set; }
        public int DeviceID { get; set; }
        public int DeviceUUID { get; set; }
        public bool Response { get; set; }
        public string DeviceVersion { get; set; }
        public string MacID { get; set; }
        public string EmailAddress { get; set; }

        public bool IsProcessed { get; set; }
        public CommandType CommandType { get; set; }
        public string ProcessFailReason { get; set; }
    }
}
