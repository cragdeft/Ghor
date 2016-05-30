using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.ModelDataContext;
using SmartHome.Service;
using SmartHome.Service.Interfaces;

namespace SmartHome.Json
{
    public class JsonParser
    {
        public HomeJsonEntity _homeJsonEntity { get; private set; }
        private IUnitOfWorkAsync _unitOfWorkAsync;
        private IHomeJsonParserService _homeJsonParserService;
        public JsonParser(string jsonString)
        {
            _homeJsonEntity = JsonDesrialized<HomeJsonEntity>(jsonString);
            InitializeParameters();
        }

        private void InitializeParameters()
        {
            IDataContextAsync context = new SmartHomeDataContext();
            _unitOfWorkAsync = new UnitOfWork(context);
            _homeJsonParserService = new HomeJsonParserService(_unitOfWorkAsync,_homeJsonEntity);
        }

        public void Save()
        {
            if (_homeJsonEntity == null)
                return;
            _homeJsonParserService.SaveJsonData();
        }

        public static T JsonDesrialized<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
