
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
        bool IsExists(int key);              
        Task<IEnumerable<VersionEntity>> GetsAsync();
        Task<VersionEntity> GetAsync(int id);

        VersionEntity Add(VersionEntity model);
        IEnumerable<Version> AddOrUpdateGraphRange(IEnumerable<Version> model);
        VersionEntity Modify(VersionEntity model);
        void Remove(string id);
        void Remove(VersionEntity entity);

    }
}
