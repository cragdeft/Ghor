using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.ModelDataContext;
using SmartHome.Model.Models;
using SmartHome.Repository.Repositories;
using SmartHome.Service;
using SmartHome.Service.Interfaces;
using SmartHome.Utility.EncryptionAndDecryption;
using SmartHome.WebAPI.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using SmartHome.Model.ViewModels;
using SmartHome.Json;
using SmartHome.Model.Enums;
using System.Diagnostics;

namespace SmartHome.WebAPI.Controllers
{
    public class UserInfoController : ApiController
    {
        public UserInfoController()
        {

        }

        [Route("api/RegisterUser")]
        public HttpResponseMessage RegisterUser(JObject encryptedString)
        {
            #region Initialization

            HttpResponseMessage response;
            LoginObjectEntity oLoginObject = new LoginObjectEntity();
            string msg = string.Empty;


            msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
            if (string.IsNullOrEmpty(msg))
            {
                return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
            }
            oLoginObject = JsonConvert.DeserializeObject<LoginObjectEntity>(msg);
            var oUserInfo = oLoginObject.UserInfo.First();

            #endregion

            using (IDataContextAsync context = new SmartHomeDataContext())
            {
                using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
                {
                    IUserInfoService service = new UserInfoService(unitOfWork);
                    response = ProcessGetRegisteredUser(oUserInfo, unitOfWork, service);
                }
            }
            return response;
        }

        [Route("api/GetUserInfo")]
        [HttpPost]
        public HttpResponseMessage GetUserInfo(JObject encryptedString)
        {
            #region Initialization

            LoginRootObjectEntity oRootObject = new LoginRootObjectEntity();
            HttpResponseMessage response = new HttpResponseMessage();
            LoginObjectEntity oLoginObject = new LoginObjectEntity();
            string msg = string.Empty;

            msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
            if (string.IsNullOrEmpty(msg))
            {
                return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
            }

            #endregion
            using (IDataContextAsync context = new SmartHomeDataContext())
            {
                using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
                {
                    IUserInfoService service = new UserInfoService(unitOfWork);
                    IConfigurationParserManagerService serviceConfigure = new ConfigurationParserManagerService(unitOfWork);
                    try
                    {
                        response = ProcessUserInfomations(oRootObject, response, oLoginObject, msg, service, serviceConfigure);
                    }
                    catch (Exception ex)
                    {
                        oRootObject.data = new LoginObjectEntity();
                        response = PrepareJsonResponse(oRootObject, ex.ToString(), HttpStatusCode.BadRequest);
                    }
                }
            }
            return response;
        }

        [Route("api/IsUserExist")]
        [HttpPost]
        public HttpResponseMessage IsUserExist(JObject encryptedString)
        {

            #region Initialization

            HttpResponseMessage response;
            string msg = string.Empty;

            msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
            if (string.IsNullOrEmpty(msg))
            {
                return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
            }
            var tempJsonObject = JsonConvert.DeserializeObject<dynamic>(msg);

            #endregion

            using (IDataContextAsync context = new SmartHomeDataContext())
            {
                using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
                {
                    IUserInfoService service = new UserInfoService(unitOfWork);
                    response = ProcessIsUserExist(Convert.ToString(tempJsonObject.Email), service);
                }
            }
            return response;
        }

        [Route("api/ChangePassword")]
        [HttpPost]
        public HttpResponseMessage ChangePassword(JObject encryptedString)
        {
            #region Initialization

            HttpResponseMessage response;
            string msg = string.Empty;
            //"{\"Email\":\"kk.test@apl.com\",\"Password\":\"6ZlPF3qG/+LcC5brkDMZgA==\"}"
            msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
            if (string.IsNullOrEmpty(msg))
            {
                return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
            }
            var tempJsonObject = JsonConvert.DeserializeObject<dynamic>(msg);

            #endregion

            using (IDataContextAsync context = new SmartHomeDataContext())
            {
                using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
                {
                    IUserInfoService service = new UserInfoService(unitOfWork);
                    response = ProcessPasswordUpdate(Convert.ToString(tempJsonObject.Email), Convert.ToString(tempJsonObject.Password), service);
                }
            }
            return response;
        }

