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
            if (customEventArgs.ReceivedTopic == CommandType.Configuration.ToString())
            {                
                 new JsonManager().JsonProcess(customEventArgs.ReceivedMessage);
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