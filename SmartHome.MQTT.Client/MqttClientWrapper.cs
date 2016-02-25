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
    public static class MqttClientWrapper
    {

        #region delegate event
        public delegate void NotifyMqttMsgPublishReceivedDelegate(CustomEventArgs customEventArgs);
        public static event NotifyMqttMsgPublishReceivedDelegate NotifyMqttMsgPublishReceivedEvent;
        #endregion

        #region constructor
        static MqttClientWrapper()
        {
            //ClientId = string.Empty;
        }
        public static void MakeConnection()
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
        public static bool client_RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
            // logic for validation here
        }
        #endregion

        #region Properties

        public static string BrokerAddress
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
        public static int BrokerPort
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
        public static UInt16 BrokerKeepAlivePeriod
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
        public static string ClientId
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
        public static MqttClient SmartHomeMQTT { get; set; }
        public static string ClientResponce { get; set; }
        #endregion

        #region Methods



        public static string Publish(string messgeTopic, string publishMessage)
        {

            ushort msgId = SmartHomeMQTT.Publish(messgeTopic, // topic
                                          Encoding.UTF8.GetBytes(publishMessage), // message body
                                          MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
                                          true);
            return "Success";


        }

        public static string Subscribe(string messgeTopic)
        {
            ushort msgId = SmartHomeMQTT.Subscribe(new string[] { messgeTopic },
                new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }
                );
            return "Success";
        }

        private static void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            //e.IsPublished //it's defined confirmation message is published or not.
            // Debug.WriteLine("MessageId = " + e.MessageId + " Published = " + e.IsPublished);
            ClientResponce = "Success";
        }



        public static void client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            //Debug.WriteLine("Subscribed for id = " + e.MessageId);
            // write your code
        }

        public static void client_MqttMsgUnsubscribed(object sender, MqttMsgUnsubscribedEventArgs e)
        {
            ClientResponce = "Success";
        }

        public static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var jsonString = Encoding.UTF8.GetString(e.Message);
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

            NotifyMessage(jsonString);
        }

        private static void CommandLog(CommandJsonEntity jsonObject)
        {
            CommandJsonManager commandJsonManager = new CommandJsonManager(jsonObject);
            commandJsonManager.LogCommand(true, "");
        }

        private static CommandJsonEntity ConvertToCommandJsonObject(string jsonString, CommandType commandType)
        {
            CommandJsonEntity jsonObject = CommandJsonManager.JsonDesrialized<CommandJsonEntity>(jsonString);
            jsonObject.CommandType = commandType;
            return jsonObject;
        }

        private static void FeedbackCommandParse(CommandJsonEntity jsonObject)
        {
            CommandJsonManager commandJsonManager = new CommandJsonManager(jsonObject);
            commandJsonManager.Parse();
            //IParserFactory factory = new ParserFactory(jsonObject);
            //var parser = factory.GetParser(jsonObject.CommandId.ToString());
            //parser.Parse();
            //parser.SaveOrUpdateStatus();

        }
        public static void NotifyMessage(string jsonString)
        {
            if (NotifyMqttMsgPublishReceivedEvent != null)
            {
                CustomEventArgs customEventArgs = new CustomEventArgs(jsonString);
                //Raise Event. All the listeners of this event will get a call.
                NotifyMqttMsgPublishReceivedEvent(customEventArgs);
            }
        }

        #endregion

        #region MQTT connection events

        private static void DefinedMQTTCommunicationEvents()
        {
            SmartHomeMQTT.MqttMsgPublished += client_MqttMsgPublished;//publish
            SmartHomeMQTT.MqttMsgSubscribed += client_MqttMsgSubscribed;//subscribe confirmation
            SmartHomeMQTT.MqttMsgUnsubscribed += client_MqttMsgUnsubscribed;
            SmartHomeMQTT.MqttMsgPublishReceived += client_MqttMsgPublishReceived;//received message.

            ushort submsgId = SmartHomeMQTT.Subscribe(new string[] { "/configuration", "/command", "/feedback" },
                              new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                                      MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        }

        private static void BrokerConnectionWithCertificate(string brokerAddress)
        {
            SmartHomeMQTT = new MqttClient(brokerAddress, MqttSettings.MQTT_BROKER_DEFAULT_SSL_PORT, true, new X509Certificate(Resource.ca), null, MqttSslProtocols.TLSv1_2, client_RemoteCertificateValidationCallback);
            SmartHomeMQTT.Connect(ClientId, "mosharraf", "mosharraf", false, BrokerKeepAlivePeriod);
        }

        private static void BrokerConnectionWithoutCertificate(string brokerAddress)
        {
            SmartHomeMQTT = new MqttClient(brokerAddress, BrokerPort, false, null, null, MqttSslProtocols.None, null);
            MQTTConnectiobn();
        }

        private static void LocalBrokerConnection(string brokerAddress)
        {
            SmartHomeMQTT = new MqttClient(brokerAddress);
            MQTTConnectiobn();
        }

        private static void MQTTConnectiobn()
        {
            SmartHomeMQTT.Connect(ClientId, null, null, false, BrokerKeepAlivePeriod);
        }
        #endregion
    }


    public class CustomEventArgs : EventArgs
    {
        public CustomEventArgs(string key)
        {
            _key = key;
        }
        private string _key;

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }
    }
}
