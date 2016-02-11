using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repository.Pattern.Repositories;
using SmartHome.Entity;
using SmartHome.Model.Models;
using SmartHome.Service;
using SmartHome.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SmartHome.Tests.Steps
{
    [Binding]
    public class Version_AddSteps
    {
        private Mock<IRepositoryAsync<Version>> _mockRepository;
        private IVersionService _service;

        [Given]
        public void Given_I_have_a_new_version_record_with_the_following_properties(Table table)
        {
            _mockRepository = new Mock<IRepositoryAsync<Version>>();
            _service = new VersionService(_mockRepository.Object);

            var temp = table.CreateSet<VersionEntity>();           
            ScenarioContext.Current.Set<VersionEntity>(table.CreateInstance<VersionEntity>());
            //ScenarioContext.Current.Set<VersionEntity>(table.CreateSet<VersionEntity>());

        }

        [When]
        public void When_I_save_the_version()
        {
            var r = _service.Add(ScenarioContext.Current.Get<VersionEntity>());
            ScenarioContext.Current.Set<VersionEntity>(r, "SaveResult");
        }

        [Then]
        public void Then_the_version_is_saved_successfully()
        {
            var temp = ScenarioContext.Current.Get<VersionEntity>();
            Assert.IsNotNull(temp);
            Assert.AreEqual(ScenarioContext.Current.Get<VersionEntity>("SaveResult").VersionId, 1);
        }

        [Then]
        public async void Then_I_can_retrieve_the_version_by_Id()
        {
            var actual = ScenarioContext.Current.Get<VersionEntity>();
            Mapper.CreateMap<VersionEntity, Version>();
            var temp = Mapper.Map<VersionEntity, Version>(actual);
            _mockRepository.Setup(x => x.FindAsync(actual.VersionId)).Returns(Task.FromResult(temp));
            //_mockRepository.Setup(x => x.Query().SelectAsync()).Returns(temp);
            var expected = await _service.GetAsync(actual.VersionId);
            Assert.IsNotNull(expected);
            Assert.AreEqual(expected.VersionId, actual.VersionId);
        }
    }
}
