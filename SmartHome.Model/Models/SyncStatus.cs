using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class SyncStatus : Entity
    {
        #region Primitive Properties
        public long SyncStatusId { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        #endregion

        #region Complex Properties
        public AuditFields AuditField { get; set; }
        #endregion

        #region Navigation Properties
        public virtual Room Room { get; set; }
        public virtual UserInfo UserProfile { get; set; } 
        #endregion
    }
}
