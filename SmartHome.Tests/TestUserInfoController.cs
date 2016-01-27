using System;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
using SmartHome.Service.Interfaces;
using SmartHome.WebAPI.Controllers;
using SmartHome.Model.Models;

namespace SmartHome.Tests
{
    [TestClass]
    public class TestUserInfoController
    {
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IUserInfoService _userInfoService;

        public TestUserInfoController(IUnitOfWorkAsync unitOfWorkAsync, IUserInfoService userInfoService)
        {
            this._unitOfWorkAsync = unitOfWorkAsync;
            this._userInfoService = userInfoService;
        }
        [TestMethod]
        public void RegisterUser_ShouldReturnRegisteredUser()
        {
            IUnityContainer myContainer = new UnityContainer();
            //myContainer.RegisterType<_unitOfWorkAsync, >();
            // Arrange
            var controller = new UserInfoController(_unitOfWorkAsync, _userInfoService);   
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            // Act
            var response = controller.RegisterUser("","","asd@email.com","","","",0);

            // Assert
            UserInfo userInfo;
            Assert.IsTrue(response.TryGetContentValue<UserInfo>(out userInfo));
            Assert.AreEqual("asd@email.com", userInfo.Email);
        }
    }
}
