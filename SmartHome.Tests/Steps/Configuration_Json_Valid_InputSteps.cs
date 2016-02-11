using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartHome.Tests.StepHelpers;
using System;
using TechTalk.SpecFlow;
using WatiN.Core;

namespace SmartHome.Tests.Steps
{
    [Binding]
    public class Configuration_Json_Valid_InputSteps
    {
        [Given]
        public void Given_I_am_at_the_configuration_json_input_page()
        {
            WebBrowser.Current.GoTo("http://localhost:8600/Home/About");
        }
        
        [When]
        public void When_I_fill_in_the_following_form(TechTalk.SpecFlow.Table table)
        {
            foreach (var row in table.Rows)
            {
                string test = row["field"];
                var textField = WebBrowser.Current.TextField(Find.ById(test));

                if (!textField.Exists)
                    Assert.Fail("Expected to find a text field with the name of '{0}'.", row["field"]);

                textField.TypeText(row["value"]);
            }
        }
        
        [When]
        public void When_I_click_the_PUBLISH_button_for_publish(string publish)
        {
            var subscribButton = WebBrowser.Current.Button(Find.ByValue("Publish"));

            if (!subscribButton.Exists)
                Assert.Fail("Expected to find a button with the value of 'Publish'.");

            subscribButton.Click();
        }
        
        [When]
        public void When_I_click_the_SUBSCRIB_button_for_subscrib(string subscrib)
        {
            var subscribButton = WebBrowser.Current.Button(Find.ByValue("Subscrib"));

            if (!subscribButton.Exists)
                Assert.Fail("Expected to find a button with the value of 'subscribe'.");

            subscribButton.Click();
        }
        
        [Then]
        public void Then_I_should_be_at_the_subscribe_page()
        {
            var expectedURL = "http://localhost:8600/Home/SubscribeMessage";
            var actualURL = WebBrowser.Current.Url;
            Assert.AreEqual(expectedURL, actualURL);
        }
    }
}
