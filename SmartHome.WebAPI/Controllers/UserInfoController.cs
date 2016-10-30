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
using SmartHome.Data.Processor;
using SmartHome.WebAPI.Utility;

namespace SmartHome.WebAPI.Controllers
{
    public class UserInfoController : ApiController
    {
        public UserInfoController()
        {

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

                msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
                if (string.IsNullOrEmpty(msg))
                {
                    return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
                }

                #endregion

                UserJsonParser jsonManager = new UserJsonParser(msg, MessageReceivedFrom.NewUser);
                bool isSuccess = jsonManager.SaveNewUser();

                if (isSuccess)
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " New User Add Successfully.", HttpStatusCode.OK, oRootObject);
                }
                else
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " Can Not Add New User.", HttpStatusCode.BadRequest, oRootObject);
                }

                response = MessageResponseUtility.PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }
            catch (Exception ex)
            {
                MessageResponseUtility.FillPasswordRecoveryInfos(string.Empty, ex.ToString(), HttpStatusCode.BadRequest, oRootObject);
                response = MessageResponseUtility.PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }

            return response;
        }

        [Route("api/DeleteUser")]
        [HttpPost]
        public HttpResponseMessage DeleteUser(JObject encryptedString)
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

                UserDeleteJsonParser jsonManager = new UserDeleteJsonParser(msg, MessageReceivedFrom.DeleteUser);
                UserInfo userInfo = jsonManager.DeleteUser();

                if (userInfo != null)
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " User Delete Successfully.", HttpStatusCode.OK, oRootObject);
                }
                else
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " Can Not Delete User.", HttpStatusCode.BadRequest, oRootObject);
                }

                response = MessageResponseUtility.PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }
            catch (Exception ex)
            {
                MessageResponseUtility.FillPasswordRecoveryInfos(string.Empty, ex.ToString(), HttpStatusCode.BadRequest, oRootObject);
                response = MessageResponseUtility.PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
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

            msg = "{\"Email\":\"qa2600@yopmail.com\",\"Password\":\"46tUX\\/XbJOPCnTLtU283wg==\"}";

            RetrivedUserRelatedDataProcessor userData = new RetrivedUserRelatedDataProcessor(msg, MessageReceivedFrom.GetUserInfo, oLoginObject, oRootObject);
            bool isUserExist = userData.GetUserInfo();


            if (isUserExist)
            {
                response = MessageResponseUtility.PrepareJsonResponseForGetUserInfos(oRootObject, "Success", HttpStatusCode.OK);
            }
            else
            {
                oRootObject.data = new LoginObjectEntity();
                response = MessageResponseUtility.PrepareJsonResponse(oRootObject, "User not found", HttpStatusCode.NotFound);
            }
            #endregion

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

                msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
                if (string.IsNullOrEmpty(msg))
                {
                    return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
                }

               // msg = "{\"UserHomeLink\":[{\"Home\":1,\"User\":1,\"IsAdmin\":0,\"IsSynced\":0,\"Id\":1}],\"Configuration\":\"All\",\"DeviceStatus\":[{\"Id\":1,\"DeviceTableId\":1,\"StatusValue\":0,\"StatusType\":5},{\"Id\":2,\"DeviceTableId\":1,\"StatusValue\":0,\"StatusType\":55},{\"Id\":1,\"DeviceTableId\":2,\"StatusValue\":0,\"StatusType\":55},{\"Id\":2,\"DeviceTableId\":2,\"StatusValue\":1,\"StatusType\":53},{\"Id\":3,\"DeviceTableId\":2,\"StatusValue\":1,\"StatusType\":5},{\"Id\":1,\"DeviceTableId\":3,\"StatusValue\":0,\"StatusType\":55},{\"Id\":2,\"DeviceTableId\":3,\"StatusValue\":1,\"StatusType\":53},{\"Id\":3,\"DeviceTableId\":3,\"StatusValue\":1,\"StatusType\":5}],\"RgbwStatus\":[{\"Id\":1,\"ColorG\":255,\"ColorR\":255,\"DimmingValue\":1,\"StatusType\":98,\"ColorB\":255,\"IsPowerOn\":1,\"IsWhiteEnabled\":0,\"DeviceTableId\":1}],\"Home\":[{\"Block\":\"Block\",\"PassPhrase\":\"01B004\",\"Name\":\"MyHome\",\"Id\":1,\"IsDefault\":1,\"IsActive\":1,\"City\":\"City\",\"Address1\":\"address1\",\"Country\":\"Country\",\"Address2\":\"address2\",\"IsSynced\":1,\"TimeZone\":\"123\",\"Zone\":\"123\",\"IsInternet\":1,\"ZipCode\":\"1215\",\"Phone\":\"Phone\",\"MeshMode\":0}],\"DatabaseVersion\":3,\"VersionDetails\":[{\"HardwareVersion\":0,\"DeviceType\":1,\"VersionId\":1,\"Id\":1},{\"HardwareVersion\":0,\"DeviceType\":2,\"VersionId\":1,\"Id\":2},{\"HardwareVersion\":0,\"DeviceType\":6,\"VersionId\":1,\"Id\":3},{\"HardwareVersion\":1,\"DeviceType\":6,\"VersionId\":1,\"Id\":4},{\"HardwareVersion\":1,\"DeviceType\":1,\"VersionId\":1,\"Id\":5},{\"HardwareVersion\":1,\"DeviceType\":2,\"VersionId\":1,\"Id\":6}],\"Device\":[{\"DeviceType\":2,\"Id\":1,\"DeviceName\":\"SMRB12-01 795275149\",\"DeviceId\":32769,\"Version\":1,\"DeviceHash\":795275149,\"Room\":1,\"IsDeleted\":0,\"Watt\":12},{\"DeviceType\":1,\"Id\":2,\"DeviceName\":\"SMSW6G-01 1553062309\",\"DeviceId\":32770,\"Version\":1,\"DeviceHash\":1553062309,\"Room\":1,\"IsDeleted\":0,\"Watt\":0},{\"DeviceType\":1,\"Id\":3,\"DeviceName\":\"SMSW6G-00 890236793\",\"DeviceId\":32771,\"Version\":0,\"DeviceHash\":890236793,\"Room\":1,\"IsDeleted\":0,\"Watt\":0}],\"NextAssociatedDeviceId\":[{\"NextDeviceId\":32772}],\"Channel\":[{\"DeviceTableId\":2,\"LoadName\":\"Dimmable Light\",\"LoadWatt\":0,\"LoadType\":2,\"ChannelNo\":6,\"Id\":3},{\"DeviceTableId\":2,\"LoadName\":\"Dimmable Light\",\"LoadWatt\":0,\"LoadType\":2,\"ChannelNo\":4,\"Id\":4},{\"DeviceTableId\":2,\"LoadName\":\"Dimmable Light\",\"LoadWatt\":0,\"LoadType\":2,\"ChannelNo\":5,\"Id\":6},{\"DeviceTableId\":3,\"LoadName\":\"Dimmable Light\",\"LoadWatt\":0,\"LoadType\":2,\"ChannelNo\":2,\"Id\":1},{\"DeviceTableId\":3,\"LoadName\":\"Dimmable Light\",\"LoadWatt\":0,\"LoadType\":2,\"ChannelNo\":3,\"Id\":2},{\"DeviceTableId\":3,\"LoadName\":\"Dimmable Light\",\"LoadWatt\":0,\"LoadType\":2,\"ChannelNo\":1,\"Id\":5}],\"Version\":[{\"AuthCode\":\"0123456789ABCDEF\",\"PassPhrase\":\"01B004\",\"AppVersion\":1.8,\"AppName\":\"SmartHome\",\"Id\":1}],\"Room\":[{\"Home\":1,\"Name\":\"MyRoom\",\"RoomNumber\":1,\"IsActive\":1,\"IsSynced\":0,\"Id\":1}],\"ChannelStatus\":[{\"Id\":1,\"ChannelTableId\":3,\"StatusValue\":0,\"StatusType\":2},{\"Id\":2,\"ChannelTableId\":3,\"StatusValue\":0,\"StatusType\":3},{\"Id\":3,\"ChannelTableId\":3,\"StatusValue\":0,\"StatusType\":1},{\"Id\":4,\"ChannelTableId\":4,\"StatusValue\":0,\"StatusType\":3},{\"Id\":5,\"ChannelTableId\":4,\"StatusValue\":0,\"StatusType\":2},{\"Id\":6,\"ChannelTableId\":4,\"StatusValue\":0,\"StatusType\":1},{\"Id\":7,\"ChannelTableId\":6,\"StatusValue\":0,\"StatusType\":1},{\"Id\":8,\"ChannelTableId\":6,\"StatusValue\":0,\"StatusType\":3},{\"Id\":9,\"ChannelTableId\":6,\"StatusValue\":0,\"StatusType\":2},{\"Id\":1,\"ChannelTableId\":1,\"StatusValue\":0,\"StatusType\":1},{\"Id\":2,\"ChannelTableId\":1,\"StatusValue\":0,\"StatusType\":2},{\"Id\":3,\"ChannelTableId\":1,\"StatusValue\":0,\"StatusType\":3},{\"Id\":4,\"ChannelTableId\":2,\"StatusValue\":0,\"StatusType\":2},{\"Id\":5,\"ChannelTableId\":2,\"StatusValue\":0,\"StatusType\":1},{\"Id\":6,\"ChannelTableId\":2,\"StatusValue\":0,\"StatusType\":3},{\"Id\":7,\"ChannelTableId\":5,\"StatusValue\":0,\"StatusType\":1},{\"Id\":8,\"ChannelTableId\":5,\"StatusValue\":0,\"StatusType\":2},{\"Id\":9,\"ChannelTableId\":5,\"StatusValue\":0,\"StatusType\":3}],\"UserInfo\":[{\"Id\":1,\"UserName\":\"Qa Test\",\"IsSynced\":0,\"Sex\":\"Femaale\",\"MobileNumber\":\"\",\"LoginStatus\":1,\"Country\":\"Bangladesh\",\"Email\":\"qa26@yopmail.com\",\"RegStatus\":1,\"Password\":\"46tUX/XbJOPCnTLtU283wg==\"}],\"UserRoomLink\":[{\"Id\":1,\"User\":1,\"IsSynced\":0,\"Room\":1}],\"RouterInfo\":[]}";

                #endregion

                //HomeJsonParser jsonManager = new HomeJsonParser(msg, MessageReceivedFrom.ConfigurationProcessFromApi);
                //jsonManager.Save();


                ConfigurationJsonParser jsonManager = new ConfigurationJsonParser(msg, MessageReceivedFrom.ConfigurationProcessFromApi);
                jsonManager.SaveNewConfiguration();

                MessageResponseUtility.FillPasswordRecoveryInfos("", " Configuration Successfully Process.", HttpStatusCode.OK, oRootObject);
                response = MessageResponseUtility.PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }
            catch (Exception ex)
            {
                MessageResponseUtility.FillPasswordRecoveryInfos(string.Empty, ex.ToString(), HttpStatusCode.BadRequest, oRootObject);
                response = MessageResponseUtility.PrepareJsonResponse<PasswordRecoveryRootObjectEntity>(oRootObject);
            }

            return response;
        }

        [Route("api/GetFirmwareUpdate")]
        [HttpPost]
        public HttpResponseMessage FirmwareUpdateInfo()
        {
            string firmwareMessage = "{\"SMSW6G\":{\"version\":\"2\", \"file\":\"Switch.img\"}, \"SMRB12\":{\"version\":\"2\", \"file\":\"RGBW_Dynamic_DName_update.img\"}, \"SMCRTV\":{\"version\":\"2\", \"file\":\"Switch.img\"}, \"SMCRTH\":{\"version\":\"2\", \"file\":\"Switch.img\"}, \"SMRWTR\":{\"version\":\"2\", \"file\":\"Switch.img\"}}";
            return new HttpResponseMessage() { Content = new StringContent(firmwareMessage, Encoding.UTF8, "application/json") };
        }
    }
}


