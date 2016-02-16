using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using SmartHome.Entity;
using SmartHome.MailApi.MailHelper;

namespace SmartHome.MailApi.Controllers
{
    public class EmailController : ApiController
    {
        // GET: Email
        [System.Web.Mvc.Route("api/SendMail")]
        public HttpResponseMessage SendMail(EmailEntity email, bool isEncrypted)
        {
            HttpResponseMessage response;
            if (ModelState.IsValid)
            {
                SmartHomeMailClient mailClient = new SmartHomeMailClient(email);
                response = Request.CreateResponse(HttpStatusCode.OK, mailClient.SendEmail(isEncrypted));
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.OK, false);
            }

            return response;
        }
    }
}