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
    public class Channel : Entity
    {
        #region Primitive Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ChannelId { get; set; }
        public int AppsChannelId { get; set; }
        public int AppsDeviceTableId { get; set; }
        public int ChannelNo { get; set; }
        public string LoadName { get; set; }
        public LoadType LoadType { get; set; }
        public int LoadWatt { get; set; }
        public bool IsSynced { get; set; }
        #endregion

        #region Complex Properties
        public AuditFields AuditField { get; set; }
        #endregion

        #region Navigation Properties    
        public virtual SmartSwitch SmartSwitch { get; set; }
        public virtual ICollection<ChannelStatus> ChannelStatuses { get; set; }
        #endregion

    }

}
