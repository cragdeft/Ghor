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
    public class Address:Entity
    {
        #region Primitive Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Zone { get; set; }
        public string Block { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        //public string OfficePhone { get; set; }
        //public string WorkPhone { get; set; }
        public string ZipCode { get; set; }
        #endregion

        #region Complex Properties
        public AuditFields AuditField { get; set; }
        #endregion

        #region Navigation Properties
        public virtual Home Home { get; set; }
        //public virtual UserProfile UserProfile { get; set; } 
        #endregion

    }
}