        [Route("api/ConfigurationProcess")]
        [HttpPost]
        public HttpResponseMessage ConfigurationProcess(JObject encryptedString)
        {
            HttpResponseMessage response;
            PasswordRecoveryRootObjectEntity oRootObject = new PasswordRecoveryRootObjectEntity();
            try
            {
                #region Initialization

                oRootObject.data = new PasswordRecoveryObjectEntity();
                string msg = string.Empty;

                //Debug.WriteLine(SecurityManager.Encrypt("{\"Version\":[{\"Id\":1,\"AppName\":\"SmartHome\",\"AppVersion\":\"2.0.0\",\"AuthCode\":\"0123456789ABCDEF\",\"PassPhrase\":\"\",\"IsSynced\":0}],\"VersionDetails\":[{\"Id\":1,\"VersionId\":1,\"HardwareVersion\":\"00\",\"DeviceType\":1,\"IsSynced\":0},{\"Id\":2,\"VersionId\":1,\"HardwareVersion\":\"00\",\"DeviceType\":2,\"IsSynced\":0},{\"Id\":3,\"VersionId\":1,\"HardwareVersion\":\"00\",\"DeviceType\":6,\"IsSynced\":0},{\"Id\":4,\"VersionId\":1,\"HardwareVersion\":\"00\",\"DeviceType\":4,\"IsSynced\":0},{\"Id\":5,\"VersionId\":1,\"HardwareVersion\":\"00\",\"DeviceType\":3,\"IsSynced\":0},{\"Id\":6,\"VersionId\":1,\"HardwareVersion\":\"01\",\"DeviceType\":1,\"IsSynced\":0},{\"Id\":7,\"VersionId\":1,\"HardwareVersion\":\"01\",\"DeviceType\":2,\"IsSynced\":0},{\"Id\":8,\"VersionId\":1,\"HardwareVersion\":\"01\",\"DeviceType\":6,\"IsSynced\":0}],\"Device\":[{\"Id\":1,\"DeviceId\":32771,\"DeviceHash\":1626016732,\"DeviceType\":1,\"DeviceName\":\"3Lab\",\"Version\":\"01\",\"Room\":3,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":2,\"DeviceId\":32772,\"DeviceHash\":1021532556,\"DeviceType\":1,\"DeviceName\":\"3W1\",\"Version\":\"01\",\"Room\":3,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":3,\"DeviceId\":32773,\"DeviceHash\":880874046,\"DeviceType\":1,\"DeviceName\":\"3W2\",\"Version\":\"01\",\"Room\":3,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":4,\"DeviceId\":32774,\"DeviceHash\":1845251746,\"DeviceType\":1,\"DeviceName\":\"3W3\",\"Version\":\"01\",\"Room\":3,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":5,\"DeviceId\":32775,\"DeviceHash\":2001133491,\"DeviceType\":1,\"DeviceName\":\"3B1\",\"Version\":\"01\",\"Room\":3,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":6,\"DeviceId\":32776,\"DeviceHash\":807085420,\"DeviceType\":1,\"DeviceName\":\"3B2\",\"Version\":\"01\",\"Room\":3,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":7,\"DeviceId\":32777,\"DeviceHash\":1576003555,\"DeviceType\":1,\"DeviceName\":\"3B3\",\"Version\":\"01\",\"Room\":3,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":8,\"DeviceId\":32779,\"DeviceHash\":345368058,\"DeviceType\":1,\"DeviceName\":\"3W5\",\"Version\":\"01\",\"Room\":3,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":9,\"DeviceId\":32781,\"DeviceHash\":902115277,\"DeviceType\":1,\"DeviceName\":\"3SC\",\"Version\":\"01\",\"Room\":3,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":10,\"DeviceId\":32782,\"DeviceHash\":476224557,\"DeviceType\":1,\"DeviceName\":\"2SC\",\"Version\":\"01\",\"Room\":4,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":11,\"DeviceId\":32783,\"DeviceHash\":572939360,\"DeviceType\":1,\"DeviceName\":\"2W1\",\"Version\":\"01\",\"Room\":4,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":12,\"DeviceId\":32784,\"DeviceHash\":584774515,\"DeviceType\":1,\"DeviceName\":\"2W2\",\"Version\":\"01\",\"Room\":4,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":13,\"DeviceId\":32786,\"DeviceHash\":1178216090,\"DeviceType\":1,\"DeviceName\":\"2B1\",\"Version\":\"01\",\"Room\":4,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":14,\"DeviceId\":32787,\"DeviceHash\":145554361,\"DeviceType\":1,\"DeviceName\":\"2K1\",\"Version\":\"01\",\"Room\":4,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":15,\"DeviceId\":32788,\"DeviceHash\":1777485768,\"DeviceType\":1,\"DeviceName\":\"2K2\",\"Version\":\"01\",\"Room\":4,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":16,\"DeviceId\":32791,\"DeviceHash\":1422024860,\"DeviceType\":1,\"DeviceName\":\"2M2\",\"Version\":\"01\",\"Room\":4,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":17,\"DeviceId\":32793,\"DeviceHash\":1031698027,\"DeviceType\":1,\"DeviceName\":\"4SC\",\"Version\":\"01\",\"Room\":5,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":18,\"DeviceId\":32794,\"DeviceHash\":503053845,\"DeviceType\":1,\"DeviceName\":\"4W1\",\"Version\":\"01\",\"Room\":5,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":19,\"DeviceId\":32795,\"DeviceHash\":845684496,\"DeviceType\":1,\"DeviceName\":\"4W2\",\"Version\":\"01\",\"Room\":5,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":20,\"DeviceId\":32798,\"DeviceHash\":2049127831,\"DeviceType\":1,\"DeviceName\":\"4CEO1\",\"Version\":\"01\",\"Room\":5,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":21,\"DeviceId\":32799,\"DeviceHash\":226911808,\"DeviceType\":1,\"DeviceName\":\"4CEO2\",\"Version\":\"01\",\"Room\":5,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":22,\"DeviceId\":32800,\"DeviceHash\":1583718343,\"DeviceType\":1,\"DeviceName\":\"4Lab1\",\"Version\":\"01\",\"Room\":5,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":23,\"DeviceId\":32801,\"DeviceHash\":1784880192,\"DeviceType\":1,\"DeviceName\":\"4Lab2\",\"Version\":\"01\",\"Room\":5,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":24,\"DeviceId\":32810,\"DeviceHash\":319932684,\"DeviceType\":2,\"DeviceName\":\"SMRB12 319932684\",\"Version\":\"01\",\"Room\":2,\"IsDeleted\":0,\"Watt\":12,\"IsSynced\":0},{\"Id\":25,\"DeviceId\":32811,\"DeviceHash\":1347527058,\"DeviceType\":1,\"DeviceName\":\"3W4\",\"Version\":\"01\",\"Room\":3,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":26,\"DeviceId\":32812,\"DeviceHash\":2043201418,\"DeviceType\":1,\"DeviceName\":\"2M1\",\"Version\":\"01\",\"Room\":4,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":27,\"DeviceId\":32813,\"DeviceHash\":290385692,\"DeviceType\":1,\"DeviceName\":\"2HR\",\"Version\":\"01\",\"Room\":4,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":28,\"DeviceId\":32815,\"DeviceHash\":40911269,\"DeviceType\":1,\"DeviceName\":\"4B1\",\"Version\":\"01\",\"Room\":5,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":29,\"DeviceId\":32816,\"DeviceHash\":1434984405,\"DeviceType\":1,\"DeviceName\":\"4Store\",\"Version\":\"01\",\"Room\":5,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":30,\"DeviceId\":32817,\"DeviceHash\":2008438047,\"DeviceType\":1,\"DeviceName\":\"4B2\",\"Version\":\"01\",\"Room\":5,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":31,\"DeviceId\":32818,\"DeviceHash\":1994686185,\"DeviceType\":1,\"DeviceName\":\"4B3\",\"Version\":\"01\",\"Room\":5,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":32,\"DeviceId\":32819,\"DeviceHash\":1439922582,\"DeviceType\":1,\"DeviceName\":\"4W3\",\"Version\":\"01\",\"Room\":5,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":33,\"DeviceId\":32820,\"DeviceHash\":1011227448,\"DeviceType\":1,\"DeviceName\":\"4W4\",\"Version\":\"01\",\"Room\":5,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0},{\"Id\":34,\"DeviceId\":32821,\"DeviceHash\":1966280428,\"DeviceType\":1,\"DeviceName\":\"TestGround\",\"Version\":\"01\",\"Room\":2,\"IsDeleted\":0,\"Watt\":0,\"IsSynced\":0}],\"DeviceStatus\":[{\"Id\":1,\"DeviceTableId\":1,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":2,\"DeviceTableId\":1,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":3,\"DeviceTableId\":1,\"StatusType\":5,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":4,\"DeviceTableId\":2,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":5,\"DeviceTableId\":2,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":6,\"DeviceTableId\":2,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":7,\"DeviceTableId\":3,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":8,\"DeviceTableId\":3,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":9,\"DeviceTableId\":3,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":10,\"DeviceTableId\":4,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":11,\"DeviceTableId\":4,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":12,\"DeviceTableId\":4,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":13,\"DeviceTableId\":5,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":14,\"DeviceTableId\":5,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":15,\"DeviceTableId\":5,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":16,\"DeviceTableId\":6,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":17,\"DeviceTableId\":6,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":18,\"DeviceTableId\":6,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":19,\"DeviceTableId\":7,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":20,\"DeviceTableId\":7,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":21,\"DeviceTableId\":7,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":22,\"DeviceTableId\":8,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":23,\"DeviceTableId\":8,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":24,\"DeviceTableId\":8,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":25,\"DeviceTableId\":9,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":26,\"DeviceTableId\":9,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":27,\"DeviceTableId\":9,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":28,\"DeviceTableId\":10,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":29,\"DeviceTableId\":10,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":30,\"DeviceTableId\":10,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":31,\"DeviceTableId\":11,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":32,\"DeviceTableId\":11,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":33,\"DeviceTableId\":11,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":34,\"DeviceTableId\":12,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":35,\"DeviceTableId\":12,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":36,\"DeviceTableId\":12,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":37,\"DeviceTableId\":13,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":38,\"DeviceTableId\":13,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":39,\"DeviceTableId\":13,\"StatusType\":5,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":40,\"DeviceTableId\":14,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":41,\"DeviceTableId\":14,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":42,\"DeviceTableId\":14,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":43,\"DeviceTableId\":15,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":44,\"DeviceTableId\":15,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":45,\"DeviceTableId\":15,\"StatusType\":5,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":46,\"DeviceTableId\":16,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":47,\"DeviceTableId\":16,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":48,\"DeviceTableId\":16,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":49,\"DeviceTableId\":17,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":50,\"DeviceTableId\":17,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":51,\"DeviceTableId\":17,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":52,\"DeviceTableId\":18,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":53,\"DeviceTableId\":18,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":54,\"DeviceTableId\":18,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":55,\"DeviceTableId\":19,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":56,\"DeviceTableId\":19,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":57,\"DeviceTableId\":19,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":58,\"DeviceTableId\":20,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":59,\"DeviceTableId\":20,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":60,\"DeviceTableId\":20,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":61,\"DeviceTableId\":21,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":62,\"DeviceTableId\":21,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":63,\"DeviceTableId\":21,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":64,\"DeviceTableId\":22,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":65,\"DeviceTableId\":22,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":66,\"DeviceTableId\":22,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":67,\"DeviceTableId\":23,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":68,\"DeviceTableId\":23,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":69,\"DeviceTableId\":23,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":70,\"DeviceTableId\":24,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":71,\"DeviceTableId\":25,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":72,\"DeviceTableId\":25,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":73,\"DeviceTableId\":25,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":74,\"DeviceTableId\":26,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":75,\"DeviceTableId\":26,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":76,\"DeviceTableId\":26,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":77,\"DeviceTableId\":27,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":78,\"DeviceTableId\":27,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":79,\"DeviceTableId\":27,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":80,\"DeviceTableId\":28,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":81,\"DeviceTableId\":28,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":82,\"DeviceTableId\":28,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":83,\"DeviceTableId\":29,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":84,\"DeviceTableId\":29,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":85,\"DeviceTableId\":29,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":86,\"DeviceTableId\":30,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":87,\"DeviceTableId\":30,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":88,\"DeviceTableId\":30,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":89,\"DeviceTableId\":31,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":90,\"DeviceTableId\":31,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":91,\"DeviceTableId\":31,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":92,\"DeviceTableId\":32,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":93,\"DeviceTableId\":32,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":94,\"DeviceTableId\":32,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":95,\"DeviceTableId\":33,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":96,\"DeviceTableId\":33,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":97,\"DeviceTableId\":33,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":98,\"DeviceTableId\":34,\"StatusType\":53,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":99,\"DeviceTableId\":34,\"StatusType\":55,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":100,\"DeviceTableId\":34,\"StatusType\":5,\"StatusValue\":0,\"IsSynced\":0}],\"Channel\":[{\"Id\":1,\"DeviceTableId\":1,\"ChannelNo\":1,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":2,\"DeviceTableId\":1,\"ChannelNo\":2,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":3,\"DeviceTableId\":1,\"ChannelNo\":3,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":4,\"DeviceTableId\":1,\"ChannelNo\":4,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":5,\"DeviceTableId\":1,\"ChannelNo\":5,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":6,\"DeviceTableId\":1,\"ChannelNo\":6,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":7,\"DeviceTableId\":2,\"ChannelNo\":1,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":8,\"DeviceTableId\":2,\"ChannelNo\":2,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":9,\"DeviceTableId\":2,\"ChannelNo\":3,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":10,\"DeviceTableId\":2,\"ChannelNo\":4,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":11,\"DeviceTableId\":2,\"ChannelNo\":5,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":12,\"DeviceTableId\":2,\"ChannelNo\":6,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":13,\"DeviceTableId\":3,\"ChannelNo\":1,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":14,\"DeviceTableId\":3,\"ChannelNo\":2,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":15,\"DeviceTableId\":3,\"ChannelNo\":3,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":16,\"DeviceTableId\":3,\"ChannelNo\":4,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":17,\"DeviceTableId\":3,\"ChannelNo\":5,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":18,\"DeviceTableId\":3,\"ChannelNo\":6,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":19,\"DeviceTableId\":4,\"ChannelNo\":1,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":20,\"DeviceTableId\":4,\"ChannelNo\":2,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":21,\"DeviceTableId\":4,\"ChannelNo\":3,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":22,\"DeviceTableId\":4,\"ChannelNo\":4,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":23,\"DeviceTableId\":4,\"ChannelNo\":5,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":24,\"DeviceTableId\":4,\"ChannelNo\":6,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":25,\"DeviceTableId\":5,\"ChannelNo\":1,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":26,\"DeviceTableId\":5,\"ChannelNo\":2,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":27,\"DeviceTableId\":5,\"ChannelNo\":3,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":28,\"DeviceTableId\":5,\"ChannelNo\":4,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":29,\"DeviceTableId\":5,\"ChannelNo\":5,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":30,\"DeviceTableId\":5,\"ChannelNo\":6,\"LoadType\":2,\"LoadName\":\"Dimlight\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":31,\"DeviceTableId\":6,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":32,\"DeviceTableId\":6,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":33,\"DeviceTableId\":6,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":34,\"DeviceTableId\":6,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":35,\"DeviceTableId\":6,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":36,\"DeviceTableId\":6,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":37,\"DeviceTableId\":7,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":38,\"DeviceTableId\":7,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":39,\"DeviceTableId\":7,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":40,\"DeviceTableId\":7,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":41,\"DeviceTableId\":7,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":42,\"DeviceTableId\":7,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":43,\"DeviceTableId\":8,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":44,\"DeviceTableId\":8,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":45,\"DeviceTableId\":8,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":46,\"DeviceTableId\":8,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":47,\"DeviceTableId\":8,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":48,\"DeviceTableId\":8,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":49,\"DeviceTableId\":9,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":50,\"DeviceTableId\":9,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":51,\"DeviceTableId\":9,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":52,\"DeviceTableId\":9,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":53,\"DeviceTableId\":9,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":54,\"DeviceTableId\":9,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":55,\"DeviceTableId\":10,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":56,\"DeviceTableId\":10,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":57,\"DeviceTableId\":10,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":58,\"DeviceTableId\":10,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":59,\"DeviceTableId\":10,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":60,\"DeviceTableId\":10,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":61,\"DeviceTableId\":11,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":62,\"DeviceTableId\":11,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":63,\"DeviceTableId\":11,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":64,\"DeviceTableId\":11,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":65,\"DeviceTableId\":11,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":66,\"DeviceTableId\":11,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":67,\"DeviceTableId\":12,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":68,\"DeviceTableId\":12,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":69,\"DeviceTableId\":12,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":70,\"DeviceTableId\":12,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":71,\"DeviceTableId\":12,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":72,\"DeviceTableId\":12,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":73,\"DeviceTableId\":13,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":74,\"DeviceTableId\":13,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":75,\"DeviceTableId\":13,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":76,\"DeviceTableId\":13,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":77,\"DeviceTableId\":13,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":78,\"DeviceTableId\":13,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":79,\"DeviceTableId\":14,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":80,\"DeviceTableId\":14,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":81,\"DeviceTableId\":14,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":82,\"DeviceTableId\":14,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":83,\"DeviceTableId\":14,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":84,\"DeviceTableId\":14,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":85,\"DeviceTableId\":15,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":86,\"DeviceTableId\":15,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":87,\"DeviceTableId\":15,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":88,\"DeviceTableId\":15,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":89,\"DeviceTableId\":15,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":90,\"DeviceTableId\":15,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":91,\"DeviceTableId\":23,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":92,\"DeviceTableId\":23,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":93,\"DeviceTableId\":23,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":94,\"DeviceTableId\":23,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":95,\"DeviceTableId\":23,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":96,\"DeviceTableId\":23,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":97,\"DeviceTableId\":22,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":98,\"DeviceTableId\":22,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":99,\"DeviceTableId\":22,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":100,\"DeviceTableId\":22,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":101,\"DeviceTableId\":22,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":102,\"DeviceTableId\":22,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":103,\"DeviceTableId\":21,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":104,\"DeviceTableId\":21,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":105,\"DeviceTableId\":21,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":106,\"DeviceTableId\":21,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":107,\"DeviceTableId\":21,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":108,\"DeviceTableId\":21,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":109,\"DeviceTableId\":20,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":110,\"DeviceTableId\":20,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":111,\"DeviceTableId\":20,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":112,\"DeviceTableId\":20,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":113,\"DeviceTableId\":20,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":114,\"DeviceTableId\":20,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":115,\"DeviceTableId\":19,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":116,\"DeviceTableId\":19,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":117,\"DeviceTableId\":19,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":118,\"DeviceTableId\":19,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":119,\"DeviceTableId\":19,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":120,\"DeviceTableId\":19,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":121,\"DeviceTableId\":18,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":122,\"DeviceTableId\":18,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":123,\"DeviceTableId\":18,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":124,\"DeviceTableId\":18,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":125,\"DeviceTableId\":18,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":126,\"DeviceTableId\":18,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":127,\"DeviceTableId\":17,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":128,\"DeviceTableId\":17,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":129,\"DeviceTableId\":17,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":130,\"DeviceTableId\":17,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":131,\"DeviceTableId\":17,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":132,\"DeviceTableId\":17,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":133,\"DeviceTableId\":16,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":134,\"DeviceTableId\":16,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":135,\"DeviceTableId\":16,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":136,\"DeviceTableId\":16,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":137,\"DeviceTableId\":16,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":138,\"DeviceTableId\":16,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":139,\"DeviceTableId\":25,\"ChannelNo\":1,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":140,\"DeviceTableId\":25,\"ChannelNo\":2,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":141,\"DeviceTableId\":25,\"ChannelNo\":3,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":142,\"DeviceTableId\":25,\"ChannelNo\":4,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":143,\"DeviceTableId\":25,\"ChannelNo\":5,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0},{\"Id\":144,\"DeviceTableId\":25,\"ChannelNo\":6,\"LoadType\":1,\"LoadName\":\"Light\",\"LoadWatt\":0,\"IsSynced\":0}],\"NextAssociatedDeviceId\":[{\"NextDeviceId\":32823,\"IsSynced\":0}],\"ChannelStatus\":[{\"Id\":1,\"ChannelTableId\":1,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":2,\"ChannelTableId\":1,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":3,\"ChannelTableId\":2,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":4,\"ChannelTableId\":2,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":5,\"ChannelTableId\":1,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":6,\"ChannelTableId\":3,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":7,\"ChannelTableId\":3,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":8,\"ChannelTableId\":2,\"StatusType\":2,\"StatusValue\":160,\"IsSynced\":0},{\"Id\":9,\"ChannelTableId\":4,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":10,\"ChannelTableId\":4,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":11,\"ChannelTableId\":3,\"StatusType\":2,\"StatusValue\":160,\"IsSynced\":0},{\"Id\":12,\"ChannelTableId\":5,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":13,\"ChannelTableId\":5,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":14,\"ChannelTableId\":4,\"StatusType\":2,\"StatusValue\":160,\"IsSynced\":0},{\"Id\":15,\"ChannelTableId\":6,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":16,\"ChannelTableId\":6,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":17,\"ChannelTableId\":5,\"StatusType\":2,\"StatusValue\":160,\"IsSynced\":0},{\"Id\":18,\"ChannelTableId\":6,\"StatusType\":2,\"StatusValue\":160,\"IsSynced\":0},{\"Id\":19,\"ChannelTableId\":7,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":20,\"ChannelTableId\":7,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":21,\"ChannelTableId\":7,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":22,\"ChannelTableId\":8,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":23,\"ChannelTableId\":8,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":24,\"ChannelTableId\":9,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":25,\"ChannelTableId\":9,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":26,\"ChannelTableId\":10,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":27,\"ChannelTableId\":10,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":28,\"ChannelTableId\":9,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":29,\"ChannelTableId\":11,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":30,\"ChannelTableId\":11,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":31,\"ChannelTableId\":10,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":32,\"ChannelTableId\":12,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":33,\"ChannelTableId\":12,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":34,\"ChannelTableId\":11,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":35,\"ChannelTableId\":12,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":36,\"ChannelTableId\":13,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":37,\"ChannelTableId\":13,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":38,\"ChannelTableId\":14,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":39,\"ChannelTableId\":14,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":40,\"ChannelTableId\":13,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":41,\"ChannelTableId\":15,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":42,\"ChannelTableId\":15,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":43,\"ChannelTableId\":14,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":44,\"ChannelTableId\":15,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":45,\"ChannelTableId\":16,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":46,\"ChannelTableId\":16,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":47,\"ChannelTableId\":17,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":48,\"ChannelTableId\":17,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":49,\"ChannelTableId\":16,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":50,\"ChannelTableId\":18,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":51,\"ChannelTableId\":18,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":52,\"ChannelTableId\":17,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":53,\"ChannelTableId\":18,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":54,\"ChannelTableId\":19,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":55,\"ChannelTableId\":19,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":56,\"ChannelTableId\":20,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":57,\"ChannelTableId\":20,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":58,\"ChannelTableId\":19,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":59,\"ChannelTableId\":21,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":60,\"ChannelTableId\":21,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":61,\"ChannelTableId\":20,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":62,\"ChannelTableId\":22,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":63,\"ChannelTableId\":22,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":64,\"ChannelTableId\":21,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":65,\"ChannelTableId\":23,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":66,\"ChannelTableId\":23,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":67,\"ChannelTableId\":22,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":68,\"ChannelTableId\":24,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":69,\"ChannelTableId\":24,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":70,\"ChannelTableId\":23,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":71,\"ChannelTableId\":24,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":72,\"ChannelTableId\":25,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":73,\"ChannelTableId\":25,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":74,\"ChannelTableId\":26,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":75,\"ChannelTableId\":26,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":76,\"ChannelTableId\":25,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":77,\"ChannelTableId\":27,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":78,\"ChannelTableId\":27,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":79,\"ChannelTableId\":26,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":80,\"ChannelTableId\":28,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":81,\"ChannelTableId\":28,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":82,\"ChannelTableId\":27,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":83,\"ChannelTableId\":29,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":84,\"ChannelTableId\":29,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":85,\"ChannelTableId\":28,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":86,\"ChannelTableId\":30,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":87,\"ChannelTableId\":30,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":88,\"ChannelTableId\":29,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":89,\"ChannelTableId\":30,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":90,\"ChannelTableId\":31,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":91,\"ChannelTableId\":31,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":92,\"ChannelTableId\":31,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":93,\"ChannelTableId\":32,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":94,\"ChannelTableId\":32,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":95,\"ChannelTableId\":32,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":96,\"ChannelTableId\":33,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":97,\"ChannelTableId\":33,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":98,\"ChannelTableId\":33,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":99,\"ChannelTableId\":34,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":100,\"ChannelTableId\":34,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":101,\"ChannelTableId\":34,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":102,\"ChannelTableId\":35,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":103,\"ChannelTableId\":35,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":104,\"ChannelTableId\":35,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":105,\"ChannelTableId\":36,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":106,\"ChannelTableId\":36,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":107,\"ChannelTableId\":36,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":108,\"ChannelTableId\":37,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":109,\"ChannelTableId\":37,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":110,\"ChannelTableId\":37,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":111,\"ChannelTableId\":38,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":112,\"ChannelTableId\":38,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":113,\"ChannelTableId\":38,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":114,\"ChannelTableId\":39,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":115,\"ChannelTableId\":39,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":116,\"ChannelTableId\":39,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":117,\"ChannelTableId\":40,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":118,\"ChannelTableId\":40,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":119,\"ChannelTableId\":40,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":120,\"ChannelTableId\":41,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":121,\"ChannelTableId\":41,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":122,\"ChannelTableId\":41,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":123,\"ChannelTableId\":42,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":124,\"ChannelTableId\":42,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":125,\"ChannelTableId\":42,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":126,\"ChannelTableId\":43,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":127,\"ChannelTableId\":43,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":128,\"ChannelTableId\":43,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":129,\"ChannelTableId\":44,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":130,\"ChannelTableId\":44,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":131,\"ChannelTableId\":44,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":132,\"ChannelTableId\":45,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":133,\"ChannelTableId\":45,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":134,\"ChannelTableId\":45,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":135,\"ChannelTableId\":46,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":136,\"ChannelTableId\":46,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":137,\"ChannelTableId\":46,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":138,\"ChannelTableId\":47,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":139,\"ChannelTableId\":47,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":140,\"ChannelTableId\":47,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":141,\"ChannelTableId\":48,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":142,\"ChannelTableId\":48,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":143,\"ChannelTableId\":48,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":144,\"ChannelTableId\":49,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":145,\"ChannelTableId\":49,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":146,\"ChannelTableId\":49,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":147,\"ChannelTableId\":50,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":148,\"ChannelTableId\":50,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":149,\"ChannelTableId\":50,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":150,\"ChannelTableId\":51,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":151,\"ChannelTableId\":51,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":152,\"ChannelTableId\":51,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":153,\"ChannelTableId\":52,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":154,\"ChannelTableId\":52,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":155,\"ChannelTableId\":52,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":156,\"ChannelTableId\":53,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":157,\"ChannelTableId\":53,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":158,\"ChannelTableId\":53,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":159,\"ChannelTableId\":54,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":160,\"ChannelTableId\":54,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":161,\"ChannelTableId\":54,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":162,\"ChannelTableId\":55,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":163,\"ChannelTableId\":55,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":164,\"ChannelTableId\":55,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":165,\"ChannelTableId\":56,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":166,\"ChannelTableId\":56,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":167,\"ChannelTableId\":56,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":168,\"ChannelTableId\":57,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":169,\"ChannelTableId\":57,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":170,\"ChannelTableId\":57,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":171,\"ChannelTableId\":58,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":172,\"ChannelTableId\":58,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":173,\"ChannelTableId\":58,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":174,\"ChannelTableId\":59,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":175,\"ChannelTableId\":59,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":176,\"ChannelTableId\":59,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":177,\"ChannelTableId\":60,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":178,\"ChannelTableId\":60,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":179,\"ChannelTableId\":60,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":180,\"ChannelTableId\":61,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":181,\"ChannelTableId\":61,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":182,\"ChannelTableId\":61,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":183,\"ChannelTableId\":62,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":184,\"ChannelTableId\":62,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":185,\"ChannelTableId\":62,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":186,\"ChannelTableId\":63,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":187,\"ChannelTableId\":63,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":188,\"ChannelTableId\":63,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":189,\"ChannelTableId\":64,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":190,\"ChannelTableId\":64,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":191,\"ChannelTableId\":64,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":192,\"ChannelTableId\":65,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":193,\"ChannelTableId\":65,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":194,\"ChannelTableId\":65,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":195,\"ChannelTableId\":66,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":196,\"ChannelTableId\":66,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":197,\"ChannelTableId\":66,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":198,\"ChannelTableId\":67,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":199,\"ChannelTableId\":67,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":200,\"ChannelTableId\":67,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":201,\"ChannelTableId\":68,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":202,\"ChannelTableId\":68,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":203,\"ChannelTableId\":68,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":204,\"ChannelTableId\":69,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":205,\"ChannelTableId\":69,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":206,\"ChannelTableId\":69,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":207,\"ChannelTableId\":70,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":208,\"ChannelTableId\":70,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":209,\"ChannelTableId\":70,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":210,\"ChannelTableId\":71,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":211,\"ChannelTableId\":71,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":212,\"ChannelTableId\":71,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":213,\"ChannelTableId\":72,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":214,\"ChannelTableId\":72,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":215,\"ChannelTableId\":72,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":216,\"ChannelTableId\":73,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":217,\"ChannelTableId\":73,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":218,\"ChannelTableId\":73,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":219,\"ChannelTableId\":74,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":220,\"ChannelTableId\":74,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":221,\"ChannelTableId\":74,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":222,\"ChannelTableId\":75,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":223,\"ChannelTableId\":75,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":224,\"ChannelTableId\":75,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":225,\"ChannelTableId\":76,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":226,\"ChannelTableId\":76,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":227,\"ChannelTableId\":76,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":228,\"ChannelTableId\":77,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":229,\"ChannelTableId\":77,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":230,\"ChannelTableId\":77,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":231,\"ChannelTableId\":78,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":232,\"ChannelTableId\":78,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":233,\"ChannelTableId\":78,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":234,\"ChannelTableId\":79,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":235,\"ChannelTableId\":79,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":236,\"ChannelTableId\":79,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":237,\"ChannelTableId\":80,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":238,\"ChannelTableId\":80,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":239,\"ChannelTableId\":80,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":240,\"ChannelTableId\":81,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":241,\"ChannelTableId\":81,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":242,\"ChannelTableId\":81,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":243,\"ChannelTableId\":82,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":244,\"ChannelTableId\":82,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":245,\"ChannelTableId\":82,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":246,\"ChannelTableId\":83,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":247,\"ChannelTableId\":83,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":248,\"ChannelTableId\":83,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":249,\"ChannelTableId\":84,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":250,\"ChannelTableId\":84,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":251,\"ChannelTableId\":84,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":252,\"ChannelTableId\":85,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":253,\"ChannelTableId\":85,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":254,\"ChannelTableId\":85,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":255,\"ChannelTableId\":86,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":256,\"ChannelTableId\":86,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":257,\"ChannelTableId\":86,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":258,\"ChannelTableId\":87,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":259,\"ChannelTableId\":87,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":260,\"ChannelTableId\":87,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":261,\"ChannelTableId\":88,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":262,\"ChannelTableId\":88,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":263,\"ChannelTableId\":88,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":264,\"ChannelTableId\":89,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":265,\"ChannelTableId\":89,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":266,\"ChannelTableId\":89,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":267,\"ChannelTableId\":90,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":268,\"ChannelTableId\":90,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":269,\"ChannelTableId\":90,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":270,\"ChannelTableId\":91,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":271,\"ChannelTableId\":91,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":272,\"ChannelTableId\":91,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":273,\"ChannelTableId\":92,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":274,\"ChannelTableId\":92,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":275,\"ChannelTableId\":92,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":276,\"ChannelTableId\":93,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":277,\"ChannelTableId\":93,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":278,\"ChannelTableId\":93,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":279,\"ChannelTableId\":94,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":280,\"ChannelTableId\":94,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":281,\"ChannelTableId\":94,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":282,\"ChannelTableId\":95,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":283,\"ChannelTableId\":95,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":284,\"ChannelTableId\":95,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":285,\"ChannelTableId\":96,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":286,\"ChannelTableId\":96,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":287,\"ChannelTableId\":96,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":288,\"ChannelTableId\":97,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":289,\"ChannelTableId\":97,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":290,\"ChannelTableId\":97,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":291,\"ChannelTableId\":98,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":292,\"ChannelTableId\":98,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":293,\"ChannelTableId\":98,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":294,\"ChannelTableId\":99,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":295,\"ChannelTableId\":99,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":296,\"ChannelTableId\":99,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":297,\"ChannelTableId\":100,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":298,\"ChannelTableId\":100,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":299,\"ChannelTableId\":100,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":300,\"ChannelTableId\":101,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":301,\"ChannelTableId\":101,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":302,\"ChannelTableId\":101,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":303,\"ChannelTableId\":102,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":304,\"ChannelTableId\":102,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":305,\"ChannelTableId\":102,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":306,\"ChannelTableId\":103,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":307,\"ChannelTableId\":103,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":308,\"ChannelTableId\":103,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":309,\"ChannelTableId\":104,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":310,\"ChannelTableId\":104,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":311,\"ChannelTableId\":104,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":312,\"ChannelTableId\":105,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":313,\"ChannelTableId\":105,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":314,\"ChannelTableId\":105,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":315,\"ChannelTableId\":106,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":316,\"ChannelTableId\":106,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":317,\"ChannelTableId\":106,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":318,\"ChannelTableId\":107,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":319,\"ChannelTableId\":107,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":320,\"ChannelTableId\":107,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":321,\"ChannelTableId\":108,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":322,\"ChannelTableId\":108,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":323,\"ChannelTableId\":108,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":324,\"ChannelTableId\":109,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":325,\"ChannelTableId\":109,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":326,\"ChannelTableId\":109,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":327,\"ChannelTableId\":110,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":328,\"ChannelTableId\":110,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":329,\"ChannelTableId\":110,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":330,\"ChannelTableId\":111,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":331,\"ChannelTableId\":111,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":332,\"ChannelTableId\":111,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":333,\"ChannelTableId\":112,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":334,\"ChannelTableId\":112,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":335,\"ChannelTableId\":112,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":336,\"ChannelTableId\":113,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":337,\"ChannelTableId\":113,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":338,\"ChannelTableId\":113,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":339,\"ChannelTableId\":114,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":340,\"ChannelTableId\":114,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":341,\"ChannelTableId\":114,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":342,\"ChannelTableId\":115,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":343,\"ChannelTableId\":115,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":344,\"ChannelTableId\":115,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":345,\"ChannelTableId\":116,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":346,\"ChannelTableId\":116,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":347,\"ChannelTableId\":116,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":348,\"ChannelTableId\":117,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":349,\"ChannelTableId\":117,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":350,\"ChannelTableId\":117,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":351,\"ChannelTableId\":118,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":352,\"ChannelTableId\":118,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":353,\"ChannelTableId\":118,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":354,\"ChannelTableId\":119,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":355,\"ChannelTableId\":119,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":356,\"ChannelTableId\":119,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":357,\"ChannelTableId\":120,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":358,\"ChannelTableId\":120,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":359,\"ChannelTableId\":120,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":360,\"ChannelTableId\":121,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":361,\"ChannelTableId\":121,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":362,\"ChannelTableId\":121,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":363,\"ChannelTableId\":122,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":364,\"ChannelTableId\":122,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":365,\"ChannelTableId\":122,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":366,\"ChannelTableId\":123,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":367,\"ChannelTableId\":123,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":368,\"ChannelTableId\":123,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":369,\"ChannelTableId\":124,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":370,\"ChannelTableId\":124,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":371,\"ChannelTableId\":124,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":372,\"ChannelTableId\":125,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":373,\"ChannelTableId\":125,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":374,\"ChannelTableId\":125,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":375,\"ChannelTableId\":126,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":376,\"ChannelTableId\":126,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":377,\"ChannelTableId\":126,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":378,\"ChannelTableId\":127,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":379,\"ChannelTableId\":127,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":380,\"ChannelTableId\":127,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":381,\"ChannelTableId\":128,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":382,\"ChannelTableId\":128,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":383,\"ChannelTableId\":128,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":384,\"ChannelTableId\":129,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":385,\"ChannelTableId\":129,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":386,\"ChannelTableId\":129,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":387,\"ChannelTableId\":130,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":388,\"ChannelTableId\":130,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":389,\"ChannelTableId\":130,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":390,\"ChannelTableId\":131,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":391,\"ChannelTableId\":131,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":392,\"ChannelTableId\":131,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":393,\"ChannelTableId\":132,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":394,\"ChannelTableId\":132,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":395,\"ChannelTableId\":132,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":396,\"ChannelTableId\":133,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":397,\"ChannelTableId\":133,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":398,\"ChannelTableId\":133,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":399,\"ChannelTableId\":134,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":400,\"ChannelTableId\":134,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":401,\"ChannelTableId\":134,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":402,\"ChannelTableId\":135,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":403,\"ChannelTableId\":135,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":404,\"ChannelTableId\":135,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":405,\"ChannelTableId\":136,\"StatusType\":1,\"StatusValue\":1,\"IsSynced\":0},{\"Id\":406,\"ChannelTableId\":136,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":407,\"ChannelTableId\":136,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":408,\"ChannelTableId\":137,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":409,\"ChannelTableId\":137,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":410,\"ChannelTableId\":137,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":411,\"ChannelTableId\":138,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":412,\"ChannelTableId\":138,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":413,\"ChannelTableId\":138,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":414,\"ChannelTableId\":139,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":415,\"ChannelTableId\":139,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":416,\"ChannelTableId\":139,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":417,\"ChannelTableId\":140,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":418,\"ChannelTableId\":140,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":419,\"ChannelTableId\":140,\"StatusType\":2,\"StatusValue\":100,\"IsSynced\":0},{\"Id\":420,\"ChannelTableId\":141,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":421,\"ChannelTableId\":141,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":422,\"ChannelTableId\":141,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":423,\"ChannelTableId\":142,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":424,\"ChannelTableId\":142,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":425,\"ChannelTableId\":142,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":426,\"ChannelTableId\":143,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":427,\"ChannelTableId\":143,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":428,\"ChannelTableId\":143,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":429,\"ChannelTableId\":144,\"StatusType\":1,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":430,\"ChannelTableId\":144,\"StatusType\":3,\"StatusValue\":0,\"IsSynced\":0},{\"Id\":431,\"ChannelTableId\":144,\"StatusType\":2,\"StatusValue\":0,\"IsSynced\":0}],\"RgbwStatus\":[{\"Id\":1,\"DeviceTableId\":24,\"StatusType\":98,\"IsPowerOn\":1,\"ColorR\":238,\"ColorG\":255,\"ColorB\":2,\"IsWhiteEnabled\":0,\"DimmingValue\":100,\"IsSynced\":0}],\"Room\":[{\"Id\":2,\"Home\":1,\"Name\":\"MyRoom\",\"RoomNumber\":0,\"IsActive\":1,\"IsSynced\":0},{\"Id\":3,\"Home\":1,\"Name\":\"3\",\"RoomNumber\":0,\"IsActive\":1,\"IsSynced\":0},{\"Id\":4,\"Home\":1,\"Name\":\"2\",\"RoomNumber\":0,\"IsActive\":1,\"IsSynced\":0},{\"Id\":5,\"Home\":1,\"Name\":\"4\",\"RoomNumber\":0,\"IsActive\":1,\"IsSynced\":0}],\"Home\":[{\"Id\":1,\"Name\":\"MyHome\",\"Address1\":null,\"Address2\":null,\"Block\":null,\"City\":null,\"ZipCode\":0,\"Country\":null,\"TimeZone\":6,\"Phone\":null,\"PassPhrase\":\"4905a51c1b987b8f_2091CP\",\"MeshMode\":1,\"Zone\":null,\"IsInternet\":1,\"IsDefault\":1,\"IsActive\":1,\"IsSynced\":0}],\"UserHomeLink\":[{\"Id\":2,\"Home\":1,\"User\":2,\"IsAdmin\":0,\"IsSynced\":0},{\"Id\":3,\"Home\":1,\"User\":3,\"IsAdmin\":0,\"IsSynced\":0}],\"UserRoomLink\":[{\"Id\":2,\"User\":2,\"Room\":2,\"IsSynced\":0},{\"Id\":3,\"User\":3,\"Room\":2,\"IsSynced\":0},{\"Id\":4,\"User\":2,\"Room\":3,\"IsSynced\":0},{\"Id\":5,\"User\":3,\"Room\":3,\"IsSynced\":0},{\"Id\":6,\"User\":2,\"Room\":4,\"IsSynced\":0},{\"Id\":7,\"User\":3,\"Room\":4,\"IsSynced\":0},{\"Id\":8,\"User\":2,\"Room\":5,\"IsSynced\":0},{\"Id\":9,\"User\":3,\"Room\":5,\"IsSynced\":0}],\"UserInfo\":[{\"Id\":2,\"UserName\":\"Qa Test\",\"Email\":\"qatest@yopmail.com\",\"Password\":\"dJOreK9VX5ZEdu8j8sqDVg==\",\"MobileNumber\":\"+6122\",\"Country\":\"Australia\",\"Sex\":\"Male\",\"LoginStatus\":0,\"RegStatus\":0,\"IsSynced\":0},{\"Id\":3,\"UserName\":\"Qa Test\",\"Email\":\"m@yopmail.com\",\"Password\":\"dJOreK9VX5ZEdu8j8sqDVg==\",\"MobileNumber\":\"+8801512345678\",\"Country\":\"Bangladesh\",\"Sex\":\"Male\",\"LoginStatus\":1,\"RegStatus\":0,\"IsSynced\":0}],\"RouterInfo\":[],\"DatabaseVersion\":2}"));

                msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
                if (string.IsNullOrEmpty(msg))
                {
                    return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
                }

                #endregion

                JsonParser jsonManager = new JsonParser(msg, MessageReceivedFrom.Api);
                jsonManager.Save();

                FillPasswordRecoveryInfos("", " Configuration Successfully Process.", HttpStatusCode.OK, oRootObject);
                response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }
            catch (Exception ex)
            {
                FillPasswordRecoveryInfos(string.Empty, ex.ToString(), HttpStatusCode.BadRequest, oRootObject);
                response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }

            return response;
        }

