﻿using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.Enums;
using SmartHome.Model.ModelDataContext;
using SmartHome.Model.Models;
using SmartHome.Service;
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
                    if (isSuccess)
                    {
                        _oRootObject.data.Password = transactionRunner.RunSelectTransaction(() => new UserInfoService(unitOfWork).PasswordRecoveryByEmail(Convert.ToString(_userEntity.Email)));
                        _oRootObject.data.UserName = transactionRunner.RunSelectTransaction(() => new UserInfoService(unitOfWork).GetsUserInfosByEmailAndPassword(Convert.ToString(_userEntity.Email), _oRootObject.data.Password).UserName);
                    }
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
