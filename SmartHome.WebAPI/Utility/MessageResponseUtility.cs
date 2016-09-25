using Newtonsoft.Json;
using SmartHome.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace SmartHome.WebAPI.Utility
{
    public class MessageResponseUtility
    {
        public static void FillPasswordRecoveryInfos(string userPassword, string message, HttpStatusCode code, PasswordRecoveryRootObjectEntity oRootObject)
        {
            oRootObject.data.Password = userPassword;
            oRootObject.MESSAGE = new LoginMessage();
            oRootObject.MESSAGE = SetResponseMessage(message, code);
        }
        public static LoginMessage SetResponseMessage(string message, HttpStatusCode code)
        {
            LoginMessage oLoginMessage = new LoginMessage();
            oLoginMessage.HTTP_MESSAGE = message;
            oLoginMessage.HTTP_STATUS = (int)code;
            return oLoginMessage;
        }

        public static HttpResponseMessage PrepareJsonResponse<T>(T oRootObject)
        {
            string msg = JsonConvert.SerializeObject(oRootObject);
            return new HttpResponseMessage() { Content = new StringContent(msg, Encoding.UTF8, "application/json") };
        }
        public static  HttpResponseMessage ProcessGetRegisteredUser(bool isRegisterSucces)
        {
            HttpResponseMessage response;
            LoginRootObjectEntity oRootObject = new LoginRootObjectEntity();
            try
            {

                if (isRegisterSucces)
                {
                    oRootObject.data = new LoginObjectEntity();
                    response = PrepareJsonResponse(oRootObject, "Unique user", HttpStatusCode.OK);
                }
                else
                {
                    oRootObject.data = new LoginObjectEntity();
                    response = PrepareJsonResponse(oRootObject, "User already exist", HttpStatusCode.Conflict);
                }
            }
            catch (Exception ex)
            {
                oRootObject.data = new LoginObjectEntity();
                response = PrepareJsonResponse(oRootObject, ex.ToString(), HttpStatusCode.BadRequest);
            }

            return response;
        }
        public static  HttpResponseMessage PrepareJsonResponseForGetUserInfos(LoginRootObjectEntity oRootObject, string message, HttpStatusCode code)
        {
            oRootObject.MESSAGE = new LoginMessage();
            oRootObject.MESSAGE = SetLoginMessage(message, code);
            string msg = JsonConvert.SerializeObject(oRootObject);
            msg = msg.Replace("false", "0");
            msg = msg.Replace("true", "1");
            return new HttpResponseMessage() { Content = new StringContent(msg, Encoding.UTF8, "application/json") };
        }
        public static HttpResponseMessage PrepareJsonResponse(LoginRootObjectEntity oRootObject, string message, HttpStatusCode code)
        {
            oRootObject.MESSAGE = new LoginMessage();
            oRootObject.MESSAGE = SetLoginMessage(message, code);
            string msg = JsonConvert.SerializeObject(oRootObject);
            return new HttpResponseMessage() { Content = new StringContent(msg, Encoding.UTF8, "application/json") };
        }

        public static LoginMessage SetLoginMessage(string message, HttpStatusCode code)
        {
            LoginMessage oLoginMessage = new LoginMessage();
            oLoginMessage.HTTP_MESSAGE = message;
            oLoginMessage.HTTP_STATUS = (int)code;
            return oLoginMessage;
        }
    }
}