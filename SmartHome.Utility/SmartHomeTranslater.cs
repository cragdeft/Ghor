using AutoMapper;
using SmartHome.Entity;
using SmartHome.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Utility
{
    public class SmartHomeTranslater
    {
        public static T MapSmartDeviceProperties<T>(SmartDeviceEntity entity, T smartDevice) where T : SmartDevice
        {
            Mapper.CreateMap<SmartDeviceEntity, T>()
              .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == 1 ? true : false))
              .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted == 1 ? true : false))
              .ForMember(x => x.Room, y => y.Ignore())
              .ForMember(x => x.DeviceId, y => y.Ignore());

            return Mapper.Map<SmartDeviceEntity, T>(entity, smartDevice);
        }

        public static UserInfo MapUserInfoProperties(UserInfoEntity entity, UserInfo userInfo)
        {
            Mapper.CreateMap<UserInfoEntity, UserInfo>()
              .ForMember(dest => dest.LoginStatus, opt => opt.MapFrom(src => src.LoginStatus == 1 ? true : false))
              .ForMember(dest => dest.RegStatus, opt => opt.MapFrom(src => src.RegStatus == 1 ? true : false))
              .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == 1 ? true : false))
              .ForMember(x => x.UserInfoId, y => y.Ignore());

            return Mapper.Map<UserInfoEntity, UserInfo>(entity, userInfo);
        }

        public static Channel MapChannelInfoProperties(ChannelEntity entity, Channel channel)
        {
            Mapper.CreateMap<ChannelEntity, Channel>()
              .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == 1 ? true : false))
              .ForMember(x => x.ChannelStatuses, y => y.Ignore())
              .ForMember(x => x.ChannelId, y => y.Ignore());

            return Mapper.Map<ChannelEntity, Channel>(entity, channel);
        }

        public static RouterInfo MapRouterInfoProperties(RouterInfoEntity entity, RouterInfo router)
        {
            Mapper.CreateMap<RouterInfoEntity, RouterInfo>()
              .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == 1 ? true : false))
              .ForMember(dest => dest.IsExternal, opt => opt.MapFrom(src => src.IsExternal == 1 ? true : false))
              .ForMember(x => x.Parent, y => y.Ignore())
              .ForMember(x => x.RouterInfoId, y => y.Ignore());

            return Mapper.Map<RouterInfoEntity, RouterInfo>(entity, router);
        }


        public static WebBrokerInfo MapWebBrokerInfoProperties(WebBrokerInfoEntity entity, WebBrokerInfo webBroker)
        {
            Mapper.CreateMap<WebBrokerInfoEntity, WebBrokerInfo>()
              .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == 1 ? true : false))
              .ForMember(x => x.Parent, y => y.Ignore())
              .ForMember(x => x.WebBrokerInfoId, y => y.Ignore());

            return Mapper.Map<WebBrokerInfoEntity, WebBrokerInfo>(entity, webBroker);
        }

        public static Home MapHomeInfoProperties(HomeEntity entity, Home home)
        {
            Mapper.CreateMap<HomeEntity, Home>()
              .ForMember(dest => dest.IsInternet, opt => opt.MapFrom(src => src.IsInternet == 1 ? true : false))
              .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(src => src.IsDefault == 1 ? true : false))
              .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == 1 ? true : false))
              .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == 1 ? true : false))
              .ForMember(x => x.HomeId, y => y.Ignore())
              .ForMember(x => x.Rooms, y => y.Ignore());

            return Mapper.Map<HomeEntity, Home>(entity, home);
        }
    }
}
