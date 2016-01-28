using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Models;
using SmartHome.Service;
using SmartHome.Service.Interfaces;
using SmartHome.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SmartHome.Tests
{
    [TestClass]
    public class TestVersionController
    {
        private Mock<IVersionService> _versionServiceMock;
        private Mock<IUnitOfWorkAsync> _unitOfWorkAsync;

        #region MyRegion
        private Mock<IRepositoryAsync<Model.Models.Version>> _mockRepository;
        private IVersionService _service;
        #endregion


        VersionController objController;
        List<Model.Models.Version> listVersion;
        List<VersionEntity> listVersionEntity;

        [TestInitialize]
        public void Initialize()
        {

            _versionServiceMock = new Mock<IVersionService>();
            _unitOfWorkAsync = new Mock<IUnitOfWorkAsync>();
            objController = new VersionController(_unitOfWorkAsync.Object, _versionServiceMock.Object);


            #region MyRegion

            _mockRepository = new Mock < IRepositoryAsync < Model.Models.Version >> ();
            
            _service = new VersionService( _mockRepository.Object);

            #endregion

            listVersion = new List<Model.Models.Version>() {
             new Model.Models.Version() { AppName="App 1",ObjectState=ObjectState.Added,AuditField=new AuditFields()},
             new Model.Models.Version() { AppName="App 2",AppVersion="1.0",ObjectState=ObjectState.Added,AuditField=new AuditFields()},
             new Model.Models.Version() { AppName="App 3",ObjectState=ObjectState.Added,AuditField=new AuditFields()}
            };


            listVersionEntity = new List<VersionEntity>() {
             new VersionEntity() { AppName="App 1"},
             new VersionEntity() { AppName="App 2",AppVersion="1.0"},
             new VersionEntity() { AppName="App 3"}
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





        [TestMethod]
        public void Version_Get_All()
        {


            //Arrange
            _versionServiceMock.Setup(x => x.Queryable()).Returns(listVersion.AsQueryable());

            //Act
            var result = ((objController.Index2() as ViewResult).Model) as List<Model.Models.Version>;
            ////Assert
            Assert.AreEqual(result.Count, 3);



        }
    }
}
