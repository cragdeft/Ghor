using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Logging;
using SmartHome.Model.Enums;
using SmartHome.Model.ModelDataContext;
using SmartHome.Model.Models;
using SmartHome.Service;
using SmartHome.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Data.Processor
{
  public class RegisterUserDataProcessor : BaseDataProcessor
  {
    public LoginObjectEntity _userEntity { get; private set; }

    public RegisterUserDataProcessor(string jsonString, MessageReceivedFrom receivedFrom)
    {
      _receivedFrom = receivedFrom;
      _homeJsonMessage = jsonString;
      _userEntity = JsonDesrialized<LoginObjectEntity>(jsonString);
    }
    public bool RegisterUser()
    {
      bool isRegisterSucces = false;

      if (_userEntity == null)
        return false;

      using (IDataContextAsync context = new SmartHomeDataContext())
      using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
      {

        var transactionRunner = new UnitOfWorkTransactionRunner(unitOfWork);
        try
        {

          bool isUserExist = transactionRunner.RunSelectTransaction(() => new CommonService(unitOfWork).IsLoginIdUnique(_userEntity.UserInfo.FirstOrDefault().Email));

          if (isUserExist)
          {
            isRegisterSucces = false;
          }
          else
          {
            transactionRunner.RunTransaction(() => new UserInfoService(unitOfWork).Add(_userEntity.UserInfo.FirstOrDefault()));
            isRegisterSucces = true;
          }
        }
        catch (Exception ex)
        {
          return false;
        }
        finally
        {
        }
      }
      return isRegisterSucces;
    }
  }


}
