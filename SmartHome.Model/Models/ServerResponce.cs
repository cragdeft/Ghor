using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Model.Models
{
   
    public class ServerResponce : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ServerResponceId { get; set; }
        public string Message { get; set; }
        public string Topic { get; set; }
        public string MessageId { get; set; }




    }
}
