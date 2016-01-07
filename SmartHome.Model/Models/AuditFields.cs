using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    [ComplexType]
    public class AuditFields
    {
        #region Primitive Properties
        public string InsertedBy { get; set; }
        public DateTime InsertedDateTime { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime LastUpdatedDateTime { get; set; } 
        #endregion
    }
}