        [Route("api/PasswordRecovery")]
        [HttpPost]
        public HttpResponseMessage PasswordRecovery(JObject encryptedString)
        {
            #region Initialization

            HttpResponseMessage response;
            string msg = string.Empty;

            msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
            if (string.IsNullOrEmpty(msg))
            {
                return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
            }
            var tempJsonObject = JsonConvert.DeserializeObject<dynamic>(msg);


            #endregion

            using (IDataContextAsync context = new SmartHomeDataContext())
            {
                using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
                {
                    IUserInfoService service = new UserInfoService(unitOfWork);
                    response = ProcessPasswordRecovery(Convert.ToString(tempJsonObject.Email), service);
                }
            }
            return response;
        }

        [Route("api/GetFirmwareUpdate")]
        [HttpPost]
        public HttpResponseMessage FirmwareUpdateInfo()
        {
            string firmwareMessage = "{\"SMSW6G\":{\"version\":\"2\", \"file\":\"Switch.img\"}, \"SMRB12\":{\"version\":\"2\", \"file\":\"RGBW_Dynamic_DName_update.img\"}, \"SMCRTV\":{\"version\":\"2\", \"file\":\"Switch.img\"}, \"SMCRTH\":{\"version\":\"2\", \"file\":\"Switch.img\"}, \"SMRWTR\":{\"version\":\"2\", \"file\":\"Switch.img\"}}";
            //string msg = JsonConvert.SerializeObject(firmwareMessage);
            return new HttpResponseMessage() { Content = new StringContent(firmwareMessage, Encoding.UTF8, "application/json") };
        }


