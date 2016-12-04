using Repository.Pattern.Ef6;
using SmartHome.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
  public class MessageLog : Entity
  {
    #region Primitive Properties

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long MessageLogId { get; set; }
    //public string EncryptMessage { get; set; }
    public string Message { get; set; }    
    public MessageReceivedFrom ReceivedFrom { get; set; }
    public string UserInfoIds { get; set; }
    #endregion

    #region Complex Properties
    public AuditFields AuditField { get; set; }
    #endregion

    //public virtual ICollection<UserInfo> UserInfoes { get; set; }
  }
}
