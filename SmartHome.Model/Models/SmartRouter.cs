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
    public class SmartRouter: SmartDevice
    {
        #region Primitive Properties
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int SmartRouterId { get; set; }
        //public int Id { get; set; }
        
        public int AppsRouterId { get; set; }
        public string LocalBrokerIp { get; set; }
        public string LocalBrokerPort { get; set; }
        public string MacAddress { get; set; }
        public string Ssid { get; set; }
        public string SsidPassword { get; set; }
        public string LocalBrokerUsername { get; set; }
        public string LocalBrokerPassword { get; set; }
        public bool IsSynced { get; set; }
        #endregion

        #region Complex Properties
        //public AuditFields AuditField { get; set; }
        #endregion

        #region Navigation Properties
        public virtual Home Parent { get; set; }         
        #endregion
    }
}
