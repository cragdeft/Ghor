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
    public class SmartDevice : Entity
    {
        #region Primitive Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeviceId { get; set; }
        public int AppsDeviceId { get; set; }
        public int AppsRoomId { get; set; }
        public int AppsBleId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceHash { get; set; }
        public string DeviceVersion { get; set; }
        public bool IsDeleted { get; set; }
        public string Watt { get; set; }
        public bool IsSynced { get; set; }
        public DeviceType DeviceType { get; set; }
        #endregion

        #region  Complex Properties
        public AuditFields AuditField { get; set; }
        #endregion

        #region  Navigation Properties
        public virtual ICollection<DeviceStatus> DeviceStatus { get; set; }
        public virtual Room Room { get; set; }
        #endregion
    }
}