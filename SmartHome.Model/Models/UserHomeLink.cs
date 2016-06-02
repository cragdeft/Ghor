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
    public class UserHomeLink: Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserHomeLinkId { get; set; }
        public int AppsHomeId { get; set; }
        public int AppsUserId { get; set; }
        public int AppsUserHomeLinkId { get; set; }
        public virtual Home Home { get; set; }
        public virtual UserInfo UserInfo { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSynced { get; set; }


       
    }
}