        [Route("api/NewRoom")]
        [HttpPost]
        public HttpResponseMessage NewRoom(JObject encryptedString)
        {
            HttpResponseMessage response;
            PasswordRecoveryRootObjectEntity oRootObject = new PasswordRecoveryRootObjectEntity();
            try
            {
                #region Initialization

                oRootObject.data = new PasswordRecoveryObjectEntity();
                string msg = string.Empty;

                //Debug.WriteLine(SecurityManager.Encrypt("{\"Room\":[{\"Id\":1,\"Home\":1,\"Name\":\"MyRoom\",\"RoomNumber\":0,\"IsActive\":1,\"IsSynced\":0}],\"Home\":[{\"PassPhrase\":\"4905a51c1b987b8f_O234AV\"}],\"UserRoomLink\":[{\"Id\":1,\"User\":1,\"Room\":1,\"IsSynced\":0}],\"UserInfo\":[{\"Email\":\"s@yopmail.com\"}]}"));

                msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
                if (string.IsNullOrEmpty(msg))
                {
                    return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
                }

                #endregion

                JsonParser jsonManager = new JsonParser(msg, MessageReceivedFrom.Api);
                jsonManager.SaveNewRoom();

                FillPasswordRecoveryInfos("", " New Room Add Successfully.", HttpStatusCode.OK, oRootObject);
                response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }
            catch (Exception ex)
            {
                FillPasswordRecoveryInfos(string.Empty, ex.ToString(), HttpStatusCode.BadRequest, oRootObject);
                response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }

            return response;
        }



