using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.MQTT.Windows.Client
{
    public interface IHomeBatchRunner
    {
        void Run(IHomeBatchStartPoint batchStartPoint);
    }
}
