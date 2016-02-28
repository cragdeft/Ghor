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
            //MakeConnection();
            //ClientId = string.Empty;
        }
        public void MakeConnection()
        {
            if (SmartHomeMQTT == null)
            {
                if (BrokerAddress == "192.168.11.195")
                {
                    LocalBrokerConnection(BrokerAddress);
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
        public bool client_RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
            // logic for validation here
        }
        #endregion

        #region Properties

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

            ushort msgId = SmartHomeMQTT.Publish(messgeTopic, // topic
                                          Encoding.UTF8.GetBytes(publishMessage), // message body
                                          MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
                                          true);
            return "Success";


        }

        public string Subscribe(string messgeTopic)
        {
            ushort msgId = SmartHomeMQTT.Subscribe(new string[] { messgeTopic },
                new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }
                );
            return "Success";
        }

        private void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            NotifyMessage("MqttMsgPublished", e.IsPublished.ToString(), string.Empty);
            //e.IsPublished //it's defined confirmation message is published or not.
            // Debug.WriteLine("MessageId = " + e.MessageId + " Published = " + e.IsPublished);
            ClientResponce = "Success";
        }



        public void client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            NotifyMessage("MqttMsgSubscribed", e.MessageId.ToString(), string.Empty);
            //Debug.WriteLine("Subscribed for id = " + e.MessageId);
            // write your code
        }

        public void client_MqttMsgUnsubscribed(object sender, MqttMsgUnsubscribedEventArgs e)
        {
            ClientResponce = "Success";
        }

        public void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            //string a = e.Topic;
            //var receivedMessage = Encoding.UTF8.GetString(e.Message);
            #region MyRegion
            //if (e.Topic == CommandType.Configuration.ToString())
            //{
            //    new JsonManager().JsonProcess(jsonString);
            //}

            //if (e.Topic == CommandType.Feedback.ToString())
            //{
            //    var jsonObject = ConvertToCommandJsonObject(jsonString, CommandType.Feedback);
            //    FeedbackCommandParse(jsonObject);
            //}

            //if (e.Topic == CommandType.Command.ToString())
            //{
            //    var jsonObject = ConvertToCommandJsonObject(jsonString, CommandType.Command);
            //    CommandLog(jsonObject);
            //} 
            #endregion

            NotifyMessage("MqttMsgPublishReceived", Encoding.UTF8.GetString(e.Message), e.Topic.ToString());
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
        public void NotifyMessage(string NotifyType,string receivedMessage, string receivedTopic)
        {
            if (NotifyType== "MqttMsgPublishReceived")
            {
                if (NotifyMqttMsgPublishReceivedEvent != null)
                {
                    CustomEventArgs customEventArgs = new CustomEventArgs(receivedMessage, receivedTopic);
                    //Raise Event. All the listeners of this event will get a call.
                    NotifyMqttMsgPublishReceivedEvent(customEventArgs);
                }
            }

            if (NotifyType == "MqttMsgPublished")
            {
                if (NotifyMqttMsgPublishedEvent != null)
                {
                    CustomEventArgs customEventArgs = new CustomEventArgs(receivedMessage, receivedTopic);
                    //Raise Event. All the listeners of this event will get a call.
                    NotifyMqttMsgPublishedEvent(customEventArgs);
                }
            }

            if (NotifyType == "MqttMsgSubscribed")
            {
                if (NotifyMqttMsgSubscribedEvent != null)
                {
                    CustomEventArgs customEventArgs = new CustomEventArgs(receivedMessage, receivedTopic);
                    //Raise Event. All the listeners of this event will get a call.
                    NotifyMqttMsgSubscribedEvent(customEventArgs);
                }
            }

        }

        #endregion

        #region MQTT connection events

        private void DefinedMQTTCommunicationEvents()
        {
            SmartHomeMQTT.MqttMsgPublished += client_MqttMsgPublished;//publish
            SmartHomeMQTT.MqttMsgSubscribed += client_MqttMsgSubscribed;//subscribe confirmation
            SmartHomeMQTT.MqttMsgUnsubscribed += client_MqttMsgUnsubscribed;
            SmartHomeMQTT.MqttMsgPublishReceived += client_MqttMsgPublishReceived;//received message.

            ushort submsgId = SmartHomeMQTT.Subscribe(new string[] { "/configuration", "/command", "/feedback" },
                              new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                                      MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        }

        private void BrokerConnectionWithCertificate(string brokerAddress)
        {
            SmartHomeMQTT = new MqttClient(brokerAddress, MqttSettings.MQTT_BROKER_DEFAULT_SSL_PORT, true, new X509Certificate(Resource.ca), null, MqttSslProtocols.TLSv1_2, client_RemoteCertificateValidationCallback);
            SmartHomeMQTT.Connect(ClientId, "mosharraf", "mosharraf", false, BrokerKeepAlivePeriod);
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
