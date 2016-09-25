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
    public class RoomInfoController : ApiController
    {
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

                msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
                if (string.IsNullOrEmpty(msg))
                {
                    return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
                }

                #endregion

                RoomJsonParser jsonManager = new RoomJsonParser(msg, MessageReceivedFrom.NewRoom);
                bool isSuccess = jsonManager.SaveNewRoom();

                if (isSuccess)
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " New Room Add Successfully.", HttpStatusCode.OK, oRootObject);
                }
                else
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " Can Not Add New Room.", HttpStatusCode.BadRequest, oRootObject);
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

        [Route("api/DeleteRoom")]
        [HttpPost]
        public HttpResponseMessage DeleteRoom(JObject encryptedString)
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

                RoomDeleteJsonParser jsonManager = new RoomDeleteJsonParser(msg, MessageReceivedFrom.DeleteRoom);
                bool isSuccess = jsonManager.DeleteRoom();

                if (isSuccess)
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " Room Delete Successfully.", HttpStatusCode.OK, oRootObject);
                }
                else
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " Can Not Delete Room.", HttpStatusCode.BadRequest, oRootObject);
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


        [Route("api/DeviceRoomUpdate")]
        [HttpPost]
        public HttpResponseMessage DeviceRoomUpdate(JObject encryptedString)
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

                DeviceRoomJsonParser jsonManager = new DeviceRoomJsonParser(msg, MessageReceivedFrom.DeviceRoomUpdate);
                bool isSuccess = jsonManager.DeviceRoomUpdate();

                if (isSuccess)
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", "Device Room Update Successfully.", HttpStatusCode.OK, oRootObject);
                }
                else
                {
                    MessageResponseUtility.FillPasswordRecoveryInfos("", " Can Not Device Room Update.", HttpStatusCode.BadRequest, oRootObject);
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
