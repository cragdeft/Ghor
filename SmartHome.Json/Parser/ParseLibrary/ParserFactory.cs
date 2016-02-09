using SmartHome.Entity;
using SmartHome.Model.Enums;

namespace SmartHome.Json.Parser.ParseLibrary
{
    public class ParserFactory : IParserFactory
    {
        private CommandJsonEntity CommandJson { get; }

        public ParserFactory(CommandJsonEntity commandJson)
        {
            CommandJson = commandJson;
        }
        public IParser GetParser(string parserType)
        {
            if(parserType == CommandId.SmartSwitchDimmingFeedback.ToString())
                return new DimmingFeedbackParser(CommandJson);

            else if (parserType == CommandId.DeviceCurrentLoadStatusFeedback.ToString())
                return new CurrentLoadStatusParser(CommandJson);

            else if(parserType == CommandId.SmartSwitchLoadTypeSelectFeedback.ToString())
                return new LoadTypeSelectParser(CommandJson);

            else if(parserType == CommandId.SmartSwitchHardwareDimmingFeedback.ToString())
                return new DimmingFeedbackEnableDisableParser(CommandJson);

            else if(parserType == CommandId.DeviceOnOffFeedback.ToString())
                return new OnOffFeedbackParser(CommandJson);

            else if(parserType == CommandId.SmartSwitchThermalShutdownNotificationFeedback.ToString())
                return new ThermalShutDownParser(CommandJson);
            else if (parserType == CommandId.DevicePingRequest.ToString())
                return new ThermalShutDownParser(CommandJson);

            else return new EmptyParser();
        }
    }
}
