using System.Net.Http;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Service.Interfaces;
using SmartHome.WebAPI.Controllers;

namespace SmartHome.Tests
{
    [TestClass]
    public class TestUserInfoController
    {
        [TestMethod]
        public void RegisterUser_ShouldReturnRegisteredUser()
        {
            var uow = new Mock<IUnitOfWorkAsync>();
            var us = new Mock<IUserInfoService>();
            // Arrange
            var controller = new UserInfoController(uow.Object, us.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            // Act
            var response = controller.RegisterUser("", "", "asd@emaiyl.com", "", "", "", "");
            System.Diagnostics.Trace.WriteLine(response.Content);
            // Assert
            bool isEmailExists = false;
            if (!response.TryGetContentValue(out isEmailExists))
            {
                UserInfoEntity userInfo;
                Assert.IsTrue(response.TryGetContentValue(out userInfo));
                Assert.AreEqual("asd@emaiyl.com", userInfo.Email);
            } 
            else
                Assert.AreEqual(true, isEmailExists);
        }
        
    }
}
