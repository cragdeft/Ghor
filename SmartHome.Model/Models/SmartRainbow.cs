using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class SmartRainbow : SmartDevice
    {
        #region  Navigation Properties

        //public string TestName { get; set; }

        public virtual ICollection<RgbwStatus> RgbwStatuses { get; set; }
        #endregion
    }
}
