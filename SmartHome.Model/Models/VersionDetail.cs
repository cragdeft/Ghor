using Repository.Pattern.Ef6;
using SmartHome.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class VersionDetail : Entity
    {
        #region Primitive Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VersionDetailId { get; set; }
        public int AppsVersionDetailId { get; set; }
        public int AppsVersionId { get; set; }
        public string HardwareVersion { get; set; }
        public string DType { get; set; }
        public DeviceType? DeviceType { get; set; }
        public bool IsSynced { get; set; }

        #endregion

        #region Complex Properties
        public AuditFields AuditField { get; set; }
        #endregion

        #region Navigation Properties
        public virtual Version Version { get; set; }
        #endregion
    }
}
