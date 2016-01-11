
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
            try
            {
                RootObject oRootObject = JsonDesrialized(JsonString);
                IEnumerable<Model.Models.Version> oVersion = ConfigureVersion(oRootObject);
                IEnumerable<Model.Models.VersionDetail> oVersionDetail = ConfigureVersionDetail(oRootObject);
                StoreVersionAndVersionDetail(oVersion, oVersionDetail);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static RootObject JsonDesrialized(string JsonString)
        {
            return JsonConvert.DeserializeObject<RootObject>(JsonString);
        }

        private static void StoreVersionAndVersionDetail(IEnumerable<Model.Models.Version> oVersion, IEnumerable<Model.Models.VersionDetail> oVersionDetail)
        {
            foreach (var item in oVersion)
            {
                using (IDataContextAsync context = new SmartHomeDataContext())
                using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
                {
                    IRepositoryAsync<Model.Models.Version> versionRepository = new Repository<Model.Models.Version>(context, unitOfWork);
                    item.VersionDetails = oVersionDetail.Where(p => p.VId == item.Id).ToArray();
                    versionRepository.InsertOrUpdateGraph(item);
                    unitOfWork.SaveChanges();
                }
            }
        }

        private static IEnumerable<Model.Models.VersionDetail> ConfigureVersionDetail(RootObject myObj)
        {
            Mapper.CreateMap<Entity.VersionDetail, Model.Models.VersionDetail>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added))//state
            .ForMember(dest => dest.VId, opt => opt.MapFrom(src => src.VersionID));
            IEnumerable<Model.Models.VersionDetail> oVersionDetail = Mapper.Map<IEnumerable<Entity.VersionDetail>, IEnumerable<Model.Models.VersionDetail>>(myObj.VersionDetails);
            return oVersionDetail;
        }

        private static IEnumerable<Model.Models.Version> ConfigureVersion(RootObject myObj)
        {
            Mapper.CreateMap<Entity.Version, Model.Models.Version>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
                        .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))//audit field
                        .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state
            IEnumerable<Model.Models.Version> oVersion = Mapper.Map<IEnumerable<Entity.Version>, IEnumerable<Model.Models.Version>>(myObj.Version);
            return oVersion;
        }

        public static TDestination Resolve<TSource, TDestination>(TSource source)
        {
            var mapped = Mapper.FindTypeMapFor(typeof(TSource), typeof(TDestination)); //this will give you a reference to existing mapping if it was created or NULL if not

            if (mapped == null)
            {
                var expression = Mapper.CreateMap<TSource, TDestination>();
            }
            return Mapper.Map<TSource, TDestination>(source);
        }
    }
}
