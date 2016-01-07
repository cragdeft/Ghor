using Repository.Pattern.UnitOfWork;
using SmartHome.Logging;
using SmartHome.Model.Models;
using SmartHome.MQTT.Client;
using SmartHome.Service.Interfaces;
using SmartHome.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace SmartHome.Web.Controllers
{
    public class HomeController : Controller
    {


        public ActionResult Index()
        {
            try
            {
                //int a = Convert.ToInt32("test");
                return View();
            }
            catch (Exception ex)
            {
                // log error
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                //continue
            }
            return View();


            //return View(_serverResponceService.Queryable());
        }

    }
}