        [Route("api/NewUser")]
        [HttpPost]
        public HttpResponseMessage NewUser(JObject encryptedString)
        {
            HttpResponseMessage response;
            PasswordRecoveryRootObjectEntity oRootObject = new PasswordRecoveryRootObjectEntity();
            try
            {
                #region Initialization

                oRootObject.data = new PasswordRecoveryObjectEntity();
                string msg = string.Empty;

                //Debug.WriteLine(SecurityManager.Encrypt("{\"Room\":[{\"Id\":1,\"Home\":1,\"Name\":\"MyRoom\",\"RoomNumber\":0,\"IsActive\":1,\"IsSynced\":0}],\"Home\":[{\"PassPhrase\":\"4905a51c1b987b8f_O234AV\"}],\"UserRoomLink\":[{\"Id\":1,\"User\":1,\"Room\":1,\"IsSynced\":0}],\"UserInfo\":[{\"Email\":\"s@yopmail.com\"}]}"));

                msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
                if (string.IsNullOrEmpty(msg))
                {
                    return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
                }

                #endregion

                JsonParser jsonManager = new JsonParser(msg, MessageReceivedFrom.Api);
                jsonManager.SaveNewUser();

                FillPasswordRecoveryInfos("", " New User Add Successfully.", HttpStatusCode.OK, oRootObject);
                response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }
            catch (Exception ex)
            {
                FillPasswordRecoveryInfos(string.Empty, ex.ToString(), HttpStatusCode.BadRequest, oRootObject);
                response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }

            return response;
        }


