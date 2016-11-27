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
        //msg = "{\"Device\":[{\"Version\":0,\"Id\":2,\"Watt\":0,\"IsDeleted\":0,\"DeviceType\":1,\"DeviceName\":\"SMSW6G-00 1205326898\",\"DeviceId\":32773,\"DeviceHash\":1205326898,\"Room\":1}],\"Configuration\":\"NewDevice\",\"DeviceStatus\":[{\"StatusType\":53,\"StatusValue\":1,\"DeviceTableId\":2,\"Id\":1},{\"StatusType\":55,\"StatusValue\":0,\"DeviceTableId\":2,\"Id\":2},{\"StatusType\":5,\"StatusValue\":1,\"DeviceTableId\":2,\"Id\":3}],\"Home\":[{\"Block\":\"Block\",\"Country\":\"Country\",\"Address1\":\"address1\",\"Address2\":\"address2\",\"Id\":2,\"Name\":\"MyHome\",\"City\":\"City\",\"IsSynced\":1,\"Zone\":\"123\",\"Phone\":\"Phone\",\"TimeZone\":\"123\",\"IsDefault\":1,\"IsInternet\":1,\"IsActive\":1,\"ZipCode\":\"1215\",\"MeshMode\":0,\"PassPhrase\":\"B1E97D\"}]}";
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


    [Route("api/NewCameraConfigInfo")]
    [HttpPost]
    public HttpResponseMessage NewCameraConfigInfo(JObject encryptedString)
    {
      HttpResponseMessage response;
      PasswordRecoveryRootObjectEntity oRootObject = new PasswordRecoveryRootObjectEntity();
      try
      {
        #region Initialization

        oRootObject.data = new PasswordRecoveryObjectEntity();
        string msg = string.Empty;

        msg = SecurityManager.Decrypt(encryptedString["encryptedString"].ToString());
        // msg = "{\"Configuration\":\"CameraConfigInfo\",\"Home\":[{\"Id\":1,\"PassPhrase\":\"4905a51c1b987b8f_U4BXUO\"}],\"CameraConfigInfo\":[{\"Id\":1,\"Ip\":\"192.168.11.201\",\"Port\":8002,\"UserName\":\"smarthometest\",\"Password\":\"123456\",\"Home\":1,\"IsSynced\":0}]}";
        if (string.IsNullOrEmpty(msg))
        {
          return response = Request.CreateResponse(HttpStatusCode.BadRequest, "Not have sufficient information to process.");
        }

        #endregion

        CameraConfigInfoJsonParser jsonManager = new CameraConfigInfoJsonParser(msg, MessageReceivedFrom.NewCameraConfigInfo);
        bool isSuccess = jsonManager.SaveNewDevice();

        if (isSuccess)
        {
          MessageResponseUtility.FillPasswordRecoveryInfos("", " New Camera Config Info Add Successfully.", HttpStatusCode.OK, oRootObject);
        }
        else
        {
          MessageResponseUtility.FillPasswordRecoveryInfos("", " Can Not Add New Camera Config Info.", HttpStatusCode.BadRequest, oRootObject);
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
