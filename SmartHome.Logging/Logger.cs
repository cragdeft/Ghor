using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Logging
{
    public static class Logger
    {
    
        public static void LogError(Exception ex, string contextualMessage = null)
        {
            try
            {
                
                if (contextualMessage != null)
                {
                   
                    var annotatedException = new Exception(contextualMessage+ " error type is " +ex.GetType().Name, ex);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(annotatedException);
                   
                    
                }
                else
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }
               
            }
            catch (Exception)
            {
               
            }
        }
    }
}