        [Route("api/NewDevice")]
        [HttpPost]
        public HttpResponseMessage NewDevice(JObject encryptedString)
        {
            HttpResponseMessage response;
            PasswordRecoveryRootObjectEntity oRootObject = new PasswordRecoveryRootObjectEntity();
            try
            {
                #region Initialization

                oRootObject.data = new PasswordRecoveryObjectEntity();
                string msg = string.Empty;

                msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
                if (string.IsNullOrEmpty(msg))
                {
                    return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
                }

                #endregion

                JsonParser jsonManager = new JsonParser(msg, MessageReceivedFrom.Api);
                bool isSuccess = jsonManager.SaveNewDevice();

                if (isSuccess)
                {
                    FillPasswordRecoveryInfos("", " New Device Add Successfully.", HttpStatusCode.OK, oRootObject);
                }
                else
                {
                    FillPasswordRecoveryInfos("", " Can Not Add New Device.", HttpStatusCode.BadRequest, oRootObject);
                }

                response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }
            catch (Exception ex)
            {
                FillPasswordRecoveryInfos(string.Empty, ex.ToString(), HttpStatusCode.BadRequest, oRootObject);
                response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }

            return response;
        }

        #region No Action Methods

        [NonAction]
        private HttpResponseMessage ProcessPasswordRecovery(string userEmail, IUserInfoService service)
        {
            HttpResponseMessage response;
            PasswordRecoveryRootObjectEntity oRootObject = new PasswordRecoveryRootObjectEntity();
            try
            {
                oRootObject.data = new PasswordRecoveryObjectEntity();
                var isEmailExists = service.IsLoginIdUnique(userEmail);
                if (isEmailExists)
                {

                    FillPasswordRecoveryInfos(service.PasswordRecoveryByEmail(userEmail), "User password", HttpStatusCode.OK, oRootObject);
                    oRootObject.data.UserName = service.GetsUserInfosByEmail(userEmail, oRootObject.data.Password).FirstOrDefault().UserName;
                    response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
                }
                else
                {
                    FillPasswordRecoveryInfos(string.Empty, "User not exist", HttpStatusCode.BadRequest, oRootObject);
                    response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
                }
            }
            catch (Exception ex)
            {
                FillPasswordRecoveryInfos(string.Empty, ex.ToString(), HttpStatusCode.BadRequest, oRootObject);
                response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }

            return response;
        }

