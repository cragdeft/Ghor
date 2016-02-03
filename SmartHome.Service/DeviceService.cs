using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Service.Pattern;
using SmartHome.Entity;
using SmartHome.Model.Models;
using SmartHome.Repository.Repositories;
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
        }

        public IEnumerable<Device> AddOrUpdateGraphRange(IEnumerable<Device> model)
        {           
            List<Device> deviceModel = new List<Device>();
            deviceModel =_repository.FillDeviceInformations(model, deviceModel);
            base.InsertOrUpdateGraphRange(deviceModel);
            return deviceModel;
        }

        public IEnumerable<DeviceInfoEntity> GetsDeviceAllInfo()
        {
            // add business logic here
            return _repository.GetsAllDevice();
        }
    }
}
