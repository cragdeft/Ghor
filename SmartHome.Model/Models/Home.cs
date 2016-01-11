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
    public class Home:Entity
    {
        #region Primitive Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HomeId { get; set; }
        public string Name { get; set; }
        public string Zone { get; set; }
        public string Block { get; set; }
        public string TimeZone { get; set; }
        public string RegistrationKey { get; set; }
        public string HardwareId { get; set; }
        public int TrialCount { get; set; }        
        public string Comment { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        #endregion

        #region Complex Properties
        public AuditFields AuditField { get; set; }

        #endregion

        #region Navigation Properties
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<HomeVersion> HomeVersions { get; set; }        
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<SmartRouter> SmartRouters { get; set; } 
        #endregion
    }
}
