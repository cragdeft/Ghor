using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class WebBrokerInfo : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long WebBrokerInfoId { get; set; }
        public int AppsWebBrokerInfoId { get; set; }
        public string BrokerIp { get; set; }
        public int BrokerPort { get; set; }
        public string BrokerUsername { get; set; }
        public string BrokerPassword { get; set; }
        public string SslCertificatePassword { get; set; }
        public bool IsSynced { get; set; }

        #region Complex Properties
        public AuditFields AuditField { get; set; }
        #endregion

        #region Navigation Properties
        public virtual Home Parent { get; set; }
        #endregion
    }
}
