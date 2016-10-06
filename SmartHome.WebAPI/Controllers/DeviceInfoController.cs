using System;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using SmartHome.Entity;
using SmartHome.Utility.EncryptionAndDecryption;
using System.Net;
using SmartHome.Data.Processor;
using SmartHome.Model.Enums;
using SmartHome.WebAPI.Utility;
using SmartHome.Json;

namespace SmartHome.WebAPI.Controllers
{
    public class DeviceInfoController : ApiController
    {
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
                //msg = "{\"Home\":[{\"Block\":\"Block\",\"PassPhrase\":\"6DCA8A\",\"Name\":\"MyHome\",\"Id\":1,\"IsDefault\":1,\"IsActive\":1,\"City\":\"City\",\"Address1\":\"address1\",\"Country\":\"Country\",\"Address2\":\"address2\",\"IsSynced\":1,\"TimeZone\":\"123\",\"Zone\":\"123\",\"IsInternet\":1,\"ZipCode\":\"1215\",\"Phone\":\"Phone\",\"MeshMode\":0}],\"DeviceStatus\":[{\"Id\":1,\"DeviceTableId\":1,\"StatusValue\":1,\"StatusType\":53},{\"Id\":2,\"DeviceTableId\":1,\"StatusValue\":0,\"StatusType\":55},{\"Id\":3,\"DeviceTableId\":1,\"StatusValue\":1,\"StatusType\":5}],\"Configuration\":\"NewDevice\",\"Device\":[{\"DeviceType\":1,\"Id\":1,\"DeviceName\":\"SMSW6G-01 704855792\",\"DeviceId\":32769,\"Version\":1,\"DeviceHash\":704855792,\"Room\":1,\"IsDeleted\":0,\"Watt\":0}]}";
                if (string.IsNullOrEmpty(msg))
                {
                    return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
                }

                #endregion

                DeviceJsonParser jsonManager = new DeviceJsonParser(msg, MessageReceivedFrom.NewDevice);
                bool isSuccess = jsonManager.SaveNewDevice();

                if (isSuccess)
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " New Device Add Successfully.", HttpStatusCode.OK, oRootObject);
                }
                else
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " Can Not Add New Device.", HttpStatusCode.BadRequest, oRootObject);
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
        [Route("api/DeleteDevice")]
        [HttpPost]
        public HttpResponseMessage DeleteDevice(JObject encryptedString)
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

                DeviceDeleteJsonParser jsonManager = new DeviceDeleteJsonParser(msg, MessageReceivedFrom.DeleteDevice);
                bool isSuccess = jsonManager.DeleteDevice();

                if (isSuccess)
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " Device Delete Successfully.", HttpStatusCode.OK, oRootObject);
                }
                else
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " Can Not Delete Device.", HttpStatusCode.BadRequest, oRootObject);
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

        [Route("api/NewRouter")]
        [HttpPost]
        public HttpResponseMessage NewRouter(JObject encryptedString)
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

                RouterJsonParser jsonManager = new RouterJsonParser(msg, MessageReceivedFrom.NewRouter);
                bool isSuccess = jsonManager.SaveNewRouter();

                if (isSuccess)
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " New Router Add Successfully.", HttpStatusCode.OK, oRootObject);
                }
                else
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " Can Not Add New Router.", HttpStatusCode.BadRequest, oRootObject);
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
    }
}
