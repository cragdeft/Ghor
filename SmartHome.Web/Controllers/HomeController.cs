using Newtonsoft.Json;
using Repository.Pattern.UnitOfWork;
using SmartHome.Logging;
using SmartHome.Model.Models;
using SmartHome.MQTT.Client;
using SmartHome.Service.Interfaces;
using SmartHome.Utility.EncriptionAndDecryption;
using SmartHome.Web.Filters;
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

    //[CustomAuthorize(Roles = "User", Users = "1,2,3")]
    public class HomeController : BaseController
    {
        //private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        //private readonly IServerResponceService _serverResponceService;

        //public HomeController(IUnitOfWorkAsync unitOfWorkAsync, IServerResponceService serverResponceService)
        //{
        //    this._unitOfWorkAsync = unitOfWorkAsync;
        //    this._serverResponceService = serverResponceService;
        //}

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


       // [CustomAuthorize(Roles = "User", Users = "1,2,3")]
        public ActionResult Index()
        {
            //var temp = User;
            #region Parese Json

            //string jsonSMessage = "{\"Version\":[{\"ID\":1,\"AppName\":\"SmartHome\",\"Version\":\"1.0\",\"AuthCode\":\"0123456789ABCDEF\",\"PassPhrase\":\"G4R9EI\"}],\"VersionDetails\":[{\"ID\":1,\"VersionID\":\"1\",\"HardwareVersion\":\"00\"}],\"Device\":[{\"ID\":32769,\"DeviceHash\":1,\"DeviceType\":\"1\",\"DeviceName\":\"title\",\"IsDeleted\":0},{\"ID\":32770,\"DeviceHash\":2,\"DeviceType\":\"1\",\"DeviceName\":\"title2\",\"IsDeleted\":0},{\"ID\":32771,\"DeviceHash\":3,\"DeviceType\":\"1\",\"DeviceName\":\"title3\",\"IsDeleted\":0},{\"ID\":32772,\"DeviceHash\":4,\"DeviceType\":\"1\",\"DeviceName\":\"title4\",\"IsDeleted\":0},{\"ID\":32773,\"DeviceHash\":5,\"DeviceType\":\"1\",\"DeviceName\":\"title5\",\"IsDeleted\":0}],\"DeviceStatus\":[{\"ID\":1,\"DeviceID\":1,\"StatusType\":53,\"Status\":1},{\"ID\":2,\"DeviceID\":2,\"StatusType\":53,\"Status\":1},{\"ID\":3,\"DeviceID\":3,\"StatusType\":55,\"Status\":1},{\"ID\":4,\"DeviceID\":32769,\"StatusType\":5,\"Status\":0},{\"ID\":5,\"DeviceID\":32770,\"StatusType\":5,\"Status\":0},{\"ID\":6,\"DeviceID\":32772,\"StatusType\":5,\"Status\":0},{\"ID\":7,\"DeviceID\":32771,\"StatusType\":5,\"Status\":0},{\"ID\":8,\"DeviceID\":32773,\"StatusType\":5,\"Status\":0}],\"ChannelConfig\":[{\"ID\":1,\"DeviceID\":32769,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"Status\":0,\"Value\":0},{\"ID\":2,\"DeviceID\":32769,\"ChannelNo\":6,\"LoadType\":2,\"LoadName\":\"Dimlightan\",\"Status\":0,\"Value\":0}],\"NextDeviceID\":32769}";
            //RootObject myObj = JsonConvert.DeserializeObject<RootObject>(jsonSMessage);
            string de = SecurityManager.Decrypt("ŒS€º¦Éb®¨…_’");

            string en = SecurityManager.Encrypt("ŒS€º¦Éb®¨…_’");
            

            #endregion




            // return RedirectToAction("Index", "Customer");

            try
            {
                // int a = Convert.ToInt32("test");
                return View(new m2mMessageViewModel());

            }
            catch (Exception ex)
            {
                // log this and continue
                Logger.LogError(ex, "This is test error ");
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

        //public async Task<ActionResult> Create(ServerResponce model)
        //{
        //    _serverResponceService.Insert(model);

        //    try
        //    {
        //        await _unitOfWorkAsync.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        if (ResponceExists(model.MessageId))
        //        {
        //            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //        }
        //        throw;
        //    }

        //    return View();
        //}

        //private bool ResponceExists(string key)
        //{
        //    return _serverResponceService.Query(e => e.MessageId == key).Select().Any();
        //}
    }



}