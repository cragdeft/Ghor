using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{
    public class HomeInfoNewEntryJsonParserService : IHomeJsonParserService<Home>
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<Home> _homeRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }

        #endregion
        public HomeInfoNewEntryJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
            SetMapper();
        }

        public Home SaveJsonData()
        {
            Home home = null;

            //SetMapper();
            try
            {
                home = SaveNewHome();
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
        private Home SaveNewHome()
        {
            HomeEntity homeEntity = _homeJsonEntity.Home.FirstOrDefault();
            Home home = null;

            if (homeEntity != null)
            {
                home = InsertHome(homeEntity);
            }
            return home;
        }
        public Home InsertHome(HomeEntity homeEntity)
        {
            //Mapper.CreateMap<HomeEntity, Home>();

            Home model = Mapper.Map<HomeEntity, Home>(homeEntity);
            model.IsInternet = Convert.ToBoolean(homeEntity.IsInternet);
            model.IsDefault = Convert.ToBoolean(homeEntity.IsDefault);
            model.IsActive = Convert.ToBoolean(homeEntity.IsActive);
            model.IsSynced = Convert.ToBoolean(homeEntity.IsSynced);
            model.ObjectState = ObjectState.Added;
            model.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            _homeRepository.Insert(model);
            return model;
        }
    }
}
