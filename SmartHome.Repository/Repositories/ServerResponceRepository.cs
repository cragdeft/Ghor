using Repository.Pattern.Repositories;
using SmartHome.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace SmartHome.Repository.Repositories
{

    public static class ServerResponceRepository
    {
      
        public static IEnumerable<ServerResponce> GetResponcesByMessageId(this IRepository<ServerResponce> repository, string country)
        {
            var serverResponces = repository.GetRepository<ServerResponce>().Queryable();
           

            return serverResponces.AsEnumerable();
        }
    }
}
