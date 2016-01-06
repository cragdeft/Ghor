using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class SmartRouter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SmartRouterId { get; set; }
        public string BrokerIp { get; set; }
        public string BrokerMac { get; set; }        
        public string BrokerAssignPort { get; set; }
        public string BrokerUserName { get; set; }
        public string BrokerPassword { get; set; }
        public AuditFields AuditField { get; set; }
        public bool IsActive { get; set; }
        public virtual Home Home { get; set; }        
    }
}
