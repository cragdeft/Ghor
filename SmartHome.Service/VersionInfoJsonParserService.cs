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
using Version = SmartHome.Model.Models.Version;

namespace SmartHome.Service
{
    public class VersionInfoJsonParserService : IHomeJsonParserService<Version>
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<Home> _homeRepository;
        private readonly IRepositoryAsync<Version> _versionRepository;
        private readonly IRepositoryAsync<VersionDetail> _versionDetailRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }

        #endregion
        public VersionInfoJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();
            _versionRepository = _unitOfWorkAsync.RepositoryAsync<Version>();
            _versionDetailRepository = _unitOfWorkAsync.RepositoryAsync<VersionDetail>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
        }
        public Version SaveJsonData()
        {
            Version version = null;
            try
            {
                if (_homeJsonEntity.Home.Count == 0)
                {
                    return version;
                }

                string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;

                HomeEntity homeEntity = _homeJsonEntity.Home.FirstOrDefault();
                Home dbHome = new CommonService(_unitOfWorkAsync).GetHome(passPhrase);

                if (dbHome != null)
                {
                    SaveOrUpdateVersion(dbHome);
                }



            }
            catch (Exception ex)
            {
                return null;
            }
            return version;
        }


        public void SaveOrUpdateVersion(Home home)
        {
            DeleteVersion(home);
            List<VersionEntity> versionEntityList = _homeJsonEntity.Version;

            foreach (var versionEntity in versionEntityList)
            {
                List<VersionDetailEntity> versionDetails =
                    _homeJsonEntity.VersionDetails.FindAll(x => x.AppsVersionId == versionEntity.AppsVersionId);
                InsertVersion(home, versionEntity, versionDetails);
            }
        }

        private void DeleteVersion(Home home)
        {
            Version version = _versionRepository.Queryable().Where(w => w.Home.HomeId == home.HomeId).FirstOrDefault();
            if (version != null)
            {
                version.ObjectState = ObjectState.Deleted;
                _versionRepository.Delete(version);
            }
        }

        private void InsertVersion(Home home, VersionEntity versionEntity, List<VersionDetailEntity> versionDetails)
        {
            Mapper.CreateMap<VersionEntity, Version>();
            Version version = Mapper.Map<VersionEntity, Version>(versionEntity);
            version.Home = home;
            version.IsSynced = Convert.ToBoolean(versionEntity.IsJsonSynced);
            version.ObjectState = ObjectState.Added;
            version.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
            _versionRepository.Insert(version);

            InsertVersionDetails(version, versionDetails);
        }

        private void InsertVersionDetails(Version version, List<VersionDetailEntity> versionDetailsEntity)
        {
            Mapper.CreateMap<VersionDetailEntity, VersionDetail>();
            foreach (var verDetail in versionDetailsEntity)
            {
                VersionDetail versionDetail = Mapper.Map<VersionDetailEntity, VersionDetail>(verDetail);
                versionDetail.IsSynced = Convert.ToBoolean(verDetail.IsJsonSynced);
                versionDetail.ObjectState = ObjectState.Added;
                versionDetail.AuditField = new AuditFields("admin", DateTime.Now, "admin", DateTime.Now);
                _versionDetailRepository.Insert(versionDetail);
                version.VersionDetails.Add(versionDetail);
            }
        }

    }
}
