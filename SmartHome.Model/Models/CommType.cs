﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class CommType
    {
        public int CommTypeId { get; set; }
        public string Comm { get; set; }
        public bool IsActive { get; set; }
        public AuditFields AuditField { get; set; }
        public virtual Home Home { get; set; }
    }
}