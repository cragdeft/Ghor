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
  public class ChannelDeleteJsonParser : BaseJsonParser
  {
    public ChannelDeleteJsonParser(string jsonString, MessageReceivedFrom receivedFrom)
    {
      _receivedFrom = receivedFrom;
      _homeJsonMessage = jsonString;
      _homeJsonEntity = JsonDesrialized<HomeJsonEntity>(jsonString);
    }

    public bool DeleteChannel()
    {
      if (_homeJsonEntity == null)
        return false;
     // new MessageLogJsonParser(_homeJsonMessage, _receivedFrom).SaveNewMessageLog();
      using (IDataContextAsync context = new SmartHomeDataContext())
      using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
      {
        var service = new ChannelDeleteJsonParserService(unitOfWork, _homeJsonEntity, _homeJsonMessage, _receivedFrom);
        var transactionRunner = new UnitOfWorkTransactionRunner(unitOfWork);

        // MessageLog messageLog = transactionRunner.RunTransaction(() => new CommonService(unitOfWork).SaveMessageLog(_homeJsonMessage, _receivedFrom));

        try
        {
          transactionRunner.RunTransaction(() => service.DeleteJsonData());
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
