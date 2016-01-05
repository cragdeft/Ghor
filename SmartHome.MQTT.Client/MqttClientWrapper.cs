using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace SmartHome.MQTT.Client
{
    public static class MqttClientWrapper
    {
        static MqttClientWrapper()
        {
            ClientId = string.Empty;
            //BrokerAddress= ConfigurationManager.AppSettings[" BrokerAddress"].ToString();

        }

        public static void MakeConnection(string brokerAddress) // the global controlled variable
        {
            if (SmartHomeMQTT == null)
            {
                SmartHomeMQTT = new MqttClient(brokerAddress, MqttSettings.MQTT_BROKER_DEFAULT_SSL_PORT, true, new X509Certificate(Resource.ca), null, MqttSslProtocols.TLSv1_2, client_RemoteCertificateValidationCallback);
                SmartHomeMQTT.Connect(Guid.NewGuid().ToString(), "mosharraf", "mosharraf", false, 3600);

                SmartHomeMQTT.MqttMsgPublished += client_MqttMsgPublished;//publish
                SmartHomeMQTT.MqttMsgSubscribed += client_MqttMsgSubscribed;//subscribe confirmation
                SmartHomeMQTT.MqttMsgUnsubscribed += client_MqttMsgUnsubscribed;
                SmartHomeMQTT.MqttMsgPublishReceived += client_MqttMsgPublishReceived;//received message.
            }
        }

        public static bool client_RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
            // logic for validation here
        }

        #region Properties

        public static string BrokerAddress
        {
            get;
        }
        public static MqttClient SmartHomeMQTT { get; set; }
        public static string ClientId { get; set; }
        public static string MQTT_BROKER_ADDRESS { get; set; }

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
            ushort msgId = SmartHomeMQTT.Subscribe(new string[] { messgeTopic, "/topic_2" },
                new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE }
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
            //ushort msgId = e.MessageId;
            // write your code
            ClientResponce = "Success";
        }

        public static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            //Debug.WriteLine("Received = " + Encoding.UTF8.GetString(e.Message) + " on topic " + e.Topic);
        }

        #endregion
    }
}
