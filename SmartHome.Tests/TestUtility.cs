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
            string plainTextForIOS = "Test)";
            string plainTextForAndroid = "Hi there";
            string encryptedTextForIOS = "P7Bv3wibqUgKOFmFU+v1SQ==";
            string encryptedTextForAndroid = "yfOoqzkm3tK97qgleRZPHA==";

            // Act

            string encryptedValueIOS = SecurityManager.Encrypt(plainTextForIOS);
            string decryptedValueIOS = SecurityManager.Decrypt(encryptedValueIOS);

            string encryptedValueAndroid = SecurityManager.Encrypt(plainTextForAndroid);
            string decryptedValueAndroid = SecurityManager.Decrypt(encryptedValueAndroid);

            //Assert for IOS
            Assert.AreEqual(encryptedTextForIOS, encryptedValueIOS);
            Assert.AreEqual(plainTextForIOS, decryptedValueIOS);

            //Assert for Android
            Assert.AreEqual(encryptedTextForAndroid, encryptedValueAndroid);
            Assert.AreEqual(plainTextForAndroid, decryptedValueAndroid);
        }

        [TestMethod]
        public void CommandPerser_ShouldSaveCommand()
        {
            byte[] command = new byte[] { 1, 2, 2, 1};
            CommandParser ft = new CommandParser(command);
        }
    }
}
