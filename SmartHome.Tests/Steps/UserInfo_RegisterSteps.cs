using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Moq;
using NUnit.Framework;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Service.Interfaces;
using SmartHome.WebAPI.Controllers;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SmartHome.Tests.Steps
{
    [Binding]
    public class UserInfo_RegisterSteps
    {
        public Mock<IUnitOfWorkAsync> UnitOfWork{ get; set; }
        public Mock<IUserInfoService> UserInfoService { get; set; }
        public UserInfoController UserInfoController { get; set; }

        public HttpResponseMessage HttpResponseMessage { get; set; }

        [Given]
        public void Given_I_have_a_new_UserInfoEntity_record_with_the_following_properties(Table table)
        {
            UnitOfWork = new Mock<IUnitOfWorkAsync>();
            UserInfoService = new Mock<IUserInfoService>();
            var temp = table.CreateSet<UserInfoEntity>();

            UserInfoService.Setup(x => x.IsLoginIdUnique(table.CreateInstance<UserInfoEntity>().Email))
                .Returns(IsLoginIdUnique(table.CreateInstance<UserInfoEntity>().Email));

            UserInfoController = new UserInfoController(UnitOfWork.Object, UserInfoService.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            
            ScenarioContext.Current.Set<UserInfoEntity>(table.CreateInstance<UserInfoEntity>());
        }
        
        [When]
        public void When_I_save_the_UserInfoEntity()
        {
            HttpResponseMessage = UserInfoController.RegisterUser(ScenarioContext.Current.Get<UserInfoEntity>());
            ScenarioContext.Current.Set<HttpResponseMessage>(HttpResponseMessage, "SaveResult");
        }
        
        [Then]
        public void Then_the_UserInfoEntity_is_saved_successfully()
        {
            Assert.AreEqual(ScenarioContext.Current.Get<HttpResponseMessage>("SaveResult").StatusCode, HttpStatusCode.OK);
        }
        
        [Then]
        public void Then_I_can_retrieve_the_UserInfoEntity_by_UserInfoId()
        {
            ScenarioContext.Current.Pending();
        }

        private bool IsLoginIdUnique(string deviceHash)
        {
            List<UserInfoEntity> userList = new List<UserInfoEntity>()
            {
                new UserInfoEntity() { Email = "a@b.com"},
                new UserInfoEntity() { Email = "c@b.com"},
                new UserInfoEntity() { Email = "d@b.com"},
                new UserInfoEntity() { Email = "e@b.com"}
            };

            return userList.Exists(x => x.Email == deviceHash);
        }
    }
}
