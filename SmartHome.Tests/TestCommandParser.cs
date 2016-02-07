using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartHome.Entity;
using SmartHome.Json;
using SmartHome.Model.Enums;
using SmartHome.MQTT.Client;

namespace SmartHome.Tests
{
    [TestClass]
    public class TestCommandJsonManager
    {
        [TestMethod]
        public void TestCommandJsonParse_ShouldParseCommand()
        {
            var jsonString = "{ \"response\": true, \"device_version\": \"00\", \"email\": \"hvuv@vuu.com\", \"device_uuid\": 2094027172,\"device_id\": 32769, \"mac_id\": \"mac\",\"command_byte\": \"[1, 1, 1, 1, 255, 255, 255, -5]\",\"command_id\": 1}";
            CommandJsonEntity jsonObject = CommandJsonManager.JsonDesrialized<CommandJsonEntity>(jsonString);
            jsonObject.CommandType = CommandType.Feedback;
            CommandJsonManager commandJsonManager = new CommandJsonManager(jsonObject);
            commandJsonManager.Parse();
        }
    }
}
