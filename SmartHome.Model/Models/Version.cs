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
    public class Version : Entity
    {
        #region Primitive Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VersionId { get; set; }
        public int Id { get; set; }
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string AuthCode { get; set; }
        public string PassPhrase { get; set; }
        public string MAC { get; set; }
        #endregion

        #region Complex Properties
        public AuditFields AuditField { get; set; }
        #endregion

        #region Navigation Properties
        public virtual ICollection<VersionDetail> VersionDetails { get; set; }         
        #endregion
    }
}
