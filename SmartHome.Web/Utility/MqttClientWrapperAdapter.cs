using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Json;
using SmartHome.Logging;
using SmartHome.Model.Enums;
using SmartHome.Model.ModelDataContext;
using SmartHome.Model.Models;
using SmartHome.MQTT.Client;
using SmartHome.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SmartHome.Web.Utility
{
  public class MqttClientWrapperAdapter
  {
    private static MqttClientWrapper instance = null;
    //Lock synchronization object
    private static object syncLock = new object();

    public static int Counter { get; set; }
    public static MqttClientWrapper WrapperInstance
    {
      get
      {
        lock (syncLock)
        {
          if (MqttClientWrapperAdapter.instance == null)
          {
            instance = new MqttClientWrapper();
            instance.NotifyMqttMsgPublishReceivedEvent += new MqttClientWrapper.NotifyMqttMsgPublishReceivedDelegate(PublishReceivedMessage_NotifyEvent);

            instance.NotifyMqttMsgPublishedEvent += new MqttClientWrapper.NotifyMqttMsgPublishedDelegate(PublishedMessage_NotifyEvent);

            instance.NotifyMqttMsgSubscribedEvent += new MqttClientWrapper.NotifyMqttMsgSubscribedDelegate(SubscribedMessage_NotifyEvent);
          }

          return instance;
        }
      }
    }

    static void PublishReceivedMessage_NotifyEvent(CustomEventArgs customEventArgs)
    {

      //if (customEventArgs.ReceivedTopic.Contains("configuration") && Counter < 4)
      //{
      //  LogMqttRequestMessages(customEventArgs);

      //  Counter = Counter + 1;
      //  if (IsValidJson(customEventArgs.ReceivedMessage))
      //  {
      //    JsonParser jsonManager = new JsonParser(customEventArgs.ReceivedMessage, MessageReceivedFrom.MQTT);
      //    jsonManager.ProcessJsonData();
      //  }
      //}

      if (customEventArgs.ReceivedTopic.Contains("feedback"))
      {

        //var jsonObject = new CommandJsonManager().ConvertToCommandJsonObject(customEventArgs.ReceivedMessage, customEventArgs.ReceivedTopic.Split('/')[1], CommandType.Feedback);
        //CommandJsonManager commandJsonManager = new CommandJsonManager(jsonObject);
        //commandJsonManager.Parse();
        //FeedbackCommandParse(jsonObject);
      }

      Counter = 0;


    }

    private static void LogMqttRequestMessages(CustomEventArgs customEventArgs)
    {
      string msg = customEventArgs.ReceivedMessage.ToString();
      new MessageLogger(msg, string.Empty, MessageReceivedFrom.MQTT).SaveNewMessageLog();
    }

    static void PublishedMessage_NotifyEvent(CustomEventArgs customEventArgs)
    {
      string msg = customEventArgs.ReceivedMessage;
    }
    static void SubscribedMessage_NotifyEvent(CustomEventArgs customEventArgs)
    {
      string msg = customEventArgs.ReceivedMessage;
    }

    private static bool IsValidJson(string strInput)
    {
      strInput = strInput.Trim();
      if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
          (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
      {
        try
        {
          var obj = JToken.Parse(strInput);
          return true;
        }
        catch (JsonReaderException jex)
        {
          //Exception in parsing json
          Console.WriteLine(jex.Message);
          return false;
        }
        catch (Exception ex) //some other exception
        {
          Console.WriteLine(ex.ToString());
          return false;
        }
      }
      else
      {
        return false;
      }
    }
  }
}