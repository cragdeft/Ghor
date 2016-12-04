using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
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

namespace SmartHome.Logging
{
  public class DbMessageLogger : IMessageLogger
  {
    public string _homeJsonMessage { get; set; }
    public string _homeJsonEncryptMessage { get; set; }
    public MessageReceivedFrom _receivedFrom { get; set; }

    public DbMessageLogger(string jsonString, string encryptJsonString, MessageReceivedFrom receivedFrom)
    {
      _homeJsonMessage = jsonString;
      _homeJsonEncryptMessage = encryptJsonString;
      _receivedFrom = receivedFrom;
    }

    public bool SaveNewMessageLog()
    {

      using (IDataContextAsync context = new SmartHomeDataContext())
      using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
      {
        var transactionRunner = new UnitOfWorkTransactionRunner(unitOfWork);
        try
        {
          MessageLog messageLog = transactionRunner.RunTransaction(() => new CommonService(unitOfWork).SaveMessageLog(_homeJsonMessage,_homeJsonEncryptMessage, _receivedFrom));
        }
        catch (Exception ex)
        {
          return false;
        }
        finally
        {
          //   transactionRunner.RunTransaction(() => new CommonService(unitOfWork).UpdateMessageLog(messageLog, _homeJsonEntity.Home[0].PassPhrase));
        }
      }
      return true;
    }
  }
}
