using SmartHome.Entity;
using SmartHome.Json;
using SmartHome.Model.Enums;
using SmartHome.MQTT.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Utility
{
    public class MqttClientWrapperAdapter
    {
        private static MqttClientWrapper instance = null;
        //Lock synchronization object
        private static object syncLock = new object();
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
            if (customEventArgs.ReceivedTopic == "jsonparse")// CommandType.Configuration.ToString()
            {
                JsonParser jsonManager = new JsonParser(customEventArgs.ReceivedMessage);
                jsonManager.Save();
            }
            if (customEventArgs.ReceivedTopic == "configuration")// CommandType.Configuration.ToString()
            {
                //new ConfigurationJsonManager().JsonProcess(customEventArgs.ReceivedMessage);
                JsonParser jsonManager = new JsonParser(customEventArgs.ReceivedMessage);
                jsonManager.Save();
            }

            if (customEventArgs.ReceivedTopic == "feedback/kanok")//CommandType.Feedback.ToString()
            {
                var jsonObject = new CommandJsonManager().ConvertToCommandJsonObject(customEventArgs.ReceivedMessage, CommandType.Feedback);
                CommandJsonManager commandJsonManager = new CommandJsonManager(jsonObject);
                commandJsonManager.Parse();
                //FeedbackCommandParse(jsonObject);
            }

            if (customEventArgs.ReceivedTopic == "command/kanok")//CommandType.Command.ToString()
            {
                var jsonObject = new CommandJsonManager().ConvertToCommandJsonObject(customEventArgs.ReceivedMessage, CommandType.Command);
                new CommandJsonManager().CommandLog(jsonObject);
            }

            if (customEventArgs.ReceivedTopic == "json")//CommandType.Command.ToString()
            {
                var jsonObject = new CommandJsonManager().ConvertToCommandJsonObject(customEventArgs.ReceivedMessage, CommandType.Command);
                new CommandJsonManager().CommandLog(jsonObject);
            }


        }
        static void PublishedMessage_NotifyEvent(CustomEventArgs customEventArgs)
        {
            string msg = customEventArgs.ReceivedMessage;

        }

        static void SubscribedMessage_NotifyEvent(CustomEventArgs customEventArgs)
        {
            string msg = customEventArgs.ReceivedMessage;
        }



    }
}