using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class SmartSwitch : SmartDevice
    {
        #region  Navigation Properties      
        public virtual ICollection<Channel> Channels { get; set; }      
        #endregion
    }
}