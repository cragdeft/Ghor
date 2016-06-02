using SmartHome.Json;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Configuration;
using System.Collections.Generic;
using SmartHome.Logging;

namespace SmartHome.MQTT.Client
{
    public class MqttClientWrapper
    {

        #region delegate event

        #region MqttMsg-Publish-Received-Notification
        public delegate void NotifyMqttMsgPublishReceivedDelegate(CustomEventArgs customEventArgs);
        public event NotifyMqttMsgPublishReceivedDelegate NotifyMqttMsgPublishReceivedEvent;
        #endregion

        #region MqttMsg-Published-Notification
        public delegate void NotifyMqttMsgPublishedDelegate(CustomEventArgs customEventArgs);
        public event NotifyMqttMsgPublishedDelegate NotifyMqttMsgPublishedEvent;
        #endregion

        #region MqttMsg-Subscribed-Notification
        public delegate void NotifyMqttMsgSubscribedDelegate(CustomEventArgs customEventArgs);
        public event NotifyMqttMsgSubscribedDelegate NotifyMqttMsgSubscribedEvent;
        #endregion

        #endregion

        #region constructor
        public MqttClientWrapper()
        {

            
        }
        public void MakeConnection()
        {


            #region MyRegion

            try
            {
                if (SmartHomeMQTT == null || !SmartHomeMQTT.IsConnected)
                {
                    if (BrokerAddress == "192.168.11.162")
                    {
                        LocalBrokerConnection(BrokerAddress);
                    }

                    else if (BrokerAddress == "192.168.2.1")
                    {
                        BrokerConnectionWithoutCertificateForCommand(BrokerAddress);
                    }

                    else if (BrokerAddress == "192.169.244.37")
                    {
                        //BrokerConnectionWithoutCertificateForCommand(BrokerAddress);
                        BrokerConnectionWithCertificateForWebBroker(BrokerAddress);
                    }
                    else if (BrokerAddress == "192.168.11.150")
                    {
                        BrokerConnectionWithoutCertificate(BrokerAddress);
                    }
                    else
                    {
                        BrokerConnectionWithCertificate(BrokerAddress);
                    }

                    DefinedMQTTCommunicationEvents();
                }

            }
            catch (Exception ex)
            {

                //Logger.LogError(ex, string.Format("Could not stablished connection to MQ broker: {1}", ex.Message));

                //don't leave the client connected
                if (SmartHomeMQTT != null && SmartHomeMQTT.IsConnected)
                    try
                    {
                        SmartHomeMQTT.Disconnect();
                    }
                    catch
                    {
                        Logger.LogError(ex, string.Format("Could not disconnect to MQ broker: {1}", ex.Message));
                    }
            }
            #endregion

        }
        public bool client_RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
            // logic for validation here
        }
        #endregion

        #region Properties

        public string WillTopic { get; set; }
        readonly object locker = new object();
        public string BrokerAddress
        {
            get
            {
                if (ConfigurationManager.AppSettings["BrokerAddress"] == null)
                {
                    return string.Empty;
                }
                return ConfigurationManager.AppSettings["BrokerAddress"].ToString();
            }

        }
        public int BrokerPort
        {
            get
            {
                if (ConfigurationManager.AppSettings["BrokerPort"] == null)
                {
                    return 1883;
                }
                return Convert.ToInt32(ConfigurationManager.AppSettings["BrokerPort"]);
            }

        }
        public UInt16 BrokerKeepAlivePeriod
        {
            get
            {
                if (ConfigurationManager.AppSettings["BrokerKeepAlivePeriod"] == null)
                {
                    return 3600;
                }
                return Convert.ToUInt16(ConfigurationManager.AppSettings["BrokerKeepAlivePeriod"]);
            }

        }
        public string ClientId
        {
            get
            {
                if (ConfigurationManager.AppSettings["BrokerAccessClientId"] == null)
                {
                    return Guid.NewGuid().ToString();
                }
                return ConfigurationManager.AppSettings["BrokerAccessClientId"].ToString();
            }

        }
        public MqttClient SmartHomeMQTT { get; set; }
        public string ClientResponce { get; set; }
        #endregion

        #region Methods



