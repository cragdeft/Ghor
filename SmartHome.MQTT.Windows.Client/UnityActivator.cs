using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace SmartHome.MQTT.Windows.Client
{
    public static class UnityActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start()
        {
           // UnityConfig.GetConfiguredContainer().Resolve<IReflect>().Reflect();
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary>
        public static void Shutdown()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }
}
