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
    public class VersionDetail : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VersionDetailId { get; set; }
        public int VersionId { get; set; }
        public int ID { get; set; }
        public string VID { get; set; }
        public string HardwareVersion { get; set; }

        public AuditFields AuditField { get; set; }
        public virtual ICollection<Version> Versions { get; set; }
    }
}
