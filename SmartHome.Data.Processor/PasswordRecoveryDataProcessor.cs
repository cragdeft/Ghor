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
  public class PasswordRecoveryDataProcessor : BaseDataProcessor
  {
    public dynamic _userEntity { get; private set; }
    public PasswordRecoveryRootObjectEntity _oRootObject { get; set; }

    public PasswordRecoveryDataProcessor(string jsonString, MessageReceivedFrom receivedFrom, PasswordRecoveryRootObjectEntity oRootObject)
    {
      _receivedFrom = receivedFrom;
      _homeJsonMessage = jsonString;
      _userEntity = JsonDesrialized<dynamic>(jsonString);
      _oRootObject = oRootObject;
    }
    public bool PasswordRecovery()
    {
      bool isSuccess = false;


      if (_userEntity == null)
        return false;


      isSuccess = IsUserAlreadyExist();

      using (IDataContextAsync context = new SmartHomeDataContext())
      using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
      {

        var transactionRunner = new UnitOfWorkTransactionRunner(unitOfWork);
        try
        {

          if (isSuccess)
          {
            _oRootObject.data.Password = transactionRunner.RunSelectTransaction(() => new UserInfoService(unitOfWork).PasswordRecoveryByEmail(Convert.ToString(_userEntity.Email)));
            _oRootObject.data.UserName = transactionRunner.RunSelectTransaction(() => new UserInfoService(unitOfWork).GetsUserInfosByEmailAndPassword(Convert.ToString(_userEntity.Email), _oRootObject.data.Password).UserName);
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
      return isSuccess;
    }

    private bool IsUserAlreadyExist()
    {
      bool isUserExist;
      using (IDataContextAsync context = new SmartHomeDataContext())
      using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
      {

        var transactionRunner = new UnitOfWorkTransactionRunner(unitOfWork);

        isUserExist = transactionRunner.RunSelectTransaction(() => new CommonService(unitOfWork).IsLoginIdUnique(Convert.ToString(_userEntity.Email)));
      }

      return isUserExist;
    }
  }
}
