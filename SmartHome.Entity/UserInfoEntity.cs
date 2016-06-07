using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class UserInfoEntity
    {
        #region Primitive Properties
        public int UserInfoId { get; set; }
        [JsonProperty("Id")]
        public int AppsUserId { get; set; }
        public string LocalId { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FullName { get; }
        public string AccNo { get; set; }
        [JsonProperty("MobileNumber")]
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
        public int LoginStatus { get; set; }
        public int RegStatus { get; set; }
        public int IsSynced { get; set; }
        #endregion
    }
}
