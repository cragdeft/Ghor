using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
using SmartHome.Logging;
using SmartHome.Model.Enums;
using SmartHome.Model.ModelDataContext;
using SmartHome.Model.Models;
using SmartHome.Service;
using SmartHome.Service.Interfaces;
using SmartHome.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Data.Processor
{
  public class UserDatatProcessor : BaseDataProcessor
  {
    public dynamic _userEntity { get; private set; }

    public UserDatatProcessor(string jsonString, MessageReceivedFrom receivedFrom)
    {
      _receivedFrom = receivedFrom;
      _homeJsonMessage = jsonString;
      _userEntity = JsonDesrialized<dynamic>(jsonString);
    }
    public bool IsUserExist()
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
