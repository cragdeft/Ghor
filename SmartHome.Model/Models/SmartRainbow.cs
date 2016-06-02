using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class SmartRainbow : SmartDevice
    {
        #region  Navigation Properties
        public virtual ICollection<RgbwStatus> RgbwStatuses { get; set; }
        #endregion
    }
}
