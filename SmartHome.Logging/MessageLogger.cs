using SmartHome.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Logging
{
  public class MessageLogger : IMessageLogger
  {

    public string _homeJsonMessage { get; set; }
    public MessageReceivedFrom _receivedFrom { get; set; }

    public MessageLogger(string jsonString, MessageReceivedFrom receivedFrom)
    {
      _receivedFrom = receivedFrom;
      _homeJsonMessage = jsonString;
    }


    public bool SaveNewMessageLog()
    {
      IMessageLogger dbMessage = new DbMessageLogger(_homeJsonMessage, _receivedFrom);
      dbMessage.SaveNewMessageLog();

      Logger.Info(" Messgae received from " + Enum.GetName(typeof(MessageReceivedFrom), _receivedFrom) + "Received message is " + _homeJsonMessage);

      return true;
    }
  }
}
