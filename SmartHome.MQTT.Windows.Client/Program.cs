using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Practices.Unity;

namespace SmartHome.MQTT.Windows.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            UnityActivator.Start();

            UnityConfig.GetConfiguredContainer().Resolve<IHomeBatchRunner>().Run(new MqttClientService());

            UnityActivator.Shutdown();
        }
    }
}
