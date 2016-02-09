using SmartHome.Entity;

namespace SmartHome.Json.Parser.ParseLibrary
{
    public class DimmingFeedbackEnableDisableParser : IParser
    {
        private BaseParser Parser { get; }

        public DimmingFeedbackEnableDisableParser(CommandJsonEntity commandJsonEntity)
        {
            Parser = new BaseParser(commandJsonEntity);
        }
        public void Parse()
        {
            if (Parser.IsDeviceExists())
            {
                Parser.ParseInitiatorAndSetVersionValue();
                Parser.DimmingFeedbackEnableDisableCommandParse();
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
