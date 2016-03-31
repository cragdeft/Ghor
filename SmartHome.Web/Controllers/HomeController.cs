using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
using SmartHome.Model.ModelDataContext;
using SmartHome.Service;
using SmartHome.Service.Interfaces;
using SmartHome.Web.Models;
using SmartHome.Web.Utility;
using System;
using System.Web.Mvc;

namespace SmartHome.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IVersionService _versionService;
        private readonly IDeviceService _deviceService;
        private readonly IConfigurationParserManagerService _configurationService;
        public HomeController(IUnitOfWorkAsync unitOfWorkAsync, IVersionService versionService, IDeviceService deviceService, IConfigurationParserManagerService configurationService)
        {
            this._unitOfWorkAsync = unitOfWorkAsync;
            this._versionService = versionService;
            this._deviceService = deviceService;
            this._configurationService = configurationService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("SmartHome");
        }
        public ActionResult Configure()
        {
            ViewBag.Message = "Configure";
            return View();
        }
        public ActionResult ConfigureTree()
        {
            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IConfigurationParserManagerService service = new ConfigurationParserManagerService(unitOfWork);
                try
                {
                    unitOfWork.BeginTransaction();
                    ViewBag.DeviceInfo = service.GetsDeviceAllInfo();
                    var tempVersion = service.GetsAllVersion();
                    unitOfWork.Commit();
                    return View(tempVersion);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult PublishMessage(m2mMessageViewModel model)
        {
            ViewBag.Message = "Publish Message";
            model.PublishMessageStatus = MqttClientWrapperAdapter.WrapperInstance.Publish(model.MessgeTopic, model.PublishMessage);
            return View("Configure", model);
        }
        [HttpPost]
        public ActionResult SubscribeMessage(m2mMessageViewModel model)
        {
            ViewBag.Message = "Subscribe Message";
            model.PublishMessageStatus = MqttClientWrapperAdapter.WrapperInstance.Subscribe(model.MessgeTopic);
            return View("Configure", model);
        }
        public ActionResult SmartHome()
        {
            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IConfigurationParserManagerService service = new ConfigurationParserManagerService(unitOfWork);
                try
                {
                    unitOfWork.BeginTransaction();
                    var oUserHomeLink = service.GetsHomesAllInfo();
                    var oVersion = service.GetsAppVersionAllInfo();
                    unitOfWork.Commit();

                    ViewBag.AppVersion = oVersion;
                    return View(oUserHomeLink);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                }
            }
            ViewBag.AppVersion = null;
            return View();
        }
    }



}