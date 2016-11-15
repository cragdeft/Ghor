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
  public class CameraConfigInfo : Entity
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long CameraConfigInfoId { get; set; }
    public int AppsCameraConfigInfoId { get; set; }
    public string CameraIp { get; set; }
    public int CameraPort { get; set; }
    public string CameraUsername { get; set; }
    public string CameraPassword { get; set; }
    public bool IsSynced { get; set; }

    #region Complex Properties
    public AuditFields AuditField { get; set; }
    #endregion

    #region Navigation Properties
    public virtual Home Parent { get; set; }
    #endregion
  }
}
