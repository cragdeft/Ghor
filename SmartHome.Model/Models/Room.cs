using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class Room : Entity
    {
        #region Primitive Properties
        public int RoomId { get; set; }
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
        public virtual ICollection<SyncStatus> SyncStatuses { get; set; }
        public virtual ICollection<Device> Devices { get; set; }
        //public ICollection<UserProfile> UserProfiles { get; set; }
        public virtual  UserInfo UserInfo { get; set; } 
        public virtual Home Home { get; set; } 
        #endregion
    }
}
