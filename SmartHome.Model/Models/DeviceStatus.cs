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
    public class DeviceStatus : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeviceStatusID { get; set; }
        public int DeviceID { get; set; }
        public int ID { get; set; }
        public int DID { get; set; }
        public int StatusType { get; set; }
        public int Status { get; set; }
        public AuditFields AuditField { get; set; }
        public virtual ICollection<Device> Devices { get; set; }
    }
}
