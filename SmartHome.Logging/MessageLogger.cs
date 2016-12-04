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
    public string _homeJsonEncryptMessage { get; set; }
    public MessageReceivedFrom _receivedFrom { get; set; }

    public MessageLogger(string jsonString, string encryptJsonString, MessageReceivedFrom receivedFrom)
    {
      _homeJsonMessage = jsonString;
      _homeJsonEncryptMessage = encryptJsonString;
      _receivedFrom = receivedFrom;
    }


    public bool SaveNewMessageLog()
    {
      IMessageLogger dbMessage = new DbMessageLogger(_homeJsonMessage, _homeJsonEncryptMessage, _receivedFrom);
      dbMessage.SaveNewMessageLog();

      //Logger.Info(" Messgae received from " + Enum.GetName(typeof(MessageReceivedFrom), _receivedFrom) + "Received message is " + _homeJsonMessage);

      return true;
    }
  }
}
