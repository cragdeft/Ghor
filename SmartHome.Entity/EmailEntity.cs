using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace SmartHome.Entity
{
    public class EmailEntity
    {
        #region Primitive Properties
        [EmailAddress]
        public string From { get; set; }
        [EmailAddress]
        public string To{ get; set; }
        public string Subject { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public bool EnableSsl { get; set; }
        public bool IsBodyHtml { get; set; }
        public DateTime SentDate { get; set; }

        public EmailEntity()
        {
            this.From = "smarthome-noreply@sinepulse.com";
        }

        public void SetValue(string jsonString)
        {
            EmailEntity entity = JsonConvert.DeserializeObject<EmailEntity>(jsonString);

            if (entity != null)
            {
                this.Body = entity.Body;
                this.To = entity.To;
                this.Subject = entity.Subject;
            }


        }
        #endregion
    }
}
