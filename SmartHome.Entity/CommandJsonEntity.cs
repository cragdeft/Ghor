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
        [JsonProperty("command_byte")]
        public String Command { get; set; }
        [JsonProperty("device_id")]
        public int DeviceId { get; set; }
        [JsonProperty("device_uuid")]
        public int DeviceUUId { get; set; }
        [JsonProperty("response")]
        public bool Response { get; set; }
        [JsonProperty("mac_id")]
        public string Mac { get; set; }
        public string DeviceVersion { get; set; }
        [JsonProperty("email")]
        public string EmailAddress { get; set; }

        public bool IsProcessed { get; set; }
        public CommandType CommandType { get; set; }
        public string ProcessFailReason { get; set; }
        public int CommandId { get; set; }
    }
}
