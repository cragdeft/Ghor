using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Service.Pattern;
using SmartHome.Entity;
using SmartHome.Model.Models;
using SmartHome.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{
    public class DeviceService : Service<Device>, IDeviceService
    {
         private readonly IRepositoryAsync<Device> _repository;

        public DeviceService(IRepositoryAsync<Device> repository) : base(repository)
        {
            _repository = repository;
            //Mapper.CreateMap<VersionEntity, Version>();
        }

        public IEnumerable<Device> AddOrUpdateGraph(IEnumerable<Device> model)
        {
            foreach (var item in model)
            {
                base.InsertOrUpdateGraph(item);
            }

            return model;
        }
    }
}
