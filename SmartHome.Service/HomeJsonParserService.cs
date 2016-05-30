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
    public class HomeJsonParserService : IHomeJsonParserService
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<SmartRouter> _routerRepository;
        private readonly IRepositoryAsync<Home> _homeRepository;
        private string _email;
        #endregion

        public HomeJsonParserService(IUnitOfWorkAsync unitOfWorkAsync)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _routerRepository = _unitOfWorkAsync.RepositoryAsync<SmartRouter>();
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();
        }
        public SmartRouterEntity GetRouter(string macAddress)
        {
            SmartRouter router = _routerRepository
                .Queryable().Where(u => u.MacAddress == macAddress).FirstOrDefault();

            Mapper.CreateMap<SmartRouter, SmartRouterEntity>();
            return Mapper.Map<SmartRouter, SmartRouterEntity>(router);
        }

        public HomeEntity GetHome(int homeId)
        {
            Home home = _homeRepository
                .Queryable().Where(u => u.Id == homeId.ToString()).FirstOrDefault();

            Mapper.CreateMap<Home, HomeEntity>();
            return Mapper.Map<Home, HomeEntity>(home);
        }

        public void InsertHome(HomeEntity home)
        {
            _unitOfWorkAsync.BeginTransaction();
            Mapper.CreateMap<HomeEntity, Home>();
            Home model = Mapper.Map<HomeEntity, Home>(home);
            try
            {
                model.ObjectState = ObjectState.Added;
                model.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
                _homeRepository.Insert(model);
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
                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();
            }
            catch (Exception ex)
            {

                _unitOfWorkAsync.Rollback();
            }
        }

        public void SaveRouter(SmartRouterEntity router)
        {

            Mapper.CreateMap<SmartRouterEntity, SmartRouter>();
            var entity = Mapper.Map<SmartRouterEntity, SmartRouter>(router);
            _unitOfWorkAsync.BeginTransaction();
            try
            {
                entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
                entity.ObjectState = ObjectState.Added;
                _routerRepository.Insert(entity);
                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();
            }
            catch (Exception ex)
            {

                _unitOfWorkAsync.Rollback();
            }
        }

        public void UpdateRouter(SmartRouterEntity router)
        {
            Mapper.CreateMap<SmartRouterEntity, SmartRouter>();
            var entity = Mapper.Map<SmartRouterEntity, SmartRouter>(router);
            try
            {
                entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
                entity.ObjectState = ObjectState.Modified;
                _routerRepository.Update(entity);
                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();
            }
            catch (Exception ex)
            {

                _unitOfWorkAsync.Rollback();
            }
        }
    }
}
