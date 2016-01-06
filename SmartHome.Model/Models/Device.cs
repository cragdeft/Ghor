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
    public class Device : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeviceId { get; set; }
        public int  Id { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string DeviceHash { get; set; }
        public string DeviceVersion { get; set; }
        public int IsDeleted { get; set; }
        public string Mac { get; set; }
        public AuditFields AuditField { get; set; }
        public virtual ICollection<DeviceStatus> DeviceStatus { get; set; }
        public virtual ICollection<ChannelConfig> ChannelConfigs { get; set; }
        public virtual Room Room { get; set; }

    }

}
