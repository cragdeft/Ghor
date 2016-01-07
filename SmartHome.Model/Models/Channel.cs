using Repository.Pattern.Ef6;
using SmartHome.Model.SmartHome.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
    public class Channel : Entity
    {
        public Channel()
        {
            NO_LOAD = LoadType.NO_LOAD;
            NON_DIMMABLE_BULB = LoadType.NON_DIMMABLE_BULB;
        }

        #region Primitive Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChannelId { get; set; }
        public int Id { get; set; }
        public int DId { get; set; }
        public int ChannelNo { get; set; }
        public string LoadName { get; set; }
        public int Status { get; set; }
        public int Value { get; set; }
        public LoadType LoadType { get; set; }
        public LoadType NO_LOAD { get; set; }
        public LoadType NON_DIMMABLE_BULB { get; set; }
        #endregion

        #region Complex Properties
        public AuditFields AuditField { get; set; }
        #endregion

        #region Navigation Properties
        public virtual Device Device { get; set; }
        #endregion

    }

}
