﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{
    public interface IHomeUpdateJsonParserService<T> where T : class
    {
        T UpdateJsonData();

    }
}
