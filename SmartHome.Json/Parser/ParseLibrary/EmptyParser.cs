using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Json.Parser.ParseLibrary
{
    public class EmptyParser : IParser
    {
        public void Parse()
        {
            throw new NotImplementedException();
        }

        public void SaveOrUpdateStatus()
        {
            throw new NotImplementedException();
        }
    }
}
