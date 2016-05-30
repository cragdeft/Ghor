﻿using System;
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
        SmartRouterEntity GetRouter(string macAddress);
        void InsertHome(HomeEntity home);
        HomeEntity GetHome(int homeId);
        void UpdateHome(HomeEntity home);
        void UpdateRouter(SmartRouterEntity router);
        void SaveRouter(SmartRouterEntity router);
    }
}
