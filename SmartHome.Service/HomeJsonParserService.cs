using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Mappers;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.Models;

namespace SmartHome.Service
{
    public class HomeJsonParserService : IHomeJsonParserService
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<SmartRouter> _routerRepository;
        private readonly IRepositoryAsync<Home> _homeRepository;
        private readonly IRepositoryAsync<Room> _roomRepository;
        public HomeJsonEntity _homeJsonEntity { get; private set; }
        private string _email;
        #endregion

        public HomeJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _routerRepository = _unitOfWorkAsync.RepositoryAsync<SmartRouter>();
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();
            _roomRepository = _unitOfWorkAsync.RepositoryAsync<Room>();
            _homeJsonEntity = homeJsonEntity;
        }
        public SmartRouterEntity GetRouter(string macAddress)
        {
            SmartRouter router = _routerRepository
                .Queryable().Include(x=>x.Parent).Where(u => u.MacAddress == macAddress).FirstOrDefault();

            Mapper.CreateMap<Home, HomeEntity>();
            Mapper.CreateMap<SmartRouter, SmartRouterEntity>();
            return Mapper.Map<SmartRouter, SmartRouterEntity>(router);
        }

        public HomeEntity GetHome(int homeId)
        {
            Home home = _homeRepository
                .Queryable().Include(x => x.Rooms).Where(u => u.HomeId == homeId).FirstOrDefault();

            Mapper.CreateMap<Home, HomeEntity>();
            return Mapper.Map<Home, HomeEntity>(home);
        }

        public Home InsertHome(HomeEntity home)
        {
            Home model = Mapper.Map<HomeEntity, Home>(home);
            model.ObjectState = ObjectState.Added;
            model.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            _homeRepository.Insert(model);
            return model;
        }

        public Home UpdateHome(HomeEntity home)
        {
            Home model = MapHomeProperty(home);
            model.ObjectState = ObjectState.Modified;
            _homeRepository.Update(model);
            return model;

        }

        public void InsertRouter(SmartRouterEntity router,Home home)
        {
            var entity = Mapper.Map<SmartRouterEntity, SmartRouter>(router);
            entity.Parent = home;
            entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            entity.ObjectState = ObjectState.Added;
            _routerRepository.Insert(entity);
        }

        public void UpdateRouter(SmartRouterEntity router)
        {
            var entity = MapRouterProperty(router);
            entity.ObjectState = ObjectState.Modified;
            _routerRepository.Update(entity);
        }

        public bool SaveJsonData()
        {
            _unitOfWorkAsync.BeginTransaction();
            SetMapper();
            try
            {
                SaveHomeAndRouter();
                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWorkAsync.Rollback();
                return false;
            }

            return true;
        }

        private void SaveHomeAndRouter()
        {
            SmartRouterEntity router = GetRouter(_homeJsonEntity.RouterInfo[0].MacAddress);
            HomeEntity home = null;
            if (router != null)
            {
                home = GetHome(router.Parent.HomeId);
                _homeJsonEntity.Home[0].HomeId = router.Parent.HomeId;
            }

            Home model = SaveOrUpdateHome(home);
            SaveOrUpdateRouter(router,model);
            SaveOrUpdateRoom(model);
            SaveOrUpdateDevice(model);
        }

        private void SaveOrUpdateDevice(Home model)
        {
            //foreach (var room in model.Rooms)
            //{
            //    if(room.SmartDevices != null)
            //    {
            //        Droom.ObjectState = ObjectState.Deleted;
            //        _roomRepository.Delete(room);
            //    }
            //}
        }

        private void DeleteAllDevices()
        {
            throw new NotImplementedException();
        }

        private void SaveOrUpdateRoom(Home model)
        {
            if (model.Rooms != null)
            {
                DeleteAllRooms(model.Rooms);
                InsertAllRooms(model);
            }
        }

        private void InsertAllRooms(Home home)
        {
            foreach (var room in _homeJsonEntity.Room)
            {
                var entity = Mapper.Map<RoomEntity, Room>(room);
                entity.Home = home;
                entity.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
                entity.ObjectState = ObjectState.Added;
                _roomRepository.Insert(entity);
            }
        }

        private void DeleteAllRooms(ICollection<Room> rooms)
        {
            foreach (var room in rooms)
            {
                room.ObjectState = ObjectState.Deleted;
                _roomRepository.Delete(room);
            }
        }


        private void SaveOrUpdateRouter(SmartRouterEntity router,Home home)
        {
            if (router == null)
            {
                InsertRouter(_homeJsonEntity.RouterInfo[0],home);
            }
            else
            {
                UpdateRouter(_homeJsonEntity.RouterInfo[0]);
            }
        }

        private Home SaveOrUpdateHome(HomeEntity home)
        {
            if (home == null)
            {
                return InsertHome(_homeJsonEntity.Home[0]);
            }
            else
            {
                return UpdateHome(_homeJsonEntity.Home[0]);
            }
            
        }

        #region Mapping
        private void SetMapper()
        {
            Mapper.CreateMap<HomeEntity, Home>();
            Mapper.CreateMap<SmartRouterEntity, SmartRouter>();
        }

        private Home MapHomeProperty(HomeEntity home)
        {
            Home model = _homeRepository
               .Queryable().Where(u => u.HomeId == home.HomeId).FirstOrDefault();

            model.Address1 = home.Address1;
            model.Address2 = home.Address2;
            model.AppsHomeId = home.AppsHomeId;
            model.AuditField.LastUpdatedBy = "admin";
            model.AuditField.LastUpdatedDateTime = DateTime.Now;
            model.Block = home.Block;
            model.HomeId = home.HomeId;
            model.City = home.City;
            model.Country = home.Country;
            model.IsActive = home.IsActive;
            model.IsDefault = home.IsDefault;
            model.IsInternet = home.IsInternet;
            model.IsSynced = home.IsSynced;
            model.MeshMode = (MeshModeType)home.MeshMode;
            model.PassPhrase = home.PassPhrase;
            model.Phone = home.Phone;
            model.TimeZone = home.TimeZone;
            model.ZipCode = home.ZipCode;

            return model;
        }

        private SmartRouter MapRouterProperty(SmartRouterEntity router)
        {
            SmartRouter model = _routerRepository
               .Queryable().Where(u => u.MacAddress == router.MacAddress).FirstOrDefault();

            model.MacAddress = router.MacAddress;
            model.Ssid = router.Ssid;
            model.AppsRouterId = router.AppsRouterId;
            model.AuditField.LastUpdatedBy = "admin";
            model.AuditField.LastUpdatedDateTime = DateTime.Now;
            model.IsSynced = router.IsSynced;
            model.LocalBrokerIp = router.LocalBrokerIp;
            model.LocalBrokerPassword = router.LocalBrokerPassword;
            model.LocalBrokerPort = router.LocalBrokerPort;
            model.LocalBrokerUsername = router.LocalBrokerUsername;

            return model;
        }
        #endregion
    }
}
