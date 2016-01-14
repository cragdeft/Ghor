using AutoMapper;
using Newtonsoft.Json;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.ModelDataContext;
using SmartHome.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartHome.Json
{
    public static class JsonManager
    {
        public static void JsonProcess(string JsonString)
        {
            try
            {

                RootObjectEntity  oRootObject = JsonDesrialized(JsonString);

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


        #region Version Conversion
        private static IEnumerable<Model.Models.VersionDetail> ConfigureVersionDetail(RootObjectEntity oRootObject)
        {
            Mapper.CreateMap<VersionDetailEntity, Model.Models.VersionDetail>()
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state
           // .ForMember(dest => dest.VId, opt => opt.MapFrom(src => src.VersionID));
            IEnumerable<Model.Models.VersionDetail> oVersionDetail = Mapper.Map<IEnumerable<Entity.VersionDetailEntity>, IEnumerable<Model.Models.VersionDetail>>(oRootObject.VersionDetails);
            return oVersionDetail;
        }

        private static IEnumerable<Model.Models.Version> ConfigureVersion(RootObjectEntity myObj)
        {
            Mapper.CreateMap<VersionEntity, Model.Models.Version>()
                        //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
                        .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))//audit field
                        .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state
            IEnumerable<Model.Models.Version> oVersion = Mapper.Map<IEnumerable<VersionEntity>, IEnumerable<Model.Models.Version>>(myObj.Version);
            return oVersion;
        }



        #endregion


        #region Device conversion
        private static IEnumerable<Model.Models.Device> ConfigureDevice(RootObjectEntity myObj)
        {
            Mapper.CreateMap<DeviceEntity, Model.Models.Device>()
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
            //.ForMember(dest => dest.DeviceVersion, opt => opt.MapFrom(src => src.Version))
            //.ForMember(dest => dest.DType, opt => opt.MapFrom(src => src.DeviceType))
            //.ForMember(dest => dest.DType, opt => opt.MapFrom(src => (int)GetEnumValue<DeviceType>(src.DeviceType)))//enum
            .ForMember(dest => dest.DeviceType, opt => opt.UseValue(58))
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state
            IEnumerable<Model.Models.Device> oDevice = Mapper.Map<IEnumerable<DeviceEntity>, IEnumerable<Model.Models.Device>>(myObj.Device);
            return oDevice;
        }

        private static IEnumerable<Model.Models.Channel> ConfigureChannel(RootObjectEntity myObj)
        {

            Mapper.CreateMap<ChannelEntity, Model.Models.Channel>()
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
            //.ForMember(dest => dest.DId, opt => opt.MapFrom(src => src.DeviceID))
            //.ForMember(dest => dest.LoadType, opt => opt.UseValue(58))
            //.ForMember(dest => dest.LoadType, opt => opt.MapFrom(src => (int)GetEnumValue<LoadType>(src.LoadType)))//enum
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state
            IEnumerable<Model.Models.Channel> oDevice = Mapper.Map<IEnumerable<ChannelEntity>, IEnumerable<Model.Models.Channel>>(myObj.Channel);
            return oDevice;
        }

        private static IEnumerable<Model.Models.DeviceStatus> ConfigureDeviceStatus(RootObjectEntity myObj)
        {
            Mapper.CreateMap<DeviceStatusEntity, Model.Models.DeviceStatus>()
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
            //.ForMember(dest => dest.DId, opt => opt.MapFrom(src => src.DeviceID))
            .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields()))
            .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));//state
            IEnumerable<Model.Models.DeviceStatus> oDeviceStatus = Mapper.Map<IEnumerable<DeviceStatusEntity>, IEnumerable<Model.Models.DeviceStatus>>(myObj.DeviceStatus);
            return oDeviceStatus;
        }
        #endregion

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

        private static RootObjectEntity JsonDesrialized(string JsonString)
        {
            return JsonConvert.DeserializeObject<RootObjectEntity>(JsonString);
        }








    }
}
