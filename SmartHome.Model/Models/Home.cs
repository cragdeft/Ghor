using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class Home
    {
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
        public AuditFields AuditField { get; set; }
        public string Comment { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<HomeVersion> HomeVersions { get; set; }
        public virtual ICollection<HomeStatus> HomeStatuses { get; set; }
        public virtual ICollection<CommType> CommTypes { get; set; }
        public virtual ICollection<HomeLink> HomeLinks { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
