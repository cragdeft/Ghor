﻿using Repository.Pattern.Ef6;
using SmartHome.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class CommandJson : Entity
    {
        #region Primitive Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CommandJsonId { get; set; }
        public int CommandId { get; set; }
        public String Command { get; set; }
        public int DeviceId { get; set; }
        public int DeviceUUId { get; set; }
        public bool Response { get; set; }
        public string DeviceVersion { get; set; }
        public string Mac { get; set; }
        public string EmailAddress { get; set; }

        public bool IsProcessed { get; set; }
        public CommandType CommandType { get; set; }
        public string ProcessFailReason { get; set; }
        #endregion

        #region Complex Properties
        public AuditFields AuditField { get; set; }
        #endregion

    }

}
