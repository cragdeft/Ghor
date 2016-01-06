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
        public int Id { get; set; }
        public string VId { get; set; }
        public string HardwareVersion { get; set; }
        public AuditFields AuditField { get; set; }

        public virtual Version Version { get; set; }
    }
}
