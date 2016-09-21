using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
using SmartHome.Model.Enums;
using SmartHome.Model.ModelDataContext;
using SmartHome.Model.Models;
using SmartHome.Service;
using SmartHome.Service.Interfaces;
using SmartHome.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Data.Processor
{
    public class UserDatatProcessor : BaseDataProcessor
    {
        public dynamic _userEntity { get; private set; }

        public UserDatatProcessor(string jsonString, MessageReceivedFrom receivedFrom)
        {
            _receivedFrom = receivedFrom;
            _homeJsonMessage = jsonString;
            _userEntity = JsonDesrialized<dynamic>(jsonString);
        }
        public bool IsUserExist()
        {
            MessageLog messageLog = null;
            bool isSuccess = false;

            if (_userEntity == null)
                return false;

            using (IDataContextAsync context = new SmartHomeDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {

                var transactionRunner = new UnitOfWorkTransactionRunner(unitOfWork);
                try
                {
                    isSuccess = transactionRunner.RunSelectTransaction(() => new CommonService(unitOfWork).IsLoginIdUnique(Convert.ToString(_userEntity.Email)));
                    messageLog = transactionRunner.RunTransaction(() => new CommonService(unitOfWork).SaveMessageLog(_homeJsonMessage, _receivedFrom));
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    transactionRunner.RunTransaction(() => new CommonService(unitOfWork).UpdateMessageLog(messageLog, string.Empty));
                }
            }
            return isSuccess;
        }
    }
}
