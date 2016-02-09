using SmartHome.Entity;

namespace SmartHome.Json.Parser.ParseLibrary
{
    public class LoadTypeSelectParser: IParser
    {
        private BaseParser Parser { get; }
        public LoadTypeSelectParser(CommandJsonEntity commandJsonEntity)
        {
            Parser = new BaseParser(commandJsonEntity);
        }
        public void Parse()
        {
            if (Parser.IsDeviceExists())
            {
                Parser.ParseInitiatorAndSetVersionValue();
                Parser.LoadTypeSelectCommandParse();
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
