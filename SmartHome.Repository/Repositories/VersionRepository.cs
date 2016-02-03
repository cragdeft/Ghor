using Repository.Pattern.Repositories;
using SmartHome.Entity;
using SmartHome.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Repository.Repositories
{
    public static class VersionRepository
    {

        public static IEnumerable<VersionInfoEntity> GetsAllVersion(this IRepositoryAsync<Version> repository)
        {
            int parentSequence = 0;
            List<VersionInfoEntity> vInfoEntity = new List<VersionInfoEntity>();
            var version = repository.Query().Include(x => x.VersionDetails).Select().ToList();
            foreach (Version nextVersion in version)
            {
                //AppName,AuthCode,PassPhrase
                VersionInfoEntity versionInfo = new VersionInfoEntity();
                versionInfo.DisplayName = "AppName--" + nextVersion.AppName + " AuthCode--" + nextVersion.AuthCode + " PassPhrase--" + nextVersion.PassPhrase;
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
