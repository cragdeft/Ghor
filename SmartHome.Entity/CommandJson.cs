using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class CommandJson
    {
        public String Command { get; set; }
        public string DeviceID { get; set; }
        public string DeviceUUID { get; set; }
        public string Response { get; set; }
        public string DeviceVersion { get; set; }
        public string MacID { get; set; }
        public string EmailAddress { get; set; }
    }
}
