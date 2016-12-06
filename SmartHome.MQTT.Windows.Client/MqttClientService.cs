using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using SmartHome.Logging;
using System.Net.Security;
using SmartHome.Model.Enums;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SmartHome.Json;

namespace SmartHome.MQTT.Windows.Client
{
    public class MqttClientService : IHomeBatchStartPoint
    {
        private MqttClient SmartHomeMqtt { get; set; }

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

        public void Execute()
        {
            Logger.Info("MQTT listener is going to start");
            MqttClientInstance();
            Logger.Info("MQTT listener has been started");
        }

        public void MqttClientInstance()
        {
            MakeConnection();
        }


        private void MakeConnection()
        {
            try
            {
                if (SmartHomeMqtt == null || !SmartHomeMqtt.IsConnected)
                {

                    if (BrokerAddress == "192.168.11.205")
                    {
                        //LocalBrokerConnection(BrokerAddress);
                    }

                    else if (BrokerAddress == "192.168.1.1")
                    {
                        //BrokerConnectionWithoutCertificateForCommand(BrokerAddress);
                    }

                    else if (BrokerAddress == "192.169.244.37")
                    {
                        Logger.Info("Try to connect with broker.");
                        //BrokerConnectionWithoutCertificateForCommand(BrokerAddress);
                        BrokerConnectionWithCertificateForWebBroker(BrokerAddress);
                        Logger.Info("Web broker connection successfull.");
                    }
                    else if (BrokerAddress == "192.168.11.150")
                    {
                        // BrokerConnectionWithoutCertificate(BrokerAddress);
                    }
                    else
                    {
                        // BrokerConnectionWithCertificate(BrokerAddress);
                    }


                    DefinedMqttCommunicationEvents();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not stablished connection to MQTT broker - " + ex.Message);

                //don't leave the client connected
                if (SmartHomeMqtt != null && SmartHomeMqtt.IsConnected)
                {
                    try
                    {
                        SmartHomeMqtt.Disconnect();
                    }
                    catch
                    {
                        Logger.Error(string.Format("Could not disconnect to MQTT broker: {1}", ex.Message));
                    }
                }
                MakeConnection();
            }
        }

        private void BrokerConnectionWithCertificateForWebBroker(string brokerAddress)
        {
            try
            {
                //if (WaitUntilBrokerAlive(brokerAddress))
                //{
                SmartHomeMqtt = new MqttClient(brokerAddress, MqttSettings.MQTT_BROKER_DEFAULT_SSL_PORT, true, new X509Certificate(Resources.WebBrokerResouce.ca), null, MqttSslProtocols.TLSv1_2, client_RemoteCertificateValidationCallback);
                SmartHomeMqtt.Connect(ClientId, "kanok", "kanok", false, BrokerKeepAlivePeriod);
                //}
            }
            catch (Exception ex)
            {

                Logger.Error(string.Format("Error on BrokerConnectionWithCertificateForWebBroker {0} ", ex.Message.ToString()));
            }

        }

        public bool client_RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
            // logic for validation here
        }

        private void DefinedMqttCommunicationEvents()
        {
            SmartHomeMqtt.MqttMsgPublished += PublishedMessage_MQTT;//publish
            SmartHomeMqtt.MqttMsgSubscribed += SubscribedMessage_MQTT;//subscribe confirmation
            SmartHomeMqtt.MqttMsgUnsubscribed += UnsubscribedMessage_MQTT;
            SmartHomeMqtt.MqttMsgPublishReceived += ReceivedMessage_MQTT;//received message.
            SmartHomeMqtt.ConnectionClosed += ConnectionClosed_MQTT;

            ushort submsgId = SmartHomeMqtt.Subscribe(new string[] { "sh/configuration/#", "sh/command", "sh/feedback/#" },
                          new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                                      MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        }

        private void PublishedMessage_MQTT(object sender, MqttMsgPublishedEventArgs e)
        {
            Logger.Info(string.Format("Mqtt-Msg-Published to topic {0}", e.IsPublished.ToString()));
        }

        private void SubscribedMessage_MQTT(object sender, MqttMsgSubscribedEventArgs e)
        {
            Logger.Info(string.Format("Mqtt-Msg-Subscribed to topic {0}", e.MessageId.ToString()));
        }

        private void UnsubscribedMessage_MQTT(object sender, MqttMsgUnsubscribedEventArgs e)
        {
            //ClientResponce = "Success";
        }

        private void ReceivedMessage_MQTT(object sender, MqttMsgPublishEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Message);
            string topic = e.Topic.ToString();
            Logger.Info("Message received from topic '" + topic + "' and message is '" + message + "'");
            //ProcessMessage(topic, message);//need to work
            //AsyncService.RunAsync((domainObjectContainer) =>
            //                 ProcessMessage(topic, message));


            #region MyRegion

            if (topic.Contains("configuration"))
            {
                LogMqttRequestMessages(message);

                if (IsValidJson(message))
                {
                    JsonParser jsonManager = new JsonParser(message, MessageReceivedFrom.MQTT);
                    jsonManager.ProcessJsonData();
                }
            }

            #endregion


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

        private void LogMqttRequestMessages(string message)
        {

            new MessageLogger(message, string.Empty, MessageReceivedFrom.MQTT).SaveNewMessageLog();
        }

        private void ConnectionClosed_MQTT(object sender, EventArgs e)
        {
            SmartHomeMqtt = null;
            if (!(sender as MqttClient).IsConnected || SmartHomeMqtt == null)
            {
                HandleReconnect();
            }
            Logger.Info("Connection has been closed");
        }

        private void HandleReconnect()
        {
            MakeConnection();
        }
    }
}
