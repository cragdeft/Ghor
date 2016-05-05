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
    public class Room : Entity
    {
        #region Primitive Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomId { get; set; }
        public string Id { get; set; }

        public string HId { get; set; }

        public string Name { get; set; }
        public int RoomNumber { get; set; }
        public string Comment { get; set; }
        public bool IsMasterRoom { get; set; }
        public bool IsActive { get; set; }
        #endregion

        #region Complex Properties
        public AuditFields AuditField { get; set; }
        #endregion

        #region Navigation Properties
        
        public virtual ICollection<SmartDevice> SmartDevices { get; set; }        
        public virtual ICollection<UserRoom> UserRooms { get; set; }
        public virtual Home Home { get; set; }
        #endregion
    }
}
