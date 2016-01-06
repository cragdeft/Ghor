using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class UserStatus
    {
        public int UserStatusId { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public AuditFields AuditField { get; set; }

        public virtual UserProfile UserProfile { get; set; }
    }
}
