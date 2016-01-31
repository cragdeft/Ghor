using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
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
             new Model.Models.Version() { AppName="App 1",ObjectState=ObjectState.Added,AuditField=new AuditFields()},
             new Model.Models.Version() { AppName="App 2",AppVersion="1.0",ObjectState=ObjectState.Added,AuditField=new AuditFields()},
             new Model.Models.Version() { AppName="App 3",ObjectState=ObjectState.Added,AuditField=new AuditFields()}
            };
        }


        [TestMethod]
        public void Version_Service_Get_All()
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

    }
}
