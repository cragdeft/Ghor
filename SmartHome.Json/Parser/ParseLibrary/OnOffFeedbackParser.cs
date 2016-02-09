using SmartHome.Entity;

namespace SmartHome.Json.Parser.ParseLibrary
{
    public class OnOffFeedbackParser : IParser
    {
        private BaseParser Parser { get; }
        public OnOffFeedbackParser(CommandJsonEntity commandJsonEntity)
        {
            Parser = new BaseParser(commandJsonEntity);
        }
        public void Parse()
        {
            if (Parser.IsDeviceExists())
            {
                Parser.ParseInitiatorAndSetVersionValue();
                Parser.ThermalShutDownCommandParse();
            }
            else
            {
                Parser.LogCommand(false, "Device not found.");
            }
        }
        public void SaveOrUpdateStatus()
        {
            Parser.SaveOrUpDateStatus();
        }
    }
}
