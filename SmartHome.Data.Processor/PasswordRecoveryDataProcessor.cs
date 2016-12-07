using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Logging;
using SmartHome.Model.Enums;
using SmartHome.Model.ModelDataContext;
using SmartHome.Model.Models;
using SmartHome.Service;
using SmartHome.Service.UserInfoServices;
using SmartHome.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Data.Processor
{
    public class PasswordRecoveryDataProcessor : BaseDataProcessor
    {
        public dynamic _userEntity { get; private set; }
        public PasswordRecoveryRootObjectEntity _oRootObject { get; set; }

        public PasswordRecoveryDataProcessor(string jsonString, MessageReceivedFrom receivedFrom, PasswordRecoveryRootObjectEntity oRootObject)
        {
            _receivedFrom = receivedFrom;
            _homeJsonMessage = jsonString;
            _userEntity = JsonDesrialized<dynamic>(jsonString);
            _oRootObject = oRootObject;
        }
        public bool PasswordRecovery()
        {
            bool isSuccess = false;


            if (_userEntity == null)
                return false;

            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {

                var transactionRunner = new UnitOfWorkTransactionRunner(unitOfWork);
                try
                {
                    UserInfo oUserInfo = transactionRunner.RunSelectTransaction(() => new UserInfoRetrieverService(unitOfWork, _userEntity).RetriveUserData());
                    if (oUserInfo != null)
                    {
                        FillRootObject(oUserInfo);
                        isSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                }
            }
            return isSuccess;
        }

        private void FillRootObject(UserInfo oUserInfo)
        {
            _oRootObject.data.Password = oUserInfo.Password;
            _oRootObject.data.UserName = oUserInfo.UserName;
        }
    }
}
