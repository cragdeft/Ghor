using Repository.Pattern.Repositories;
using Service.Pattern;
using SmartHome.Model.Models;
using SmartHome.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{
    public  class ServerResponceService : Service<ServerResponce>, IServerResponceService
    {
        private readonly IRepositoryAsync<ServerResponce> _repository;

        public ServerResponceService(IRepositoryAsync<ServerResponce> repository) : base(repository)
        {
            _repository = repository;
        }

        public IEnumerable<ServerResponce> ResponcesByMessageId(string MessageId)
        {
            throw new NotImplementedException();
        }
    }
}
