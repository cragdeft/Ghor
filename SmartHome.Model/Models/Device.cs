using Repository.Pattern.Ef6;
using SmartHome.Model.SmartHome.Enums;
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
        public Device()
        {
            this.SMART_SWITCH_6G = DeviceType.SMART_SWITCH_6G;
            this.SMART_RAINBOW_12 = DeviceType.SMART_RAINBOW_12;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeviceId { get; set; }
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string DeviceHash { get; set; }
        public string DeviceVersion { get; set; }
        public int IsDeleted { get; set; }
        public string Mac { get; set; }
        public AuditFields AuditField { get; set; }
        public DeviceType DeviceType { get; set; }
        public DeviceType SMART_SWITCH_6G { get; set; }
        public DeviceType SMART_RAINBOW_12 { get; set; }
        public virtual ICollection<DeviceStatus> DeviceStatus { get; set; }
        public virtual ICollection<Channel> Channels { get; set; }
        public virtual Room Room { get; set; }

    }

}
