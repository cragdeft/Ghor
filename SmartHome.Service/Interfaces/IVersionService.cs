
using Service.Pattern;
using SmartHome.Entity;
using SmartHome.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service.Interfaces
{   

    public interface IVersionService : IService<Version>
    {
        VersionEntity Add(VersionEntity model);
        VersionEntity Modify(VersionEntity model);
        //Task<VersionEntity> ManageEntity(int Id);
        Task<IEnumerable<VersionEntity>> GetsAsync();
        Task<VersionEntity> GetAsync(int id);
        void Remove(string id);
        void Remove(VersionEntity entity);

    }
}
