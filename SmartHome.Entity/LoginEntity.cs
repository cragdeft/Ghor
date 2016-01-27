using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class LoginEntity
    {
        #region Primitive Properties
        [Required(ErrorMessage = "Username required.", AllowEmptyStrings = false)]

        public string Username { get; set; }


        [Required(ErrorMessage = "Password required.", AllowEmptyStrings = false)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        #endregion
    }
}
