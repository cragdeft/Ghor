using System;
using NUnit.Framework;
using SmartHome.Entity;
using SmartHome.Mail;
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
        
        [When]
        public void When_I_save_the_Email()
        {
            SmartHomeMailClient mailClient = new SmartHomeMailClient(ScenarioContext.Current.Get<EmailEntity>());
            ScenarioContext.Current.Set<bool>(mailClient.SendEmail(true), "SaveResult");
        }
        
        [Then]
        public void Then_I_will_check_Email_sent_or_not()
        {
            Assert.AreEqual(ScenarioContext.Current.Get<bool>("SaveResult"), true);

        }
    }
}
