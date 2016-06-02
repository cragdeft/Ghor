using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class NextAssociatedDevice:Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NextAssociatedDeviceId { get; set; }
        
        public int NextDeviceId { get; set; }

        #region Navigation Properties
        public virtual Home Home { get; set; }
        #endregion
    }
}
