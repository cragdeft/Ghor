using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Models;

namespace SmartHome.Service
{
    public class JsonParserService:IJsonParserService
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<SmartRouter> _routerRepository;
        private readonly IRepositoryAsync<Home> _homeRepository;
        private string _email;
        #endregion

        public JsonParserService(IUnitOfWorkAsync unitOfWorkAsync)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _routerRepository = _unitOfWorkAsync.RepositoryAsync<SmartRouter>();
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();
        }
        public bool IsRouterExists(SmartRouter router)
        {
            return
            _routerRepository
                .Queryable().Where(u => u.Ssid == router.Ssid.ToString()).FirstOrDefault() != null;
        }

        public bool IsHomeExists(HomeEntity home)
        {
            return
            _homeRepository
                .Queryable().Where(u => u.Id == home.Id.ToString()).FirstOrDefault() != null;
        }

        public void SaveHome(HomeEntity home)
        {
            InsertHome(home);
        }

        private void InsertHome(HomeEntity home)
        {
            _unitOfWorkAsync.BeginTransaction();
            Mapper.CreateMap<HomeEntity, Home>();
            Home model = Mapper.Map<HomeEntity, Home>(home);
            try
            {
                model.ObjectState = ObjectState.Added;
                model.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
                _homeRepository.Insert(model);
                SaveHomeRouter(home);
                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();
            }
            catch (Exception ex)
            {

                _unitOfWorkAsync.Rollback();
            }
        }

        public void UpdateHome(HomeEntity home)
        {
            _unitOfWorkAsync.BeginTransaction();
            Mapper.CreateMap<HomeEntity, Home>();
            Home model = Mapper.Map<HomeEntity, Home>(home);
            try
            {
                model.ObjectState = ObjectState.Modified;
                model.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
                _homeRepository.Update(model);
                UpdateHomeRouter(home);
                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();
            }
            catch (Exception ex)
            {

                _unitOfWorkAsync.Rollback();
            }
        }

        private void SaveHomeRouter(HomeEntity home)
        {
            foreach (var smartRouterEntity in home.SmartRouter)
            {
                Mapper.CreateMap<SmartRouterEntity, SmartRouter>();
                var entity = Mapper.Map<SmartRouterEntity, SmartRouter>(smartRouterEntity);
                entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
                entity.ObjectState = ObjectState.Added;
                _routerRepository.Insert(entity);
                _routerRepository.FindAsync(smartRouterEntity);
            }
        }

        private void UpdateHomeRouter(HomeEntity home)
        {
            foreach (var smartRouterEntity in home.SmartRouter)
            {
                Mapper.CreateMap<SmartRouterEntity, SmartRouter>();
                var entity = Mapper.Map<SmartRouterEntity, SmartRouter>(smartRouterEntity);
                entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
                entity.ObjectState = ObjectState.Modified;
                _routerRepository.Update(entity);
                _routerRepository.FindAsync(smartRouterEntity);
            }
        }
    }
}
