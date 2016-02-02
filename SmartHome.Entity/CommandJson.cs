using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class CommandJson
    {
        public int CommandID { get; set; }
        public String Command { get; set; }
        public int DeviceID { get; set; }
        public int DeviceUUID { get; set; }
        public bool Response { get; set; }
        public string DeviceVersion { get; set; }
        public string MacID { get; set; }
        public string EmailAddress { get; set; }
    }
}
