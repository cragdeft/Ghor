using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.Models;
using SmartHome.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{
    public class HomeInfoUpdateJsonParserService : IHomeUpdateJsonParserService<Home>
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<Home> _homeRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }

        #endregion
        public HomeInfoUpdateJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
        }

        public Home UpdateJsonData()
        {
            Home home = null;

            SetMapper();
            try
            {
                home = UpdateHomeInfo();
            }
            catch (Exception ex)
            {
                return null;
            }

            return home;
        }
        private void SetMapper()
        {
            Mapper.CreateMap<HomeEntity, Home>();
        }
        private Home UpdateHomeInfo()
        {
            HomeEntity homeEntity = _homeJsonEntity.Home.FirstOrDefault();
            Home dbHome = null;
            Home home = null;

            dbHome = new CommonService(_unitOfWorkAsync).GetHome(homeEntity.PassPhrase);

            if (homeEntity != null)
            {
                home = UpdateHome(homeEntity, dbHome);
            }
            return home;
        }
        public Home UpdateHome(HomeEntity homeEntity, Home dbHome)
        {
            Home model = SmartHomeTranslater.MapHomeInfoProperties(homeEntity, dbHome);
            model.ObjectState = ObjectState.Modified;
            _homeRepository.Update(model);
            return model;
        }
    }
}
