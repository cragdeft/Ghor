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
    }
}