        private void FillPasswordRecoveryInfos(string userPassword, string message, HttpStatusCode code, PasswordRecoveryRootObjectEntity oRootObject)
        {
            oRootObject.data.Password = userPassword;
            oRootObject.MESSAGE = new LoginMessage();
            oRootObject.MESSAGE = SetResponseMessage(message, code);
        }

        [NonAction]
        private LoginMessage SetResponseMessage(string message, HttpStatusCode code)
        {
            LoginMessage oLoginMessage = new LoginMessage();
            oLoginMessage.HTTP_MESSAGE = message;
            oLoginMessage.HTTP_STATUS = (int)code;
            return oLoginMessage;
        }

        [NonAction]
        private HttpResponseMessage PrepareJsonResponse<T>(T oRootObject)
        {
            string msg = JsonConvert.SerializeObject(oRootObject);
            return new HttpResponseMessage() { Content = new StringContent(msg, Encoding.UTF8, "application/json") };
        }

        [NonAction]
        private HttpResponseMessage ProcessIsUserExist(string userEmail, IUserInfoService service)
        {
            HttpResponseMessage response;
            PasswordRecoveryRootObjectEntity oRootObject = new PasswordRecoveryRootObjectEntity();
            try
            {
                oRootObject.data = new PasswordRecoveryObjectEntity();
                var isEmailExists = service.IsLoginIdUnique(userEmail);
                if (isEmailExists)
                {
                    FillPasswordRecoveryInfos("", "User already exist", HttpStatusCode.Conflict, oRootObject);
                    response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
                }
                else
                {
                    FillPasswordRecoveryInfos("", "User not exist", HttpStatusCode.OK, oRootObject);
                    response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
                }
            }
            catch (Exception ex)
            {
                FillPasswordRecoveryInfos(string.Empty, ex.ToString(), HttpStatusCode.BadRequest, oRootObject);
                response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }

            return response;
        }

        [NonAction]
        private HttpResponseMessage ProcessPasswordUpdate(string userEmail, string userPassword, IUserInfoService service)
        {
            HttpResponseMessage response;
            PasswordRecoveryRootObjectEntity oRootObject = new PasswordRecoveryRootObjectEntity();
            try
            {
                oRootObject.data = new PasswordRecoveryObjectEntity();
                var isEmailExists = service.IsLoginIdUnique(userEmail);
                if (isEmailExists)
                {
                    service.PasswordUpdate(userEmail, userPassword);
                    FillPasswordRecoveryInfos("", "Successfully Password Update", HttpStatusCode.OK, oRootObject);
                    response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
                }
                else
                {
                    FillPasswordRecoveryInfos("", "Password not update", HttpStatusCode.BadRequest, oRootObject);
                    response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
                }
            }
            catch (Exception ex)
            {
                FillPasswordRecoveryInfos(string.Empty, ex.ToString(), HttpStatusCode.BadRequest, oRootObject);
                response = PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }

            return response;
        }

        [NonAction]
        private HttpResponseMessage ProcessGetRegisteredUser(UserInfoEntity oUserInfo, IUnitOfWorkAsync unitOfWork, IUserInfoService service)
        {
            HttpResponseMessage response;
            LoginRootObjectEntity oRootObject = new LoginRootObjectEntity();
            try
            {
                var isEmailExists = service.IsLoginIdUnique(oUserInfo.Email);
                if (isEmailExists)
                {
                    oRootObject.data = new LoginObjectEntity();
                    response = PrepareJsonResponse(oRootObject, "User already exist", HttpStatusCode.Conflict);
                }
                else
                {
                    unitOfWork.BeginTransaction();
                    try
                    {
                        service.Add(oUserInfo);
                        var changes = unitOfWork.SaveChanges();
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        oRootObject.data = new LoginObjectEntity();
                        response = PrepareJsonResponse(oRootObject, ex.ToString(), HttpStatusCode.BadRequest);
                    }
                    oRootObject.data = new LoginObjectEntity();
                    response = PrepareJsonResponse(oRootObject, "Unique user", HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                oRootObject.data = new LoginObjectEntity();
                response = PrepareJsonResponse(oRootObject, ex.ToString(), HttpStatusCode.BadRequest);
            }

            return response;
        }

        [NonAction]
        private HttpResponseMessage ProcessUserInfomations(LoginRootObjectEntity oRootObject, HttpResponseMessage response, LoginObjectEntity oLoginObject, string msg, IUserInfoService service, IConfigurationParserManagerService serviceConfigure)
        {
            oLoginObject = JsonConvert.DeserializeObject<LoginObjectEntity>(msg);
            var oUserInfo = oLoginObject.UserInfo.First();
            var user = service.GetsUserInfosByEmail(oUserInfo.Email, oUserInfo.Password).FirstOrDefault();
            if (user != null)
            {
                try
                {
                    response = GetUserInformationsFormDatabase(oRootObject, oLoginObject, serviceConfigure, user);
                }
                catch (Exception ex)
                {
                    oRootObject.data = new LoginObjectEntity();
                    response = PrepareJsonResponse(oRootObject, ex.ToString(), HttpStatusCode.BadRequest);
                }
            }
            else
            {
                oRootObject.data = new LoginObjectEntity();
                response = PrepareJsonResponse(oRootObject, "User not found", HttpStatusCode.NotFound);
            }
            return response;
        }

        [NonAction]
        private HttpResponseMessage GetUserInformationsFormDatabase(LoginRootObjectEntity oRootObject, LoginObjectEntity oLoginObject, IConfigurationParserManagerService serviceConfigure, UserInfo user)
        {
            HttpResponseMessage response;
            ObjectInitialization(oLoginObject);
            var homeViewModel = serviceConfigure.GetsHomesAllInfo(user.UserInfoId);
            FillLoginObjectByData(oLoginObject, homeViewModel);
            oRootObject.data = new LoginObjectEntity();
            oRootObject.data = oLoginObject;
            response = PrepareJsonResponseForGetUserInfos(oRootObject, "Success", HttpStatusCode.OK);
            return response;
        }
        [NonAction]
        private HttpResponseMessage PrepareJsonResponseForGetUserInfos(LoginRootObjectEntity oRootObject, string message, HttpStatusCode code)
        {
            oRootObject.MESSAGE = new LoginMessage();
            oRootObject.MESSAGE = SetLoginMessage(message, code);
            string msg = JsonConvert.SerializeObject(oRootObject);
            msg = msg.Replace("false", "0");
            msg = msg.Replace("true", "1");
            return new HttpResponseMessage() { Content = new StringContent(msg, Encoding.UTF8, "application/json") };
        }


        [NonAction]
        private HttpResponseMessage PrepareJsonResponse(LoginRootObjectEntity oRootObject, string message, HttpStatusCode code)
        {
            oRootObject.MESSAGE = new LoginMessage();
            oRootObject.MESSAGE = SetLoginMessage(message, code);
            string msg = JsonConvert.SerializeObject(oRootObject);
            return new HttpResponseMessage() { Content = new StringContent(msg, Encoding.UTF8, "application/json") };
        }
        private void FillLoginObjectByData(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            FillUserInfoToLoginObject(oLoginObject, homeViewModel);
            FillUserHomeLinkInfoToLoginObject(oLoginObject, homeViewModel);
            FillHomeInfoToLoginObject(oLoginObject, homeViewModel);
            //user room link
            FillUserRoomLinkInfoToLoginObject(oLoginObject, homeViewModel);
            //smart router
            FillSmartRouterInfoToLoginObject(oLoginObject, homeViewModel);
            //room
            FillRoomInfoToLoginObject(oLoginObject, homeViewModel);
            //smart device
            FillSmartDeviceInfoToLoginObject(oLoginObject, homeViewModel);
            //d-status
            FillSmartDeviceStatusInfoToLoginObject(oLoginObject, homeViewModel);
            //channel
            FillChannelInfoToLoginObject(oLoginObject, homeViewModel);
            //c-status
            FillChannelStatusInfoToLoginObject(oLoginObject, homeViewModel);
            //Rgb-status
            FillRgbwStatusInfoToLoginObject(oLoginObject, homeViewModel);
            FillNextAssociatedDeviceInfoToLoginObject(oLoginObject, homeViewModel);
        }

        [NonAction]
        private void FillNextAssociatedDeviceInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<NextAssociatedDevice, NextAssociatedDeviceEntity>();
            IEnumerable<NextAssociatedDeviceEntity> oNADeviceEntity = Mapper.Map<IEnumerable<NextAssociatedDevice>, IEnumerable<NextAssociatedDeviceEntity>>(homeViewModel.NextAssociatedDevice);
            oLoginObject.NextAssociatedDeviceId.AddRange(oNADeviceEntity);
        }

        [NonAction]
        private void FillRgbwStatusInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<RgbwStatus, RgbwStatusEntity>()
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0))
                .ForMember(dest => dest.IsPowerOn, opt => opt.MapFrom(src => src.IsPowerOn == true ? 1 : 0))
                .ForMember(dest => dest.IsWhiteEnabled, opt => opt.MapFrom(src => src.IsWhiteEnabled == true ? 1 : 0));

