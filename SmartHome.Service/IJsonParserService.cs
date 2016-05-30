using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Entity;
using SmartHome.Model.Models;

namespace SmartHome.Service
{
    public interface IJsonParserService
    {
        bool IsRouterExists(SmartRouter router);
        void SaveHome(HomeEntity home);
        bool IsHomeExists(HomeEntity home);
        void UpdateHome(HomeEntity home);
    }
}
