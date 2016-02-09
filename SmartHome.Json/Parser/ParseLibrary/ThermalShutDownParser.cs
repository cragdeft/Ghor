using SmartHome.Entity;

namespace SmartHome.Json.Parser.ParseLibrary
{
    public class ThermalShutDownParser: IParser
    {
        private BaseParser Parser { get; set; }
        public ThermalShutDownParser(CommandJsonEntity commandJsonEntity)
        {
            Parser = new BaseParser(commandJsonEntity);
        }
        public void Parse()
        {
            if (Parser.IsDeviceExists())
            {
                Parser.ParseInitiatorAndSetVersionValue();
                Parser.CurrentLoadStatusParse();
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
