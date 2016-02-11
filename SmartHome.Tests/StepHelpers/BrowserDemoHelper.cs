using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatiN.Core;
using WatiN.Core.Native.Windows;

namespace SmartHome.Tests.StepHelpers
{
    internal static class BrowserDemoHelper
    {
        public static void BringToFrontForDemo(this Browser browser)
        {
            browser.ShowWindow(NativeMethods.WindowShowStyle.Minimize);
            browser.ShowWindow(NativeMethods.WindowShowStyle.Maximize);
        }
    }
}
