﻿using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{   
    public class UserRoomLink : Entity
    {
        [Key, Column(Order = 0)]
        public int AppsRoomId { get; set; }
        [Key, Column(Order = 1)]
        public int AppsUserId { get; set; }

        public int AppsUserRoomLinkId { get; set; }

        public virtual Room Room { get; set; }
        public virtual UserInfo UserInfo { get; set; }       
        public bool IsSynced { get; set; }



    }
}
