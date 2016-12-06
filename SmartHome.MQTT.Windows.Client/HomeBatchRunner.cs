using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHome.MQTT.Windows.Client
{
    //class HomeBatchRunner
    //{
    //}


    public sealed class HomeBatchRunner : IHomeBatchRunner
    {
        //private readonly INakedObjectsFramework framework;

        public HomeBatchRunner()
        {
            //Assert.AssertNotNull(framework);
            //this.framework = framework;
        }

        #region IBatchRunner Members

        public void Run(IHomeBatchStartPoint batchStartPoint)
        {
            //framework.DomainObjectInjector.InjectInto(batchStartPoint);
            //StartTransaction();
            batchStartPoint.Execute();
            //EndTransaction();
            while (true)
            {
                Thread.Sleep(5000);
            }
        }

        #endregion

        private void StartTransaction()
        {
           // framework.TransactionManager.StartTransaction();
        }

        private void EndTransaction()
        {
           // framework.TransactionManager.EndTransaction();
        }
    }
}
