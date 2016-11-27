using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
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

namespace SmartHome.Json
{

  public class MessageLogJsonParser : BaseJsonParser
  {
    public MessageLogJsonParser(string jsonString, MessageReceivedFrom receivedFrom)
    {
      _receivedFrom = receivedFrom;
      _homeJsonMessage = jsonString;
      //_homeJsonEntity = JsonDesrialized<HomeJsonEntity>(jsonString);
    }

    public bool SaveNewMessageLog()
    {
     

      using (IDataContextAsync context = new SmartHomeDataContext())
      using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
      {
        var transactionRunner = new UnitOfWorkTransactionRunner(unitOfWork);
        try
        {
          MessageLog messageLog = transactionRunner.RunTransaction(() => new CommonService(unitOfWork).SaveMessageLog(_homeJsonMessage, _receivedFrom));
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
