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
    public class ChannelInfoController : ApiController
    {
        [Route("api/NewChannel")]
        [HttpPost]
        public HttpResponseMessage NewChannel(JObject encryptedString)
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
                //msg = "{\"Device\":[{\"Version\":0,\"Id\":1,\"Watt\":0,\"IsDeleted\":0,\"DeviceType\":1,\"DeviceName\":\"SMSW6G-00 890236793\",\"DeviceId\":32803,\"DeviceHash\":890236793,\"Room\":3}],\"Configuration\":\"NewChannel\",\"ChannelStatus\":[{\"StatusType\":2,\"ChannelTableId\":3,\"StatusValue\":0,\"Id\":1},{\"StatusType\":1,\"ChannelTableId\":3,\"StatusValue\":0,\"Id\":2},{\"StatusType\":3,\"ChannelTableId\":3,\"StatusValue\":0,\"Id\":3}],\"Channel\":[{\"LoadType\":2,\"LoadName\":\"Dimmable Light\",\"LoadWatt\":0,\"ChannelNo\":3,\"DeviceTableId\":1,\"Id\":3}],\"Home\":[{\"Block\":\"Block\",\"Country\":\"Country\",\"Address1\":\"address1\",\"Address2\":\"address2\",\"Id\":1,\"Name\":\"MyHome\",\"City\":\"City\",\"IsSynced\":1,\"Zone\":\"123\",\"Phone\":\"Phone\",\"TimeZone\":\"123\",\"IsDefault\":1,\"IsInternet\":1,\"IsActive\":1,\"ZipCode\":\"1215\",\"MeshMode\":0,\"PassPhrase\":\"64565C\"}]}";

                #endregion

                ChannelJsonParser jsonManager = new ChannelJsonParser(msg, MessageReceivedFrom.NewChannel);
                bool isSuccess = jsonManager.SaveNewChannel();

                if (isSuccess)
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " New Channel Add Successfully.", HttpStatusCode.OK, oRootObject);
                }
                else
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " Can Not Add New Channel.", HttpStatusCode.BadRequest, oRootObject);
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

        [Route("api/DeleteChannel")]
        [HttpPost]
        public HttpResponseMessage DeleteChannel(JObject encryptedString)
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

                ChannelDeleteJsonParser jsonManager = new ChannelDeleteJsonParser(msg, MessageReceivedFrom.DeleteChannel);
                bool isSuccess = jsonManager.DeleteChannel();

                if (isSuccess)
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " Channel Delete Successfully.", HttpStatusCode.OK, oRootObject);
                }
                else
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " Can Not Delete New Channel.", HttpStatusCode.BadRequest, oRootObject);
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
