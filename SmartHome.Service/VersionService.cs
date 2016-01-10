
using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Service.Pattern;
using SmartHome.Entity;
using SmartHome.Model.Models;
using SmartHome.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{


    public class VersionService : Service<Version>, IVersionService
    {
        private readonly IRepositoryAsync<Version> _repository;

        public VersionService(IRepositoryAsync<Version> repository) : base(repository)
        {
            _repository = repository;
            Mapper.CreateMap<VersionEntity, Version>();
        }

        public bool IsExists(int key)
        {
            return base.Query(e => e.VersionId == key).Select().Any();
        }

        public async Task<IEnumerable<VersionEntity>> GetsAsync()
        {
            try
            {
                Mapper.CreateMap<Version, VersionEntity>();
                var syncList = await _repository.Query().SelectAsync();
                return Mapper.Map<IEnumerable<Version>, IEnumerable<VersionEntity>>(syncList);
            }
            catch (System.Exception ex)
            {

                throw;
            }


        }

        public async Task<VersionEntity> GetAsync(int Id)
        {
            var SyncList = await _repository.FindAsync(Id);
            return Mapper.Map<Version, VersionEntity>(SyncList);
        }

        public VersionEntity Add(VersionEntity entity)
        {
            Version model = Mapper.Map<VersionEntity, Version>(entity);
            model.AuditField = new AuditFields();
            model.ObjectState = ObjectState.Added;
            base.Insert(model);
            return entity;
        }

        public VersionEntity Modify(VersionEntity entity)
        {
            try
            {
                Version model = Mapper.Map<VersionEntity, Version>(entity);
                model.AuditField = new AuditFields();
                model.ObjectState = ObjectState.Modified;
                base.Update(model);
                return entity;
            }
            catch (System.Exception ex)
            {

                throw;
            }
        }

        public void Remove(string id)
        {

            base.Delete(id);
        }

        public void Remove(VersionEntity entity)
        {
            Version model = Mapper.Map<VersionEntity, Version>(entity);
            model.AuditField = new AuditFields();
            model.ObjectState = ObjectState.Deleted;
            base.Delete(model);
        }

       


    }
}
