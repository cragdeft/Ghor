using Repository.Pattern.Infrastructure;
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

        //public static IEnumerable<VersionInfoEntity> GetsAllVersion(this IRepositoryAsync<Version> repository)
        //{
        //    int parentSequence = 0;
        //    List<VersionInfoEntity> vInfoEntity = new List<VersionInfoEntity>();
        //    var version = repository.Query().Include(x => x.VersionDetails).Select().ToList();
        //    foreach (Version nextVersion in version)
        //    {
        //        //AppName,AuthCode,PassPhrase
        //        VersionInfoEntity versionInfo = new VersionInfoEntity();
        //        versionInfo.DisplayName = "AppName--" + nextVersion.AppName + ", AuthCode--" + nextVersion.AuthCode + ", PassPhrase--" + nextVersion.PassPhrase;
        //        versionInfo.ParentId = 0;
        //        versionInfo.SequenceId = vInfoEntity.Count() + 1;
        //        parentSequence = versionInfo.SequenceId;
        //        vInfoEntity.Add(versionInfo);

        //        foreach (VersionDetail nextVDetail in nextVersion.VersionDetails)
        //        {
        //            versionInfo = new VersionInfoEntity();
        //            versionInfo.DisplayName = "DeviceType--" + nextVDetail.DeviceType;
        //            versionInfo.ParentId = parentSequence;
        //            versionInfo.SequenceId = vInfoEntity.Count() + 1;
        //            vInfoEntity.Add(versionInfo);
        //        }
        //    }
        //    return vInfoEntity;
        //}

        //public static List<Version> FillVersionInformations(this IRepositoryAsync<Version> repository, IEnumerable<Version> model, List<Version> versionModel)
        //{
        //    foreach (var item in model)
        //    {
        //        //check already exist or not.
        //        IEnumerable<Version>  temp = IsVersionExists(item.Id, item.Mac, repository);
        //        if (temp.Count() == 0)
        //        {
        //            //new item
        //            versionModel.Add(item);
        //            continue;
        //        }
        //        else
        //        {
        //            //existing item               
        //           // versionModel = temp;
        //            foreach (var existingItem in temp.ToList())
        //            {
        //                //modify version                    
        //                FillExistingVersionInfo(item, existingItem);

        //                if (item.VersionDetails != null && item.VersionDetails.Count > 0)
        //                {
        //                    AddOrEditExistingVDetailItems(item, existingItem);
        //                }

        //            }
        //        }
        //    }

        //    return versionModel;
        //}

        

        //private static void AddOrEditExistingVDetailItems(Version item, Version existingItem)
        //{
        //    foreach (var nextVDetail in item.VersionDetails)
        //    {
        //        var tempExistingVDetail = existingItem.VersionDetails.Where(p => p.Id == nextVDetail.Id).FirstOrDefault();
        //        if (tempExistingVDetail != null)
        //        {
        //            //modify
        //            FillExistingVDetailInfo(nextVDetail, tempExistingVDetail);
        //        }
        //        else
        //        {
        //            //add
        //            existingItem.VersionDetails.Add(nextVDetail);
        //        }
        //    }
        //}

        //private static void FillExistingVDetailInfo(VersionDetail nextVDetail, VersionDetail tempExistingVDetail)
        //{
        //    tempExistingVDetail.ObjectState = ObjectState.Modified;
        //    tempExistingVDetail.Id = nextVDetail.Id;
        //    tempExistingVDetail.VId = nextVDetail.VId;
        //    tempExistingVDetail.HardwareVersion = nextVDetail.HardwareVersion;
        //    tempExistingVDetail.DeviceType = nextVDetail.DeviceType;
        //    tempExistingVDetail.AuditField = new AuditFields();
        //}

        //private static void FillExistingVersionInfo(Version item, Version existingItem)
        //{
        //    existingItem.AppName = item.AppName;
        //    existingItem.AppVersion = item.AppVersion;
        //    existingItem.AuditField = new AuditFields();
        //    existingItem.AuthCode = item.AuthCode;
        //    existingItem.Mac = item.Mac;
        //    existingItem.Id = item.Id;
        //    existingItem.ObjectState = ObjectState.Modified;
        //}

        //private static IEnumerable<Version> IsVersionExists(string key, string Mac, IRepositoryAsync<Model.Models.Version> repository)
        //{
        //    return repository.Query(e => e.Id == key && e.Mac == Mac).Include(x => x.VersionDetails).Select();
        //}


    }
}
