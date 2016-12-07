using SmartHome.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Model.Models;
using Repository.Pattern.UnitOfWork;
using Repository.Pattern.Repositories;
using SmartHome.Entity;
using AutoMapper;
using Repository.Pattern.Infrastructure;

namespace SmartHome.Service.UserInfoServices
{
  public class UserInfoRegisterService : IUserInfoAdditionService<UserInfo>
  {
    #region PrivateProperty
    private readonly IUnitOfWorkAsync _unitOfWorkAsync;
    private readonly IRepositoryAsync<UserInfo> _usreInfoRepository;

    public LoginObjectEntity _loginObjectEntity { get; private set; }

    #endregion
    public UserInfoRegisterService(IUnitOfWorkAsync unitOfWorkAsync, LoginObjectEntity loginObjectEntity)
    {
      _unitOfWorkAsync = unitOfWorkAsync;
      _loginObjectEntity = loginObjectEntity;
      _usreInfoRepository = _unitOfWorkAsync.RepositoryAsync<UserInfo>();
    }

    public UserInfo SaveData()
    {
      //bool isNew = new CommonService(_unitOfWorkAsync).IsLoginIdUnique(_loginObjectEntity.UserInfo.FirstOrDefault().Email);

      //if (!isNew)
      //{
      return AddUserInfo(_loginObjectEntity.UserInfo.FirstOrDefault());
      //}
      //else
      //{
      //return null;
      //}

    }

    private UserInfo AddUserInfo(UserInfoEntity entity)
    {
      Mapper.CreateMap<UserInfoEntity, UserInfo>()
      .ForMember(dest => dest.AuditField, opt => opt.UseValue(new AuditFields("Admin", DateTime.Now, "Admin", DateTime.Now)))
      .ForMember(dest => dest.ObjectState, opt => opt.UseValue(ObjectState.Added));
      UserInfo model = Mapper.Map<UserInfoEntity, UserInfo>(entity);

      _usreInfoRepository.Insert(model);
      return model;

    }

  }
}