        public string Publish(string messgeTopic, string publishMessage)
        {
            if (SmartHomeMQTT != null)
            {
                try
                {
                    lock (locker)
                    {
                        ushort msgId = SmartHomeMQTT.Publish(messgeTopic, // topic
                                          Encoding.UTF8.GetBytes(publishMessage), // message body
                                          MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
                                          true);
                    }
                }
                catch (Exception ex)
                {
                    //    log.Warn("Error while publishing: " + ex.Message, ex);
                }
            }
            return "Success";


        }

        public string Subscribe(string messgeTopic)
        {
            if (SmartHomeMQTT != null)
            {
                ushort msgId = SmartHomeMQTT.Subscribe(new string[] { messgeTopic },
                     new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }
                     );
                Logger.Log(string.Format("Subscription to topic {0}", messgeTopic));
            }
            return "Success";
        }

        /// <summary>
        /// Subscribe to a list of topics
        /// </summary>
        public void Subscribe(IEnumerable<string> messgeTopics)
        {
            foreach (var item in messgeTopics)
                Subscribe(item);
        }

        private void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            NotifyMessage("MqttMsgPublished", e.IsPublished.ToString(), string.Empty);
            Logger.Log(string.Format("Mqtt-Msg-Published to topic {0}", e.IsPublished.ToString()));
            ClientResponce = "Success";
        }



        public void client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            NotifyMessage("MqttMsgSubscribed", e.MessageId.ToString(), string.Empty);
            Logger.Log(string.Format("Mqtt-Msg-Subscribed to topic {0}", e.MessageId.ToString()));

        }

        public void client_MqttMsgUnsubscribed(object sender, MqttMsgUnsubscribedEventArgs e)
        {
            ClientResponce = "Success";
        }

        public void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            //var receivedMessage = Encoding.UTF8.GetString(e.Message);
            #region MyRegion
            //if (e.Topic == CommandType.Configuration.ToString())
            //{
            //    new JsonManager().JsonProcess(jsonString);
            //}

            //if (e.Topic == CommandType.Feedback.ToString())
            //{
             //   var jsonObject = ConvertToCommandJsonObject(jsonString, CommandType.Feedback);
            //    FeedbackCommandParse(jsonObject);
            //}

            //if (e.Topic == CommandType.Command.ToString())
            //{
            //    var jsonObject = ConvertToCommandJsonObject(jsonString, CommandType.Command);
            //    CommandLog(jsonObject);
            //} 
            #endregion

            NotifyMessage("MqttMsgPublishReceived", Encoding.UTF8.GetString(e.Message), e.Topic.ToString());
            Logger.Log(string.Format("Mqtt-Msg-Publish-Received to topic {0}", e.Topic.ToString()));
        }

        public void client_ConnectionClosed(object sender, EventArgs e)
        {

            if (!(sender as MqttClient).IsConnected || SmartHomeMQTT == null)
            {
                HandleReconnect();
            }
            Logger.Log("Connection has been closed");
        }

    
        void HandleReconnect()
        {            
            MakeConnection();
        }
        



        private void CommandLog(CommandJsonEntity jsonObject)
        {
            CommandJsonManager commandJsonManager = new CommandJsonManager(jsonObject);
            commandJsonManager.LogCommand(true, "");
        }

        private CommandJsonEntity ConvertToCommandJsonObject(string jsonString, CommandType commandType)
        {
            CommandJsonEntity jsonObject = CommandJsonManager.JsonDesrialized<CommandJsonEntity>(jsonString);
            jsonObject.CommandType = commandType;
            return jsonObject;
        }

        private void FeedbackCommandParse(CommandJsonEntity jsonObject)
        {
            CommandJsonManager commandJsonManager = new CommandJsonManager(jsonObject);
            commandJsonManager.Parse();
            //IParserFactory factory = new ParserFactory(jsonObject);
            //var parser = factory.GetParser(jsonObject.CommandId.ToString());
            //parser.Parse();
            //parser.SaveOrUpdateStatus();

        }

        #region Delegate and event implementation
        public void NotifyMessage(string NotifyType, string receivedMessage, string receivedTopic)
        {
            if (NotifyType == "MqttMsgPublishReceived")
            {
                InvokeEvents<NotifyMqttMsgPublishReceivedDelegate>(receivedMessage, receivedTopic, NotifyMqttMsgPublishReceivedEvent);
            }

            if (NotifyType == "MqttMsgPublished")
            {
                InvokeEvents<NotifyMqttMsgPublishedDelegate>(receivedMessage, receivedTopic, NotifyMqttMsgPublishedEvent);
            }

            if (NotifyType == "MqttMsgSubscribed")
            {
                InvokeEvents<NotifyMqttMsgSubscribedDelegate>(receivedMessage, receivedTopic, NotifyMqttMsgSubscribedEvent);

            }
        }

        private static void InvokeEvents<T>(string receivedMessage, string receivedTopic, T eventDelegate)
        {
            if (eventDelegate != null)
            {
                var customEventArgs = new CustomEventArgs(receivedMessage, receivedTopic);
                ((Delegate)(object)eventDelegate).DynamicInvoke(customEventArgs);
            }
        }
        #endregion

        #endregion

        #region MQTT connection events

        private void DefinedMQTTCommunicationEvents()
        {

            SmartHomeMQTT.MqttMsgPublished += client_MqttMsgPublished;//publish
            SmartHomeMQTT.MqttMsgSubscribed += client_MqttMsgSubscribed;//subscribe confirmation
            SmartHomeMQTT.MqttMsgUnsubscribed += client_MqttMsgUnsubscribed;
            SmartHomeMQTT.MqttMsgPublishReceived += client_MqttMsgPublishReceived;//received message.
            SmartHomeMQTT.ConnectionClosed += client_ConnectionClosed;

            var temp = new string[] { "configuration", "/command", "/feedback", "/command/kanok", "/feedback/kanok" };

            ushort submsgId = SmartHomeMQTT.Subscribe(new string[] { "configuration", "/command", "/feedback","/command/kanok","/feedback/kanok" },
                              new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                                      MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        }

        private void BrokerConnectionWithCertificate(string brokerAddress)
        {
            SmartHomeMQTT = new MqttClient(brokerAddress, MqttSettings.MQTT_BROKER_DEFAULT_SSL_PORT, true, new X509Certificate(Resource.ca), null, MqttSslProtocols.TLSv1_2, client_RemoteCertificateValidationCallback);
            SmartHomeMQTT.Connect(ClientId, "mosharraf", "mosharraf", false, BrokerKeepAlivePeriod);
        }

        private void BrokerConnectionWithCertificateForWebBroker(string brokerAddress)
        {
            SmartHomeMQTT = new MqttClient(brokerAddress, MqttSettings.MQTT_BROKER_DEFAULT_SSL_PORT, true, new X509Certificate(WebBrokerResouce.ca), null, MqttSslProtocols.TLSv1_2, client_RemoteCertificateValidationCallback);
            SmartHomeMQTT.Connect(ClientId, "kanok", "kanok", false, BrokerKeepAlivePeriod);
        }        

        private void BrokerConnectionWithoutCertificateForCommand(string brokerAddress)
        {
            SmartHomeMQTT = new MqttClient(brokerAddress, BrokerPort, false, null, null, MqttSslProtocols.None, null);
            SmartHomeMQTT.Connect(ClientId, "kanok", "kanok", false, BrokerKeepAlivePeriod);
        }

        private void BrokerConnectionWithoutCertificate(string brokerAddress)
        {
            SmartHomeMQTT = new MqttClient(brokerAddress, BrokerPort, false, null, null, MqttSslProtocols.None, null);
            MQTTConnectiobn();
        }

        private void LocalBrokerConnection(string brokerAddress)
        {
            SmartHomeMQTT = new MqttClient(brokerAddress);
            MQTTConnectiobn();
        }

        private void MQTTConnectiobn()
        {
            SmartHomeMQTT.Connect(ClientId, null, null, false, BrokerKeepAlivePeriod);
        }
        #endregion
    }


    public class CustomEventArgs : EventArgs
    {
        public CustomEventArgs(string receivedMessage, string receivedTopic)
        {
            _receivedMessage = receivedMessage;
            _receivedTopic = receivedTopic;
        }
        private string _receivedMessage;

        public string ReceivedMessage
        {
            get { return _receivedMessage; }
            set { _receivedMessage = value; }
        }


        private string _receivedTopic;
        public string ReceivedTopic
        {
            get { return _receivedTopic; }
            set { _receivedTopic = value; }
        }
    }
}
