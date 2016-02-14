using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Utility.EncriptionAndDecryption
{
    public static class SecurityManager
    {
        static SecurityManager()
        {
            RandomString = "wrx2XfP&du-taHEY";//CryptLib.GenerateRandomIV(16);
            CypherKey = CryptLib.getHashSha256("7=BMCJrxW8tX_H%X", 32);
        }

        public static string Encrypt(string plainText)
        {
            return new CryptLib().encrypt(plainText, CypherKey, RandomString);
        }

        public static string Decrypt(string cypherText)
        {
            return new CryptLib().decrypt(cypherText, CypherKey, RandomString);
        }
        #region General properties

        private static string RandomString { get; set; }
        private static string CypherKey { get; set; }
        #endregion
    }
}
