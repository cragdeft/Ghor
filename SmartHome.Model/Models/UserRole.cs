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
    public class UserRole : Entity
    {
        
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserRoleId { get; set; }
        public virtual WebPagesRole WebPagesRole { get; set; }
        public virtual UserInfo UserInfo { get; set; }       
       
    }
}
