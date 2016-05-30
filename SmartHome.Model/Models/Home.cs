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
    public class Home : Entity
    {
        public Home()
        {
            MeshMode = MeshModeType.BLE;
        }
        #region Primitive Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HomeId { get; set; }

        public int AppsHomeId { get; set; }

        public string Name { get; set; }
        public string Address1 { get; set; }

        public string Address2 { get; set; }
        public string Block { get; set; }
        public string City { get; set; }

        public string ZipCode { get; set; }

        public string Country { get; set; }

        public string TimeZone { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public MeshModeType MeshMode { get; set; }
        public string Phone { get; set; }
        public string PassPhrase { get; set; }
        public string Zone { get; set; }
        public bool IsInternet { get; set; }

        public bool IsSynced { get; set; }


        #endregion

        #region Complex Properties
        public AuditFields AuditField { get; set; }

        #endregion

        #region Navigation Properties        
        public virtual ICollection<UserHomeLink> UserHomeLinks { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        //public virtual ICollection<SmartRouter> SmartRouters { get; set; }
        public virtual ICollection<SmartRouterInfo> SmartRouterInfoes { get; set; }
        #endregion
    }
}
