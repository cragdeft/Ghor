using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartHome.MQTT.Client;
using SmartHome.Utility.EncriptionAndDecryption;

namespace SmartHome.Tests
{
    [TestClass]
    public class TestUtility
    {
        [TestMethod]
        public void Encrypt_ShouldReturnEncryptedString()
        {

            // Arrange
            var plainTextForIos = "Hi there";
            string plainTextForAndroid = "smart home";
            const string encryptedTextForIos = "6NTdfJFMBWKUW5IQtKKZUQ==";
            const string encryptedTextForAndroid = "4tT5SFAoxd/Z+m5bpVyG3Q==";

            // Act

            var encryptedValueIos = SecurityManager.Encrypt(plainTextForIos);
            var decryptedValueIos = SecurityManager.Decrypt(encryptedValueIos);

            var encryptedValueAndroid = SecurityManager.Encrypt(plainTextForAndroid);
            var decryptedValueAndroid = SecurityManager.Decrypt(encryptedValueAndroid);

            //Assert for IOS
            Assert.AreEqual(encryptedTextForIos, encryptedValueIos);
            Assert.AreEqual(plainTextForIos, decryptedValueIos);

            //Assert for Android
            Assert.AreEqual(encryptedTextForAndroid, encryptedValueAndroid);
            Assert.AreEqual(plainTextForAndroid, decryptedValueAndroid);
        }
    }
}
