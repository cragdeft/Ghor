using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Utility.EncryptionAndDecryption
{
    public static class SecurityManager
    {
        static SecurityManager()
        {
            IV = "wrx2XfP&du-taHEY";//CryptLib.GenerateRandomIV(16);//IV
            CypherKey = CryptLib.getHashSha256("7=BMCJrxW8tX_H%X", 32);
            PassCypherKey= CryptLib.getHashSha256("RwKPj7H%", 32);
        }

        public static string Encrypt(string plainText)
        {
            return new CryptLib().encrypt(plainText, CypherKey, IV);
        }

        public static string Decrypt(string cypherText)
        {
            return new CryptLib().decrypt(cypherText, CypherKey, IV);
        }


        public static string PassEncrypt(string plainText)
        {
            return new CryptLib().encrypt(plainText, PassCypherKey, IV);
        }

        public static string PassDecrypt(string cypherText)
        {
            return new CryptLib().decrypt(cypherText, PassCypherKey, IV);
        }

        #region General properties

        private static string IV { get; set; }
        private static string CypherKey { get; set; }
        private static string PassCypherKey { get; set; }
        #endregion
    }
}
