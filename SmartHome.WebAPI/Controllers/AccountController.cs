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
using SmartHome.Logging;

namespace SmartHome.WebAPI.Controllers
{
  //test
  public class AccountController : ApiController
  {
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

      #endregion

      RegisterUserDataProcessor registerUser = new RegisterUserDataProcessor(msg, MessageReceivedFrom.RegisterUser);
      bool isRegisterSucces = registerUser.RegisterUser();

      response = MessageResponseUtility.ProcessGetRegisteredUser(isRegisterSucces);
      return response;
    }

    [Route("api/IsUserExist")]
    [HttpPost]
    public HttpResponseMessage IsUserExist(JObject encryptedString)
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

        //new MessageLogger(msg, MessageReceivedFrom.IsUserExist).SaveNewMessageLog();

        UserDatatProcessor userData = new UserDatatProcessor(msg, MessageReceivedFrom.IsUserExist);
        bool isSuccess = userData.IsUserExist();

        if (isSuccess)
        {
          MessageResponseUtility.FillPasswordRecoveryInfos("", "User already exist", HttpStatusCode.Conflict, oRootObject);
        }
        else
        {
          MessageResponseUtility.FillPasswordRecoveryInfos("", "User not exist", HttpStatusCode.OK, oRootObject);
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

    [Route("api/ChangePassword")]
    [HttpPost]
    public HttpResponseMessage ChangePassword(JObject encryptedString)
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

        //new MessageLogger(msg, MessageReceivedFrom.ChangePassword).SaveNewMessageLog();

        PasswordDataProcessor passwordData = new PasswordDataProcessor(msg, MessageReceivedFrom.ChangePassword);
        bool isSuccess = passwordData.ChangePassword();

        if (isSuccess)
        {
          MessageResponseUtility.FillPasswordRecoveryInfos("", "Successfully Password Update", HttpStatusCode.OK, oRootObject);
        }
        else
        {
          MessageResponseUtility.FillPasswordRecoveryInfos("", "Password not update", HttpStatusCode.BadRequest, oRootObject);
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

    [Route("api/PasswordRecovery")]
    [HttpPost]
    public HttpResponseMessage PasswordRecovery(JObject encryptedString)
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

        //new MessageLogger(msg, MessageReceivedFrom.PasswordRecovery).SaveNewMessageLog();

        PasswordRecoveryDataProcessor passworRecoverydData = new PasswordRecoveryDataProcessor(msg, MessageReceivedFrom.PasswordRecovery, oRootObject);
        bool isSuccess = passworRecoverydData.PasswordRecovery();

        if (isSuccess)
        {
          MessageResponseUtility.FillPasswordRecoveryInfos(oRootObject.data.Password, "User password", HttpStatusCode.OK, oRootObject);
        }
        else
        {
          MessageResponseUtility.FillPasswordRecoveryInfos(string.Empty, "User not exist", HttpStatusCode.BadRequest, oRootObject);
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
