using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class HomeLink
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HomeLinkId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }        
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public string VideoQuality { get; set; }
        public int ChannelCount { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public AuditFields AuditField { get; set; }
        public virtual Home Home { get; set; }
        public virtual ICollection<SyncStatus> SyncStatuses { get; set; }
        public virtual ICollection<LinkType> LinkTypes { get; set; }
    }
}
