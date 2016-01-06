using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class UserProfile
    {
        public int UserProfileId { get; set; }
        public string LocalID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FullName { get; }
        public string AccNo { get; set; }        
        public string CellPhone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string OldAcc { get; set; }
        public string SocialSecurityNumber { get; set; }
        public bool IsEmailRecipient { get; set; }
        public bool? IsLoggedIn { get; set; }
        public bool IsSMSRecipient { get; set; }
        public DateTime? LastLogIn { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<UserType> UserTypes { get; set; }
        public virtual ICollection<SyncStatus> SyncStatuses { get; set; }
        public virtual ICollection<UserStatus> UserStatuses { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual Room Room { get; set; }


    }
}
