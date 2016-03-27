using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
    public class UserHomeLinkEntity
    {
        public int UserHomeLinkEntityId { get; set; }
        public string Id { get; set; }
        public string Home { get; set; }

        public string User { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsSynced { get; set; }

    }
}
