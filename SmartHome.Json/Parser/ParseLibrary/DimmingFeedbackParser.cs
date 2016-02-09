using SmartHome.Entity;

namespace SmartHome.Json.Parser.ParseLibrary
{
    public class DimmingFeedbackParser:IParser
    {
        private BaseParser Parser { get; }
        public DimmingFeedbackParser(CommandJsonEntity commandJsonEntity)
        {
            Parser = new BaseParser(commandJsonEntity);
        }
        public void Parse()
        {
            if (Parser.IsDeviceExists())
            {
                Parser.ParseInitiatorAndSetVersionValue();
                Parser.DimmingFeedbackCommandParse();
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
