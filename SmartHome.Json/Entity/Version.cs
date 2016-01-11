using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Json.Entity
{
    public class Version
    {
        public int ID { get; set; }
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string AuthCode { get; set; }
        public string PassPhrase { get; set; }
    }
}
