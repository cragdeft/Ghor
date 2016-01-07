
using AutoMapper;
using SmartHome.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{

    public class VersionListEntity
    {
        public VersionListEntity()
        {
            this.Versions = new List<VersionEntity>();
        }
        public VersionListEntity(IEnumerable<Version> version)
        {
            this.Versions = new List<VersionEntity>();
            Mapper.CreateMap<Version, VersionEntity>();

            foreach (var modelItem in version)
            {
                VersionEntity userModel = Mapper.Map<Version, VersionEntity>(modelItem);
                this.Versions.Add(userModel);
            }
        }
        public List<VersionEntity> Versions { get; set; }
    }
}
