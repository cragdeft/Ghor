using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Models
{

    public class m2mMessageViewModel
    {
        [Display(Name = "Topic")]
        public string MessgeTopic { get; set; }

        [Display(Name = "Status")]
        public string PublishMessageStatus { get; set; }

        [Display(Name = "Publish")]
        public string PublishMessage { get; set; }


        [Display(Name = "Subscribe")]
        public string SubscriberMessage { get; set; }

        [Display(Name = "Status")]
        public string SubscribehMessageStatus { get; set; }

        [Display(Name = "Responce")]
        [DataType(DataType.MultilineText)]
        public string ReceivedMessage { get; set; }
    }
}