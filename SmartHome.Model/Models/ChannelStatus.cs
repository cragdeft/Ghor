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
    public class ChannelStatus : Entity
    {
        #region  Primitive Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ChannelStatusId { get; set; }
        public int AppsChannelStatusId { get; set; }
        public int AppsChannelId { get; set; }
        public ChannelStatusType StatusType { get; set; }
        public int StatusValue { get; set; }
        public bool IsSynced { get; set; }
        #endregion

        #region Complex Properties
        public AuditFields AuditField { get; set; }
        #endregion

        #region  Navigation Properties
        public virtual Channel Channel { get; set; }
        #endregion
    }
}
