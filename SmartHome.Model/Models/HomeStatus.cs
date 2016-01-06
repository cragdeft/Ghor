using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class HomeStatus:Entity
    {
        public int HomeStatusId { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public AuditFields AuditField { get; set; }
        public virtual Home Home { get; set; }
    }
}
