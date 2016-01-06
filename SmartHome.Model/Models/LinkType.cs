using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class LinkType
    {
        public int LinkTypeId { get; set; }
        public string Link { get; set; }
        public bool IsActive { get; set; }

        public AuditFields AuditField { get; set; }
        public virtual HomeLink HomeLink { get; set; }
        
    }
}
