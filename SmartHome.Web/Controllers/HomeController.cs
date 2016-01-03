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
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IServerResponceService _serverResponceService;

        public HomeController(IUnitOfWorkAsync unitOfWorkAsync, IServerResponceService serverResponceService)
        {
            this._unitOfWorkAsync = unitOfWorkAsync;
            this._serverResponceService = serverResponceService;
        }

        //public ActionResult Index()
        //{
        //    try
        //    {
        //        int a = Convert.ToInt32("test");
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        // log error
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //        //continue
        //    }
        //    return View();


        //    //return View(_serverResponceService.Queryable());
        //}

        #region m2m



        public ActionResult Index()
        {
            try
            {
                int a = Convert.ToInt32("test");
                return View(new m2mMessageViewModel());
                
            }
            catch (Exception ex)
            {
                // log this and continue
                Logger.LogError(ex, "This is test error " );
            }
            return View(new m2mMessageViewModel());

        }

        public ActionResult Publish(m2mMessageViewModel model)
        {
            model.PublishMessageStatus = MqttClientWrapper.Publish(model.MessgeTopic, model.PublishMessage);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Subscribe(m2mMessageViewModel model)
        {
            model.SubscribehMessageStatus = MqttClientWrapper.Subscribe(model.MessgeTopic);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ServerResponce()
        {
            //ServerResponce serverResponce = Session.GetsServerResponce("ServerResponce");
            //return Json(serverResponce.ServerMessages, JsonRequestBehavior.AllowGet);


            return Json(null, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async Task<ActionResult> Create(ServerResponce model)
        {
            _serverResponceService.Insert(model);

            try
            {
                await _unitOfWorkAsync.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ResponceExists(model.MessageId))
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
                throw;
            }

            return View();
        }

        private bool ResponceExists(string key)
        {
            return _serverResponceService.Query(e => e.MessageId == key).Select().Any();
        }
    }
}