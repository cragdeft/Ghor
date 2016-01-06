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
    public class ChannelConfig : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChannelConfigId { get; set; }        
        public int DeviceId { get; set; }
        public int Id { get; set; }
        public int DId { get; set; }
        public int ChannelNo { get; set; }
        public int LoadType { get; set; }
        public string LoadName { get; set; }
        public int Status { get; set; }
        public int Value { get; set; }
        public AuditFields AuditField { get; set; }
        public virtual ICollection<Device> Devices { get; set; }
    }

}
