﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.0.0.0
//      SpecFlow Generator Version:2.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace SmartHome.Tests.Feature
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Configuration_Json_Valid_Input")]
    public partial class Configuration_Json_Valid_InputFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Configuration_Json_Parse_Input_Check.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Configuration_Json_Valid_Input", "\tIn order to avoid silly mistakes\r\n\tI want to check json parse process with varit" +
                    "y of input set", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Configuration json with valid input")]
        public virtual void ConfigurationJsonWithValidInput()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Configuration json with valid input", ((string[])(null)));
#line 5
this.ScenarioSetup(scenarioInfo);
#line 6
 testRunner.Given("I am at the configuration json input page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "field",
                        "value"});
            table1.AddRow(new string[] {
                        "MessgeTopic",
                        "Configuration"});
            table1.AddRow(new string[] {
                        "PublishMessage",
                        @"{""Version"":[{""Id"":1,""AppName"":""SmartHome"",""AppVersion"":""1.5"",""AuthCode"":""0123456789ABCDEF"",""PassPhrase"":""Y1JJ9N""}],""VersionDetails"":[{""Id"":1,""VersionId"":1,""HardwareVersion"":""00"",""DeviceType"":1},{""Id"":2,""VersionId"":1,""HardwareVersion"":""00"",""DeviceType"":2}],""Device"":[{""Id"":1,""DeviceId"":32769,""DeviceHash"":1606113433,""DeviceType"":0,""DeviceName"":""SMSW6G 1606113433"",""Version"":""00"",""IsDeleted"":false,""Watt"":0}],""DeviceStatus"":[{""Id"":1,""DeviceTableId"":1,""StatusType"":53,""StatusValue"":""1""},{""Id"":2,""DeviceTableId"":1,""StatusType"":5,""StatusValue"":""1""}],""Channel"":[{""Id"":1,""DeviceTableId"":1,""ChannelNo"":2,""LoadType"":3,""LoadName"":""Fan"",""LoadWatt"":0},{""Id"":2,""DeviceTableId"":1,""ChannelNo"":6,""LoadType"":5,""LoadName"":""CFL"",""LoadWatt"":0}],""NextAssociatedDeviceId"":[{""NextDeviceId"":32770}],""ChannelStatus"":[{""Id"":1,""ChannelTableId"":1,""StatusType"":1,""StatusValue"":""1""},{""Id"":2,""ChannelTableId"":1,""StatusType"":3,""StatusValue"":""0""},{""Id"":3,""ChannelTableId"":1,""StatusType"":2,""StatusValue"":""53""},{""Id"":4,""ChannelTableId"":2,""StatusType"":1,""StatusValue"":""0""},{""Id"":5,""ChannelTableId"":2,""StatusType"":3,""StatusValue"":""0""},{""Id"":6,""ChannelTableId"":2,""StatusType"":2,""StatusValue"":""100""}]}"});
#line 7
 testRunner.When("I fill in the following form", ((string)(null)), table1, "When ");
#line 11
  testRunner.And("I click the \'publish\' button for publish", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 12
  testRunner.And("I click the \'subscribe\' button for subscrib", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 13
 testRunner.Then("I should be at the subscribe page", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
