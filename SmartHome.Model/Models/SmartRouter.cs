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
        #region Primitive Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SmartRouterId { get; set; }
        public string IP { get; set; }
        public string MacAddress { get; set; }
        public string Port { get; set; }
        public string RouterUserName { get; set; }
        public string RouterPassword { get; set; }
        public bool IsActive { get; set; }

        public bool IsDefault { get; set; }
        #endregion

        #region Complex Properties
        public AuditFields AuditField { get; set; }
        #endregion

        #region Navigation Properties
        public virtual Home Home { get; set; }         
        #endregion
    }
}
