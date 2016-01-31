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

namespace SmartHome.Tests.Controllers
{
    [TestClass]
    public class TestVersionController
    {
        private Mock<IVersionService> _versionServiceMock;
        private Mock<IUnitOfWorkAsync> _unitOfWorkAsync;
        VersionController objController;
        List<Model.Models.Version> listVersion;
        public Mock<IVersionService> VersionServiceMock
        {
            get
            {
                return _versionServiceMock;
            }

            set
            {
                _versionServiceMock = value;
            }
        }

        [TestInitialize]
        public void Initialize()
        {

            VersionServiceMock = new Mock<IVersionService>();
            _unitOfWorkAsync = new Mock<IUnitOfWorkAsync>();
            objController = new VersionController(_unitOfWorkAsync.Object, VersionServiceMock.Object);

            listVersion = new List<Model.Models.Version>() {
             new Model.Models.Version() { AppName="App 1",ObjectState=ObjectState.Added,AuditField=new AuditFields()},
             new Model.Models.Version() { AppName="App 2",AppVersion="1.0",ObjectState=ObjectState.Added,AuditField=new AuditFields()},
             new Model.Models.Version() { AppName="App 3",ObjectState=ObjectState.Added,AuditField=new AuditFields()}
            };
        }

        [TestMethod]
        public void Version_Get_All()
        {
            //Arrange
            VersionServiceMock.Setup(x => x.Queryable()).Returns(listVersion.AsQueryable());

            //Act
            var result = ((objController.Index2() as ViewResult).Model) as List<Model.Models.Version>;
            ////Assert
            Assert.AreEqual(result.Count, 3);



        }
    }
}
