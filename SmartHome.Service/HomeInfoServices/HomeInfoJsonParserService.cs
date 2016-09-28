using AutoMapper;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{
    public class HomeInfoJsonParserService : IHomeJsonParserService<Home>
    {
        #region PrivateProperty
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<Home> _homeRepository;

        public HomeJsonEntity _homeJsonEntity { get; private set; }
        public string _homeJsonMessage { get; private set; }
        public MessageReceivedFrom _receivedFrom { get; private set; }

        #endregion
        public HomeInfoJsonParserService(IUnitOfWorkAsync unitOfWorkAsync, HomeJsonEntity homeJsonEntity, string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Home>();

            _homeJsonEntity = homeJsonEntity;
            _homeJsonMessage = homeJsonMessage;
            _receivedFrom = receivedFrom;
        }
        public Home SaveJsonData()
        {
            Home home = null;
            try
            {
                if (_homeJsonEntity.Home.Count == 0)
                {
                    return home;
                }

                string passPhrase = _homeJsonEntity.Home.FirstOrDefault().PassPhrase;

                HomeEntity homeEntity = _homeJsonEntity.Home.FirstOrDefault();
                Home dbHome = new CommonService(_unitOfWorkAsync).GetHome(passPhrase);

                if (dbHome != null)
                {
                    IHomeUpdateJsonParserService<Home> service = new HomeInfoUpdateJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.UpdateHome);
                    home = service.UpdateJsonData();
                }
                else
                {
                    IHomeJsonParserService<Home> service = new HomeInfoNewEntryJsonParserService(_unitOfWorkAsync, _homeJsonEntity, _homeJsonMessage, MessageReceivedFrom.NewHome);
                    home = service.SaveJsonData();
                }


            }
            catch (Exception ex)
            {
                return null;
            }
            return home;
        }

    }
}
