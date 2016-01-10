using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{

    public class VersionEntity 
    {
      

        #region Primitive Properties
        
        public int VersionId { get; set; }
        public int Id { get; set; }
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string AuthCode { get; set; }
        public string PassPhrase { get; set; }
        public string MAC { get; set; }
        #endregion

        #region Complex Properties
        //public AuditFields AuditField { get; set; }
        #endregion

    }
}
