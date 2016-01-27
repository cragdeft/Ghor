using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class NextAssociatedDevice:Entity
    {
        public int NextDeviceId { get; set; }
    }
}
