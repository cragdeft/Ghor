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
    public class RgbwStatus : Entity
    {
        #region  Primitive Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RgbwStatusId { get; set; }
        public int AppsRgbtStatusId { get; set; }
        public int AppsDeviceId { get; set; }
        public RGBColorStatusType RGBColorStatusType { get; set; }
        public bool IsPowerOn { get; set; }
        public int ColorR { get; set; }
        public int ColorG { get; set; }
        public int ColorB { get; set; }
        public bool IsWhiteEnabled { get; set; }
        public int DimmingValue { get; set; }
        public bool IsSynced { get; set; }

        #endregion

        #region Complex Properties
        public AuditFields AuditField { get; set; }
        #endregion

        #region  Navigation Properties
        public virtual SmartRainbow SmartRainbow { get; set; }
        #endregion

    }
}
