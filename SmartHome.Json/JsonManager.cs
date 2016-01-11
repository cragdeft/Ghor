
using AutoMapper;
using Newtonsoft.Json;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Json.Entity;
using SmartHome.Model.ModelDataContext;
using SmartHome.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Json
{
    public static class JsonManager
    {
        public static void JsonProcess(string JsonString)
        {

            #region MyRegion

            RootObject myObj = JsonConvert.DeserializeObject<RootObject>(JsonString);
            Mapper.CreateMap<RootObject, Model.Models.Version>();

             // Mapper.Map<IEnumerable<myObj.Version>, IEnumerable<Model.Models.Version>>(syncList);
            foreach (var item in myObj.Version)
            {
                // Model.Models.Version model = Mapper.Map<Entity.Version, Model.Models.Version>(item);


                using (IDataContextAsync context = new SmartHomeDataContext())
                using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
                {
                    IRepositoryAsync<Model.Models.Version> versionRepository = new Repository<Model.Models.Version>(context, unitOfWork);

                    var Version = new Model.Models.Version
                    {
                        AppName = item.AppName,
                        AppVersion = item.AppVersion,
                        Id = item.ID,
                        AuthCode=item.AuthCode,
                        PassPhrase=item.PassPhrase,
                        AuditField = new AuditFields(),
                        ObjectState = ObjectState.Added
                    };
                    versionRepository.Insert(Version);
                    unitOfWork.SaveChanges();
                }

            }



            // Create new customer
            //using (IDataContextAsync context = new SmartHomeDataContext())
            //using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            //{
            //    IRepositoryAsync<Model.Models.Version> versionRepository = new Repository<Model.Models.Version>(context, unitOfWork);

            //    var Version = new Model.Models.Version
            //    {
            //        AppName = "App 3",
            //        AppVersion = "1.3",
            //        AuditField = new AuditFields(),
            //        ObjectState = ObjectState.Added
            //    };

            //    versionRepository.Insert(Version);
            //    unitOfWork.SaveChanges();
            //}
            #endregion
        }
    }
}
