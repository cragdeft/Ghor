using Newtonsoft.Json;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Service.Interfaces;
using SmartHome.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        #region Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginEntity model, string returnUrl = "")
        {
            if (ModelState.IsValid)
            {
                var user = _userInfoService
                    .Query(p => p.UserName == model.Username && p.Password == model.Password)
                    .Include(x => x.WebPagesRoles)
                    .Select()
                    .FirstOrDefault();

                if (user != null)
                {
                    SetCookieInfomation(model, user);
                    return Redirect(returnUrl);
                }

                ModelState.AddModelError("", "Incorrect username and/or password");
            }
            ModelState.Remove("Password");
            return View(model);
        } 
        #endregion

        #region Cookie infomations

        private void SetCookieInfomation(LoginEntity model, Model.Models.UserInfo user)
        {
            var roles = user.WebPagesRoles.Select(p => p.RoleName).ToArray();
            CustomPrincipalSerializeModel serializeModel = FillCookieSerializedInformation(user, roles);

            string userData = JsonConvert.SerializeObject(serializeModel);
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                     1,
                    user.Email,
                     DateTime.Now,
                     DateTime.Now.AddMinutes(15),
                     model.RememberMe,
                     userData);
            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            Response.Cookies.Add(faCookie);
        }

        private static CustomPrincipalSerializeModel FillCookieSerializedInformation(Model.Models.UserInfo user, string[] roles)
        {
            CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
            serializeModel.UserId = user.UserInfoId;
            serializeModel.FirstName = user.FirstName;
            serializeModel.LastName = user.LastName;
            serializeModel.roles = roles;
            return serializeModel;
        } 
        #endregion

        [AllowAnonymous]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account", null);
        }
    }
}