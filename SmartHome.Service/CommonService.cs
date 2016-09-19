using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Model.Enums;
using SmartHome.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service
{
    public class CommonService
    {
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IRepositoryAsync<Home> _homeRepository;
        private readonly IRepositoryAsync<Room> _roomRepository;
        private readonly IRepositoryAsync<UserInfo> _userRepository;
        private readonly IRepositoryAsync<UserRoomLink> _userRoomLinkRepository;
        private readonly IRepositoryAsync<UserHomeLink> _userHomeRepository;
        private readonly IRepositoryAsync<MessageLog> _mqttMessageLogRepository;

        public CommonService(IUnitOfWorkAsync unitOfWorkAsync)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _homeRepository = _unitOfWorkAsync.RepositoryAsync<Model.Models.Home>();
            _roomRepository = _unitOfWorkAsync.RepositoryAsync<Room>();
            _userRepository = _unitOfWorkAsync.RepositoryAsync<UserInfo>();
            _userRoomLinkRepository = _unitOfWorkAsync.RepositoryAsync<UserRoomLink>();
            _userHomeRepository = _unitOfWorkAsync.RepositoryAsync<UserHomeLink>();
            _mqttMessageLogRepository = _unitOfWorkAsync.RepositoryAsync<MessageLog>();
        }

        public Home GetHome(string passPhrase)
        {
            return _homeRepository.Queryable().Where(p => p.PassPhrase == passPhrase).FirstOrDefault();
        }

        public Home GetHomeWithRooms(string passPhrase)
        {
            return _homeRepository.Queryable().Include(x => x.Rooms).Where(p => p.PassPhrase == passPhrase).FirstOrDefault();
        }

        public UserInfo GetUser(string email)
        {
            UserInfo user = _userRepository
                .Queryable().Include(x => x.UserRoomLinks).Include(x => x.UserHomeLinks).Where(u => u.Email == email).FirstOrDefault();

            return user;
        }

        public UserInfo GetUserByEmail(string email)
        {
            UserInfo user = _userRepository
                .Queryable().Where(u => u.Email == email).FirstOrDefault();

            return user;
        }


        public MessageLog SaveMessageLog(string homeJsonMessage, MessageReceivedFrom receivedFrom)
        {
            _unitOfWorkAsync.BeginTransaction();
            MessageLog messageLog = new MessageLog();

            try
            {
                DateTime processTime = DateTime.Now;
                var entity = new MessageLog();
                entity.Message = homeJsonMessage;
                entity.ReceivedFrom = receivedFrom;
                entity.UserInfoIds = string.Empty;
                entity.AuditField = new AuditFields("admin", processTime, "admin", processTime);
                entity.ObjectState = ObjectState.Added;
                _mqttMessageLogRepository.Insert(entity);

                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();

                messageLog = entity;

            }

            catch (Exception ex)
            {
                _unitOfWorkAsync.Rollback();
            }

            return messageLog;
        }


        public MessageLog UpdateMessageLog(MessageLog entity, string passPhrase)
        {
            _unitOfWorkAsync.BeginTransaction();
            MessageLog messageLog = new MessageLog();

            try
            {
                DateTime processTime = DateTime.Now;
                //var entity = _messageLog;
                //entity.Message = _homeJsonMessage;
                //entity.ReceivedFrom = _receivedFrom;
                entity.UserInfoIds = GetUserInfosByHomePassphase(passPhrase);
                entity.AuditField = new AuditFields("admin", entity.AuditField.InsertedDateTime, "admin", processTime);
                entity.ObjectState = ObjectState.Modified;
                _mqttMessageLogRepository.Update(entity);

                var changes = _unitOfWorkAsync.SaveChanges();
                _unitOfWorkAsync.Commit();

                messageLog = entity;

            }
            catch (Exception ex)
            {
                _unitOfWorkAsync.Rollback();
            }
            return messageLog;
        }

        public string GetUserInfosByHomePassphase(string passPhrase)
        {
            string userInfos = string.Empty;

            Home home = _homeRepository.Queryable().Where(p => p.PassPhrase == passPhrase).FirstOrDefault();
            if (home != null)
            {
                var temp = _userHomeRepository.Queryable().Where(p => p.Home.HomeId == home.HomeId).DefaultIfEmpty().Select(q => q.UserInfo.UserInfoId);
                if (temp != null)
                {
                    userInfos = string.Join(",", temp);
                }
            }
            return userInfos;
        }
    }
}
