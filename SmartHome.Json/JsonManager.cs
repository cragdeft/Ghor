
using AutoMapper;
using Newtonsoft.Json;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Json.Entity;
using SmartHome.Model.Enums;
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


                IEnumerable<Model.Models.Device> oDevice = ConfigureDevice(oRootObject);
                IEnumerable<Model.Models.Channel> oChannel = ConfigureChannel(oRootObject);
                IEnumerable<Model.Models.DeviceStatus> oDeviceStatus = ConfigureDeviceStatus(oRootObject);
                StoreDeviceAndChannel(oDevice, oChannel, oDeviceStatus);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #region Store
        private static void StoreDeviceAndChannel(IEnumerable<Model.Models.Device> oDevice, IEnumerable<Model.Models.Channel> oChannel, IEnumerable<Model.Models.DeviceStatus> oDeviceStatus)
        {

            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                foreach (var item in oDevice)
                {
                    IRepositoryAsync<Model.Models.Device> versionRepository = new Repository<Model.Models.Device>(context, unitOfWork);
                    item.Channels = oChannel.Where(p => p.DId == item.Id).ToArray();
                    item.DeviceStatus = oDeviceStatus.Where(p => p.DId == item.Id).ToArray();
                    versionRepository.InsertOrUpdateGraph(item);
                    unitOfWork.SaveChanges();
                }
            }

        }

        private static void StoreVersionAndVersionDetail(IEnumerable<Model.Models.Version> oVersion, IEnumerable<Model.Models.VersionDetail> oVersionDetail)
        {
            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                foreach (var item in oVersion)
                {
                    IRepositoryAsync<Model.Models.Version> versionRepository = new Repository<Model.Models.Version>(context, unitOfWork);
                    item.VersionDetails = oVersionDetail.Where(p => p.VId == item.Id).ToArray();
                    versionRepository.InsertOrUpdateGraph(item);
                    unitOfWork.SaveChanges();
                }
            }
        }
        #endregion

        private static RootObject JsonDesrialized(string JsonString)
        {
            return JsonConvert.DeserializeObject<RootObject>(JsonString);
        }

        #region Version Conversion
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

        #endregion


        #region Device conversion
        private static IEnumerable<Model.Models.Device> ConfigureDevice(RootObject myObj)
        {
            Mapper.CreateMap<Entity.Device, Model.Models.Device>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.DeviceVersion, opt => opt.MapFrom(src => src.Version))
            .ForMember(dest => dest.DType, opt => opt.MapFrom(src => src.DeviceType))
            //.ForMember(dest => dest.DType, opt => opt.MapFrom(src => (int)GetEnumValue<DeviceType>(src.DeviceType)))//enum
            .ForMember(dest => dest.DeviceType, opt => opt.UseValue(58))
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state
            IEnumerable<Model.Models.Device> oDevice = Mapper.Map<IEnumerable<Entity.Device>, IEnumerable<Model.Models.Device>>(myObj.Device);
            return oDevice;
        }

        private static IEnumerable<Model.Models.Channel> ConfigureChannel(RootObject myObj)
        {


            Mapper.CreateMap<Entity.ChannelConfig, Model.Models.Channel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.DId, opt => opt.MapFrom(src => src.DeviceID))
            //.ForMember(dest => dest.LoadType, opt => opt.UseValue(58))
            //.ForMember(dest => dest.LoadType, opt => opt.MapFrom(src => (int)GetEnumValue<LoadType>(src.LoadType)))//enum
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state
            IEnumerable<Model.Models.Channel> oDevice = Mapper.Map<IEnumerable<Entity.ChannelConfig>, IEnumerable<Model.Models.Channel>>(myObj.ChannelConfig);
            return oDevice;
        }

        private static IEnumerable<Model.Models.DeviceStatus> ConfigureDeviceStatus(RootObject myObj)
        {
            Mapper.CreateMap<Entity.DeviceStatus, Model.Models.DeviceStatus>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.DId, opt => opt.MapFrom(src => src.DeviceID))
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state
            IEnumerable<Model.Models.DeviceStatus> oDeviceStatus = Mapper.Map<IEnumerable<Entity.DeviceStatus>, IEnumerable<Model.Models.DeviceStatus>>(myObj.DeviceStatus);
            return oDeviceStatus;
        }
        #endregion


        #region Utility

        public static TDestination Resolve<TSource, TDestination>(TSource source)
        {
            var mapped = Mapper.FindTypeMapFor(typeof(TSource), typeof(TDestination)); //this will give you a reference to existing mapping if it was created or NULL if not

            if (mapped == null)
            {
                var expression = Mapper.CreateMap<TSource, TDestination>();
            }
            return Mapper.Map<TSource, TDestination>(source);
        }

        public static T GetEnumValue<T>(string str) where T : struct, IConvertible
        {
            //(int)((DeviceType)Enum.Parse(typeof(DeviceType), "SMART_SWITCH_6G"));
            if (!typeof(T).IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }
            T val = ((T[])Enum.GetValues(typeof(T)))[0];
            if (!string.IsNullOrEmpty(str))
            {
                foreach (T enumValue in (T[])Enum.GetValues(typeof(T)))
                {
                    if (enumValue.ToString().ToUpper().Equals(str.ToUpper()))
                    {
                        val = enumValue;
                        break;
                    }
                }
            }

            return val;
        }
        #endregion
    }
}
