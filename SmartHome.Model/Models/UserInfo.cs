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
    public class UserInfo : Entity
    {
        #region Primitive Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserInfoId { get; set; }
        public int AppsUserId { get; set; }
        public string LocalId { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FullName { get; }
        public string AccNo { get; set; }
        public string CellPhone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string OldAcc { get; set; }
        public string SocialSecurityNumber { get; set; }
        public bool IsEmailRecipient { get; set; }
        public bool? IsLoggedIn { get; set; }
        public bool IsSMSRecipient { get; set; }
        public DateTime? LastLogIn { get; set; }
        public bool IsActive { get; set; }
        public string Country { get; set; }
        public bool LoginStatus { get; set; }
        public bool RegStatus { get; set; }
        public bool IsSynced { get; set; }
        #endregion

        #region Complex Properties
        public AuditFields AuditField { get; set; }
        #endregion

        #region Navigation Properties
        public virtual ICollection<UserType> UserTypes { get; set; }
        public virtual ICollection<SyncStatus> SyncStatuses { get; set; }
        public virtual ICollection<UserStatus> UserStatuses { get; set; }

        public virtual ICollection<UserHomeLink> UserHomeLinks { get; set; }
        //public virtual ICollection<Home> Homes { get; set; }
        //public virtual ICollection<Address> Addresses { get; set; }
        //public virtual Room Room { get; set; } 
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<UserRoomLink> UserRoomLinks { get; set; }
        #endregion


    }
}
