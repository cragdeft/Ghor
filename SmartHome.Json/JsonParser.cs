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
        public RoomJsonEntity _roomJsonEntity { get; set; }
        private IUnitOfWorkAsync _unitOfWorkAsync;
        private IJsonParserService _jsonParserService;
        public JsonParser(RoomJsonEntity roomJsonEntity)
        {
            _roomJsonEntity = roomJsonEntity;
            InitializeParameters(roomJsonEntity);
        }

        private void InitializeParameters(RoomJsonEntity roomJsonEntity)
        {
            IDataContextAsync context = new SmartHomeDataContext();
            _unitOfWorkAsync = new UnitOfWork(context);
            _jsonParserService = new JsonParserService(_unitOfWorkAsync);
        }

        public void Parse()
        {
            if (_roomJsonEntity == null)
                return;

            if (_roomJsonEntity.RouterInfo[0] != null)
            {
                HomeEntity home = _roomJsonEntity.Home.Find(x => x.Id == _roomJsonEntity.RouterInfo[0].HId);
                if (!_jsonParserService.IsHomeExists(home))
                {
                    home.SmartRouter.Add(_roomJsonEntity.RouterInfo[0]);
                    _jsonParserService.SaveHome(home);
                }
                else
                {
                    home.SmartRouter.Add(_roomJsonEntity.RouterInfo[0]);
                    _jsonParserService.UpdateHome(home);
                }
            }
        }

        public static T JsonDesrialized<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
