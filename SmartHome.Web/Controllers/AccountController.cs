using Newtonsoft.Json;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
using SmartHome.Model.ModelDataContext;
using SmartHome.Service;
using SmartHome.Service.Interfaces;
using SmartHome.Utility.EncryptionAndDecryption;
using SmartHome.Web.Models;
using SmartHome.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SmartHome.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IUserInfoService _userInfoService;
        public AccountController(IUnitOfWorkAsync unitOfWorkAsync, IUserInfoService userInfoService)
        {
            this._unitOfWorkAsync = unitOfWorkAsync;
            this._userInfoService = userInfoService;
        }




        public ActionResult Index(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginViewModel model, string ReturnUrl = "")
        {
            if (ModelState.IsValid)
            {
                using (IDataContextAsync context = new SmartHomeDataContext())
                {
                    using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
                    {
                        IUserInfoService service = new UserInfoService(unitOfWork);
                        try
                        {
                            unitOfWork.BeginTransaction();
                            // string en = SecurityManager.PassEncrypt(model.Password.Trim());
                            //pass=qwerty
                            //string de = SecurityManager.PassDecrypt("46tUX/XbJOPCnTLtU283wg==");
                            var user = service.GetsUserInfosByEmail(model.Username, SecurityManager.PassEncrypt(model.Password.Trim())).FirstOrDefault();

                            #region Login Logic

                            if (user != null)
                            {
                                //var roles = service.GetUserRole(user.UserInfoId).Select(p => p.RoleName).ToArray();//user.Roles.Select(m => m.RoleName).ToArray();
                                var roles = new[] { user.UserHomeLinks.FirstOrDefault() == null ? "Normal" : user.UserHomeLinks.FirstOrDefault().IsAdmin == true ? "Admin" : "Normal" };
                                CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
                                serializeModel.UserInfoId = user.UserInfoId;
                                serializeModel.FirstName = user.FirstName;
                                serializeModel.LastName = user.LastName;
                                serializeModel.Email = user.Email;
                                serializeModel.IsAdmin = user.UserHomeLinks.FirstOrDefault() == null ? false : user.UserHomeLinks.FirstOrDefault().IsAdmin;
                                serializeModel.roles = roles;

                                string userData = JsonConvert.SerializeObject(serializeModel);
                                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                                         (int)user.UserInfoId,
                                        user.Email,
                                         DateTime.Now,
                                         DateTime.Now.AddMinutes(15),
                                         model.RememberMe,
                                         userData);

                                string encTicket = FormsAuthentication.Encrypt(authTicket);
                                HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                                if (model.RememberMe)
                                {
                                    faCookie.Expires = DateTime.Now.AddMinutes(30);
                                }

                                Response.Cookies.Add(faCookie);
                                string redirectUrl = FormsAuthentication.GetRedirectUrl(model.Username, false);
                                Response.Redirect(redirectUrl.Contains(".aspx") ? "/" : redirectUrl);

                            }

                            #endregion


                            unitOfWork.Commit();

                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                        }
                    }
                }

                ModelState.AddModelError("", "Incorrect username and/or password");
            }


            return View(model);
        }

        [AllowAnonymous]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Response.Cookies["ASPXPIKESADMINAUTH"].Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("Index", "Account", null);
        }
    }
}