using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Moq;
using NUnit.Framework;
using SmartHome.Entity;
using SmartHome.MailApi.Controllers;
using SmartHome.Model.Models;
using SmartHome.Service.Interfaces;
using SmartHome.Utility.EncriptionAndDecryption;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SmartHome.Tests.Steps
{
    [Binding]
    public class SendEmailSteps
    {
        [Given]
        public void Given_I_have_a_EmailoEntity_record_with_the_following_properties(Table table)
        {
            
            ScenarioContext.Current.Set<EmailEntity>(table.CreateInstance<EmailEntity>());
        }

        [Given]
        public void Given_I_have_a_EmailoEntity_record_with_the_following_property(Table table)
        {
            foreach (string jsonString in table.Rows.Select(row => row["value"]))
            {
                ScenarioContext.Current.Set<string>(jsonString);
            }
        }

        [Then]
        public void Then_I_will_check_Email_sent_or_not_and_response()
        {
            Assert.AreEqual(ScenarioContext.Current.Get<HttpResponseMessage>("SaveResult").StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(ScenarioContext.Current.Get<HttpResponseMessage>("SaveResult").Content.ReadAsStringAsync().Result, "true");
        }

        [When]
        public void When_I_send_the_Email()
        {
            EmailController = new EmailController
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            ScenarioContext.Current.Set<HttpResponseMessage>(EmailController.SendMail(ScenarioContext.Current.Get<string>()), "SaveResult");
        }


        public EmailController EmailController { get; set; }
    }
}
