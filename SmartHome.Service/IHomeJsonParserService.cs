using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Entity;
using SmartHome.Model.Models;

namespace SmartHome.Service
{
    public interface IHomeJsonParserService
    {
        bool SaveJsonData();
    }
}
