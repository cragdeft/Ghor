using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SmartHome.Entity
{
    public class EmailEntity
    {
        #region Primitive Properties
        [EmailAddress]
        public string FromAddress { get; set; }
        [EmailAddress]
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public bool EnableSSL { get; set; }
        public bool IsBodyHtml { get; set; }
        public DateTime SentDate { get; set; }

        public EmailEntity()
        {
            this.FromAddress = "smarthome-noreply@sinepulse.com";
        }
        #endregion
    }
}
