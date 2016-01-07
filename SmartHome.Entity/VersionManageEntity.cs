using AutoMapper;
using SmartHome.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{



    public class VersionManageEntity : VersionEntity
    {
        public VersionManageEntity()
            : base()
        {

        }
        public VersionManageEntity(Version model)
            : base(model)
        {
        }        

        public Version ToDalEntity(VersionEntity model)
        {
            Mapper.CreateMap<VersionEntity, Version>();
            return Mapper.Map<VersionEntity, Version>(model);

        }

    }
}
