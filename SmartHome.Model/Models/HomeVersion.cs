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
    public class HomeVersion :Entity
    {
        #region Primitive Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HomeVersionId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime LaunchDate { get; set; }
        #endregion

        #region Complex Properties
        public AuditFields AuditField { get; set; }
        #endregion

        #region Navigation Properties
        public virtual Home Home { get; set; } 
        #endregion
    }
}
