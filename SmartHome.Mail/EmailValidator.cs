using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Mail
{
    public static class EmailValidator
    {
        public static bool IsValid(object email)
        {
            if (email != null)
            {
                List<ValidationResult> errors = new List<ValidationResult>();

                var context = new ValidationContext(email, null, null);

                bool isValid = Validator.TryValidateObject(email, context, errors, true);

                if (isValid)
                {
                    return true;
                }
            }

            return false;

        }
    }
}