            IEnumerable<RgbwStatusEntity> oDeviceStatusEntity = Mapper.Map<IEnumerable<RgbwStatus>, IEnumerable<RgbwStatusEntity>>(homeViewModel.RgbwStatuses);
            oLoginObject.RgbwStatus.AddRange(oDeviceStatusEntity);
        }
        [NonAction]
        private void FillChannelStatusInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            List<ChannelStatus> channelStatuses = new List<ChannelStatus>();
            foreach (Channel ch in homeViewModel.Channels)
            {
                channelStatuses.AddRange(ch.ChannelStatuses);
            }
            Mapper.CreateMap<ChannelStatus, ChannelStatusEntity>()
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));
            IEnumerable<ChannelStatusEntity> oChannelStatusEntity = Mapper.Map<IEnumerable<ChannelStatus>, IEnumerable<ChannelStatusEntity>>(channelStatuses);
            oLoginObject.ChannelStatus.AddRange(oChannelStatusEntity);
        }
        [NonAction]
        private void FillChannelInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<Channel, ChannelEntity>()
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));
            IEnumerable<ChannelEntity> oChannelEntity = Mapper.Map<IEnumerable<Channel>, IEnumerable<ChannelEntity>>(homeViewModel.Channels);
            oLoginObject.Channel.AddRange(oChannelEntity);
        }
        [NonAction]
        private void FillSmartDeviceStatusInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            List<DeviceStatus> deviceStatuses = new List<DeviceStatus>();
            foreach (SmartDevice device in homeViewModel.SmartDevices)
            {
                deviceStatuses.AddRange(device.DeviceStatus);
            }
            Mapper.CreateMap<DeviceStatus, DeviceStatusEntity>()
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));
            IEnumerable<DeviceStatusEntity> oDeviceStatusEntity = Mapper.Map<IEnumerable<DeviceStatus>, IEnumerable<DeviceStatusEntity>>(deviceStatuses);
            oLoginObject.DeviceStatus.AddRange(oDeviceStatusEntity);
        }
        [NonAction]
        private void FillSmartDeviceInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<SmartDevice, SmartDeviceEntity>()
                 .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0))
                 .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted == true ? 1 : 0));
            IEnumerable<SmartDeviceEntity> oDeviceEntity = Mapper.Map<IEnumerable<SmartDevice>, IEnumerable<SmartDeviceEntity>>(homeViewModel.SmartDevices);
            oLoginObject.Device.AddRange(oDeviceEntity);
        }
        [NonAction]
        private void FillRoomInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<Room, RoomEntity>()
                .ForMember(dest => dest.IsMasterRoom, opt => opt.MapFrom(src => src.IsMasterRoom == true ? 1 : 0))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == true ? 1 : 0))
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));
            IEnumerable<RoomEntity> oRoomEntity = Mapper.Map<IEnumerable<Room>, IEnumerable<RoomEntity>>(homeViewModel.Rooms);
            oLoginObject.Room.AddRange(oRoomEntity);
        }
        [NonAction]
        private void FillSmartRouterInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<RouterInfo, RouterInfoEntity>()
                .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));
            IEnumerable<RouterInfoEntity> oSmartRouterEntity = Mapper.Map<IEnumerable<RouterInfo>, IEnumerable<RouterInfoEntity>>(homeViewModel.Routers);
            oLoginObject.RouterInfo.AddRange(oSmartRouterEntity);
        }
        [NonAction]
        private void FillHomeInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<Home, HomeEntity>()
                  .ForMember(dest => dest.MeshMode, opt => opt.MapFrom(src => (int)src.MeshMode))
                  .ForMember(dest => dest.IsInternet, opt => opt.MapFrom(src => src.IsInternet == true ? 1 : 0))
                  .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0))
                  .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == true ? 1 : 0))
                  .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(src => src.IsDefault == true ? 1 : 0));
            IEnumerable<HomeEntity> oHomeEntity = Mapper.Map<IEnumerable<Home>, IEnumerable<HomeEntity>>(homeViewModel.Homes);
            oLoginObject.Home.AddRange(oHomeEntity);
        }
        [NonAction]
        private void FillUserHomeLinkInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<UserHomeLink, UserHomeLinkEntity>()
                            .ForMember(dest => dest.UserHomeLinkEntityId, opt => opt.MapFrom(src => src.AppsHomeId))
                            .ForMember(dest => dest.AppsUserHomeLinkId, opt => opt.MapFrom(src => src.AppsUserHomeLinkId))
                            .ForMember(dest => dest.AppsHomeId, opt => opt.MapFrom(src => src.Home.AppsHomeId))
                            .ForMember(dest => dest.AppsUserId, opt => opt.MapFrom(src => src.UserInfo.AppsUserId))
                            .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.IsAdmin == true ? 1 : 0))
                            .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));

            IEnumerable<UserHomeLinkEntity> linkEntity = Mapper.Map<IEnumerable<UserHomeLink>, IEnumerable<UserHomeLinkEntity>>(homeViewModel.UserHomeLinks);
            oLoginObject.UserHomeLink.AddRange(linkEntity);
        }

        [NonAction]
        private void FillUserRoomLinkInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<UserRoomLink, UserRoomLinkEntity>()
                                        .ForMember(dest => dest.AppsUserId, opt => opt.MapFrom(src => src.UserInfo.AppsUserId))
                                        .ForMember(dest => dest.AppsRoomId, opt => opt.MapFrom(src => src.Room.AppsRoomId))
                                        .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0));
            IEnumerable<UserRoomLinkEntity> oUserRoomLinkEntity = Mapper.Map<IEnumerable<UserRoomLink>, IEnumerable<UserRoomLinkEntity>>(homeViewModel.UserRoomLinks);
            oLoginObject.UserRoomLink.AddRange(oUserRoomLinkEntity);
        }
        [NonAction]
        private void FillUserInfoToLoginObject(LoginObjectEntity oLoginObject, HomeViewModel homeViewModel)
        {
            Mapper.CreateMap<UserInfo, UserInfoEntity>()
                            .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.Sex))
                            .ForMember(dest => dest.LoginStatus, opt => opt.MapFrom(src => src.LoginStatus == true ? 1 : 0))
                            .ForMember(dest => dest.IsSynced, opt => opt.MapFrom(src => src.IsSynced == true ? 1 : 0))
                            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == true ? 1 : 0))
                            .ForMember(dest => dest.RegStatus, opt => opt.MapFrom(src => src.RegStatus == true ? 1 : 0));

            IEnumerable<UserInfoEntity> oUserInfoEntity = Mapper.Map<IEnumerable<UserInfo>, IEnumerable<UserInfoEntity>>(homeViewModel.Users);
            oLoginObject.UserInfo.AddRange(oUserInfoEntity);
        }

        [NonAction]
        private void ObjectInitialization(LoginObjectEntity oLoginObject)
        {
            oLoginObject.UserInfo = new List<UserInfoEntity>();
            oLoginObject.UserHomeLink = new List<UserHomeLinkEntity>();
            oLoginObject.UserRoomLink = new List<UserRoomLinkEntity>();
            oLoginObject.RgbwStatus = new List<RgbwStatusEntity>();
            oLoginObject.Home = new List<Entity.HomeEntity>();
            oLoginObject.RouterInfo = new List<RouterInfoEntity>();
            oLoginObject.Room = new List<RoomEntity>();
            oLoginObject.ChannelStatus = new List<ChannelStatusEntity>();
            oLoginObject.Channel = new List<ChannelEntity>();
            oLoginObject.Device = new List<SmartDeviceEntity>();
            oLoginObject.DeviceStatus = new List<DeviceStatusEntity>();
            oLoginObject.NextAssociatedDeviceId = new List<NextAssociatedDeviceEntity>();
        }

        [NonAction]
        private LoginMessage SetLoginMessage(string message, HttpStatusCode code)
        {
            LoginMessage oLoginMessage = new LoginMessage();
            oLoginMessage.HTTP_MESSAGE = message;
            oLoginMessage.HTTP_STATUS = (int)code;
            return oLoginMessage;
        }
        #endregion
    }
}


