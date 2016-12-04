using Newtonsoft.Json.Linq;
using SmartHome.Logging;
using SmartHome.Model.Enums;
using SmartHome.Utility.EncryptionAndDecryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SmartHome.WebAPI.Filter
{
  public class LogActionFilterAttribute : ActionFilterAttribute
  {
    public string msg { get; set; }


    public override void OnActionExecuting(HttpActionContext filterContext)
    {
      msg = string.Empty;
      string actionName = filterContext.ActionDescriptor.ActionName;
      MessageReceivedFrom messageReceivedFrom = GetReceivedFrom(actionName);

      JObject json = GetActionArgumentJson(filterContext);
      msg = SecurityManager.Decrypt(json["encryptedString"].ToString());

      new MessageLogger(msg, json["encryptedString"].ToString(), messageReceivedFrom).SaveNewMessageLog();
    }

    private JObject GetActionArgumentJson(HttpActionContext filterContext)
    {
      var encryptedString = filterContext.ActionArguments["encryptedString"].ToString();
      JObject json = JObject.Parse(encryptedString);
      return json;
    }


    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    {
      //base.OnActionExecuted(actionExecutedContext);
      //Stopwatch stopwatch = (Stopwatch)actionExecutedContext.Request.Properties[actionExecutedContext.ActionContext.ActionDescriptor.ActionName];
      //Trace.WriteLine(actionExecutedContext.ActionContext.ActionDescriptor.ActionName + "-Elapsed = " + stopwatch.Elapsed);
    }

    private MessageReceivedFrom GetReceivedFrom(string actionMethodName)
    {
      return (MessageReceivedFrom)System.Enum.Parse(typeof(MessageReceivedFrom), actionMethodName);
    }
  }
}