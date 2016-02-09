using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Json.Parser
{
    public interface IParserFactory
    {
        IParser GetParser(string parserType);
    }
}
