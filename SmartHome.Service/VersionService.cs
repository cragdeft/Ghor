
using Repository.Pattern.Repositories;
using Service.Pattern;
using SmartHome.Model.Models;
using SmartHome.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{


    public class VersionService : Service<Version>, IVersionService
    {
        private readonly IRepositoryAsync<Version> _repository;

        public VersionService(IRepositoryAsync<Version> repository) : base(repository)
        {
            _repository = repository;
        }
    }
}
