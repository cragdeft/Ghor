using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
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
  public class PasswordDataProcessor : BaseDataProcessor
  {
    public dynamic _userEntity { get; private set; }

    public PasswordDataProcessor(string jsonString, MessageReceivedFrom receivedFrom)
    {
      _receivedFrom = receivedFrom;
      _homeJsonMessage = jsonString;
      _userEntity = JsonDesrialized<dynamic>(jsonString);
    }


    public bool ChangePassword()
    {
      bool isSuccess = false;

      if (_userEntity == null)
        return false;


      using (IDataContextAsync context = new SmartHomeDataContext())
      using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
      {

        var transactionRunner = new UnitOfWorkTransactionRunner(unitOfWork);
        try
        {
          isSuccess = transactionRunner.RunSelectTransaction(() => new CommonService(unitOfWork).IsLoginIdUnique(Convert.ToString(_userEntity.Email)));

          if (isSuccess)
          {
            isSuccess = transactionRunner.RunTransaction(() => new CommonService(unitOfWork).PasswordUpdate(Convert.ToString(_userEntity.Email), Convert.ToString(_userEntity.Password)));
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
  }
}
