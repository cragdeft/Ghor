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
    public class UserRoomLink : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserRoomLinkId { get; set; }
<<<<<<< HEAD
        public int AppsRoomId { get; set; }       
=======
        
        public int AppsRoomId { get; set; }
>>>>>>> d10c3a7e114a29b790e7629ed9723e18dcc11261
        public int AppsUserId { get; set; }

        public int AppsUserRoomLinkId { get; set; }

        public virtual Room Room { get; set; }
        public virtual UserInfo UserInfo { get; set; }       
        public bool IsSynced { get; set; }



    }
}
