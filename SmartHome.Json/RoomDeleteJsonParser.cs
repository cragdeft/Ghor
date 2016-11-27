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
  public class RoomDeleteJsonParser : BaseJsonParser
  {
    public RoomDeleteJsonParser(string jsonString, MessageReceivedFrom receivedFrom)
    {
      _receivedFrom = receivedFrom;
      _homeJsonMessage = jsonString;
      _homeJsonEntity = JsonDesrialized<HomeJsonEntity>(jsonString);
    }
    public bool DeleteRoom()
    {
      if (_homeJsonEntity == null)
        return false;

      using (IDataContextAsync context = new SmartHomeDataContext())
      using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
      {
        var service = new RoomDeleteJsonParserService(unitOfWork, _homeJsonEntity, _homeJsonMessage, _receivedFrom);
        var transactionRunner = new UnitOfWorkTransactionRunner(unitOfWork);

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
        }
      }
      return true;
    }
  }
}
