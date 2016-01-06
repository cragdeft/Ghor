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
        public int DeviceStatusId { get; set; }        
        public int Id { get; set; }
        public int DId { get; set; }
        public int StatusType { get; set; }
        public int Status { get; set; }
        public AuditFields AuditField { get; set; }
        public virtual Device Device { get; set; }
       
    }
}
