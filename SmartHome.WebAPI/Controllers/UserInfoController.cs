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


