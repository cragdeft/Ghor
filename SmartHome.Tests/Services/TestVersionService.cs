using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Models;
using SmartHome.Service;
using SmartHome.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Tests.Services
{
    [TestClass]
    public class TestVersionService
    {
        private Mock<IRepositoryAsync<Model.Models.Version>> _mockRepository;
        private IVersionService _service;
        List<Model.Models.Version> listVersion;

        [TestInitialize]
        public void Initialize()
        {

            _mockRepository = new Mock<IRepositoryAsync<Model.Models.Version>>();
            _service = new VersionService(_mockRepository.Object);

            listVersion = new List<Model.Models.Version>() {
             new Model.Models.Version() { VersionId=1,AppName="App 1",AppVersion="1.0",ObjectState=ObjectState.Added,AuditField=new AuditFields()},
             new Model.Models.Version() { VersionId=1, AppName="App 2",AppVersion="1.0",ObjectState=ObjectState.Added,AuditField=new AuditFields()},
             new Model.Models.Version() { VersionId=1,AppName="App 3",ObjectState=ObjectState.Added,AuditField=new AuditFields()}
            };
        }


        [TestMethod]
        public void Version_Service_Gets_Async()
        {
            //Arrange            
            _mockRepository.Setup(x => x.Query().SelectAsync()).Returns(Task.FromResult((IEnumerable<Model.Models.Version>)listVersion));

            //Act
            Task.Run(async () =>
            {
                var executionResult = await _service.GetsAsync();
                //Assert
                Assert.AreEqual(executionResult.Count(), 3);
            }).GetAwaiter().GetResult();
        }


        [TestMethod]
        public void Version_Service_Get_Async()
        {
            //Arrange            
            _mockRepository.Setup(x => x.FindAsync(1)).Returns(Task.FromResult((Model.Models.Version)listVersion.First()));

            //Act
            Task.Run(async () =>
            {
                var executionResult = await _service.GetAsync(1);

                //Assert                
                Assert.IsNotNull(executionResult);
                Assert.AreEqual(executionResult.AppName, "App 1");

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Can_Add_Version()
        {
            //Arrange
            int versionId = 1;
            VersionEntity version = new VersionEntity() { VersionId = versionId, AppVersion = "App1" };

            //Act
            _service.Add(version);

            //Assert
            Assert.AreEqual(version.VersionId, versionId);
        }


        [TestMethod]
        public void Can_Add_Invalid_Version()
        {
            //Arrange
            int versionId = 1;
            VersionEntity version = new VersionEntity() { AppVersion = "App1" };

            //Act
            _service.Add(version);

            //Assert
            Assert.AreEqual(version.VersionId, versionId);
        }


        public void Version_Service_Modify()
        {
            //Arrange
            int versionId = 1;

            var version = listVersion.First();

            //Act
            _service.Update(version);

            //Assert
          
            Assert.AreEqual(version.VersionId, versionId);
        }

        public void Version_Service_Delete()
        {
            //Arrange
            int versionId = 1;

            var version = listVersion.First();

            //Act
            _service.Delete(version);

            //Assert

            Assert.AreEqual(version.VersionId, versionId);
        }


        


    }
}
