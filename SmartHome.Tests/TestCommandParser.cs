using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartHome.MQTT.Client;

namespace SmartHome.Tests
{
    [TestClass]
    public class TestCommandParser
    {
        [TestMethod]
        public void Encrypt_ShouldParseCommand()
        {
            string cs = "[{1,6,32,0,0,3,0,1,1,1,0,0,2,1,56,1,3,0,0,0,4,0,0,0,5,0,0,0,6,1,0,0}]";
            var values = cs.Replace("[{", string.Empty).Replace("}]", string.Empty).Split(',');
            byte[] commandArray = new byte[values.Length];

            for (int index = 0; index < values.Length; index++)
            {
                commandArray[index] = Convert.ToByte(values[index]);
            }
           // CommandParser fp = new CommandParser(commandArray);
        }
    }
}
