using System.Security.Cryptography;
using System.Text;

namespace Apteryx.Routing.Role.Authority.RDS
{
    public class AES256Handler
    {
        ///// <summary>
        ///// AES加密
        ///// </summary>
        ///// <param name="encryptStr">明文</param>
        ///// <param name="key">密钥</param>
        ///// <returns></returns>
        //public string Encrypt(string encryptStr, string key)
        //{
        //    byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
        //    byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(encryptStr);
        //    RijndaelManaged rDel = new RijndaelManaged();
        //    rDel.Key = keyArray;
        //    rDel.Mode = CipherMode.ECB;
        //    rDel.Padding = PaddingMode.PKCS7;
        //    ICryptoTransform cTransform = rDel.CreateEncryptor();
        //    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        //    return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        //}

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="encryptStr">明文字符串</param>
        /// <param name="key">秘钥</param>
        /// <param name="iv">加密辅助向量</param>
        /// <returns>密文</returns>
        public string Encrypt(string encryptStr, string key, string iv)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length) len = keyBytes.Length;
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
            rijndaelCipher.IV = ivBytes;
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(encryptStr);
            byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);
            return Convert.ToBase64String(cipherBytes);
        }

        ///// <summary>
        ///// AES解密
        ///// </summary>
        ///// <param name="decryptStr">密文</param>
        ///// <param name="key">密钥</param>
        ///// <returns></returns>
        //public string Decrypt(string decryptStr, string key)
        //{
        //    byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
        //    byte[] toEncryptArray = Convert.FromBase64String(decryptStr);
        //    RijndaelManaged rDel = new RijndaelManaged();
        //    rDel.Key = keyArray;
        //    rDel.Mode = CipherMode.ECB;
        //    rDel.Padding = PaddingMode.PKCS7;
        //    ICryptoTransform cTransform = rDel.CreateDecryptor();
        //    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        //    return UTF8Encoding.UTF8.GetString(resultArray);
        //}

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="decryptStr">加密字符串</param>
        /// <param name="key">秘钥</param>
        /// <param name="iv">加密辅助向量</param>
        /// <returns>明文</returns>
        public string Decrypt(string decryptStr, string key, string iv)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] encryptedData = Convert.FromBase64String(decryptStr);
            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length) len = keyBytes.Length;
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
            rijndaelCipher.IV = ivBytes;
            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
            byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }
    }
}
