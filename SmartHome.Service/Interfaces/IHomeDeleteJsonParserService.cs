using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{
    public interface IHomeDeleteJsonParserService<T>
    {
        T DeleteJsonData();

    }
}
