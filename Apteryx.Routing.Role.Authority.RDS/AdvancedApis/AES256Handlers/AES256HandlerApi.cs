namespace Apteryx.Routing.Role.Authority.RDS
{
    public static class AES256HandlerApi
    {
        private static AES256Handler aes256Handler = null;

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="encryptStr">明文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">偏移量</param>
        /// <returns></returns>
        public static string Encrypt(string encryptStr, string key, string iv)
        {
            if (string.IsNullOrWhiteSpace(encryptStr))
                throw new ArgumentNullException(nameof(encryptStr));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if(string.IsNullOrWhiteSpace(iv))
                throw new ArgumentNullException(nameof(iv));

            if (aes256Handler == null)
                aes256Handler = new AES256Handler();
            return aes256Handler.Encrypt(encryptStr, key, iv);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="decryptStr">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">偏移量</param>
        /// <returns></returns>
        public static string Decrypt(string decryptStr, string key, string iv)
        {
            if (string.IsNullOrWhiteSpace(decryptStr))
                throw new ArgumentNullException(nameof(decryptStr));
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(iv))
                throw new ArgumentNullException(nameof(iv));

            if (aes256Handler == null)
                aes256Handler = new AES256Handler();
            return aes256Handler.Decrypt(decryptStr, key, iv);
        }
    }
}
