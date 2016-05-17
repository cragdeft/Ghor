using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class UserRoomLinkEntity
    {
        public int UserRoomLinkEntityId { get; set; }
        public string Id { get; set; }
        public string User { get; set; }

        public string Room { get; set; }

        public bool IsSynced { get; set; }
    }
}
