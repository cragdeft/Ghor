
using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Service.Pattern;
using SmartHome.Entity;
using SmartHome.Model.Models;
using SmartHome.Repository.Repositories;
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
            // Mapper.CreateMap<VersionEntity, Version>();
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
            Mapper.CreateMap<Version, VersionEntity>();
            var SyncList = await _repository.FindAsync(Id);
            return Mapper.Map<Version, VersionEntity>(SyncList);
        }

        public VersionEntity Add(VersionEntity entity)
        {
            Mapper.CreateMap<VersionEntity, Version>();
            Version model = Mapper.Map<VersionEntity, Version>(entity);
            model.AuditField = new AuditFields();
            model.ObjectState = ObjectState.Added;
            base.Insert(model);
            return entity;
        }

        #region AddOrUpdateGraphRange
        public IEnumerable<Version> AddOrUpdateGraphRange(IEnumerable<Version> model)
        {
            List<Version> versionModel = new List<Version>();
            versionModel = FillVersionInformations(model, versionModel);
            base.InsertOrUpdateGraphRange(versionModel);
            return versionModel;
        }


        public List<Version> FillVersionInformations(IEnumerable<Version> model, List<Version> versionModel)
        {
            foreach (var item in model)
            {
                //check already exist or not.
                IEnumerable<Version> temp = IsVersionExists(item.Id, item.Mac);
                if (temp.Count() == 0)
                {
                    //new item
                    versionModel.Add(item);
                    continue;
                }
                else
                {
                    //existing item               
                    // versionModel = temp;
                    foreach (var existingItem in temp.ToList())
                    {
                        //modify version                    
                        FillExistingVersionInfo(item, existingItem);

                        if (item.VersionDetails != null && item.VersionDetails.Count > 0)
                        {
                            AddOrEditExistingVDetailItems(item, existingItem);
                        }

                    }
                }
            }

            return versionModel;
        }



        private void AddOrEditExistingVDetailItems(Version item, Version existingItem)
        {
            foreach (var nextVDetail in item.VersionDetails)
            {
                var tempExistingVDetail = existingItem.VersionDetails.Where(p => p.Id == nextVDetail.Id).FirstOrDefault();
                if (tempExistingVDetail != null)
                {
                    //modify
                    FillExistingVDetailInfo(nextVDetail, tempExistingVDetail);
                }
                else
                {
                    //add
                    existingItem.VersionDetails.Add(nextVDetail);
                }
            }
        }

        private void FillExistingVDetailInfo(VersionDetail nextVDetail, VersionDetail tempExistingVDetail)
        {
            tempExistingVDetail.ObjectState = ObjectState.Modified;
            tempExistingVDetail.Id = nextVDetail.Id;
            tempExistingVDetail.VId = nextVDetail.VId;
            tempExistingVDetail.HardwareVersion = nextVDetail.HardwareVersion;
            tempExistingVDetail.DeviceType = nextVDetail.DeviceType;
            tempExistingVDetail.AuditField = new AuditFields();
        }

        private void FillExistingVersionInfo(Version item, Version existingItem)
        {
            existingItem.AppName = item.AppName;
            existingItem.AppVersion = item.AppVersion;
            existingItem.AuditField = new AuditFields();
            existingItem.AuthCode = item.AuthCode;
            existingItem.Mac = item.Mac;
            existingItem.Id = item.Id;
            existingItem.ObjectState = ObjectState.Modified;
        }

        private IEnumerable<Version> IsVersionExists(string key, string Mac)
        {
            return _repository.Query(e => e.Id == key && e.Mac == Mac).Include(x => x.VersionDetails).Select();
        }



        #endregion

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



        public IEnumerable<VersionInfoEntity> GetsAllVersion()
        {
            // add business logic here
            // return _repository.GetsAllVersion();

            int parentSequence = 0;
            List<VersionInfoEntity> vInfoEntity = new List<VersionInfoEntity>();
            var version = _repository.Query().Include(x => x.VersionDetails).Select().ToList();
            foreach (Version nextVersion in version)
            {
                //AppName,AuthCode,PassPhrase
                VersionInfoEntity versionInfo = new VersionInfoEntity();
                versionInfo.DisplayName = "AppName--" + nextVersion.AppName + ", AuthCode--" + nextVersion.AuthCode + ", PassPhrase--" + nextVersion.PassPhrase;
                versionInfo.ParentId = 0;
                versionInfo.SequenceId = vInfoEntity.Count() + 1;
                parentSequence = versionInfo.SequenceId;
                vInfoEntity.Add(versionInfo);
                foreach (VersionDetail nextVDetail in nextVersion.VersionDetails)
                {
                    versionInfo = new VersionInfoEntity();
                    versionInfo.DisplayName = "DeviceType--" + nextVDetail.DeviceType;
                    versionInfo.ParentId = parentSequence;
                    versionInfo.SequenceId = vInfoEntity.Count() + 1;
                    vInfoEntity.Add(versionInfo);
                }
            }
            return vInfoEntity;
        }


    }
}
