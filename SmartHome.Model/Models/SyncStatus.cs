using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class SyncStatus
    {
        public int SyncStatusId { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public AuditFields AuditField { get; set; }
        public virtual HomeLink HomeLink { get; set; }
        public virtual Room Room { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
