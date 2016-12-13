using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Logging;
using SmartHome.Model.Enums;
using SmartHome.Model.ModelDataContext;
using SmartHome.Model.Models;
using SmartHome.Service;
using SmartHome.Service.UserInfoServices;
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
      bool isRegisterSucces = true;      

      if (_userEntity == null)
        return false;      

      using (IDataContextAsync context = new SmartHomeDataContext())
      using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
      {

        var transactionRunner = new UnitOfWorkTransactionRunner(unitOfWork);
        try
        {
          UserInfo oUserInfo = transactionRunner.RunTransaction(() => new UserInfoRegisterService(unitOfWork, _userEntity).SaveData());
          if (oUserInfo ==null)
          {
            isRegisterSucces = false;
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
