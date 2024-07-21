using System.Security.Cryptography;

namespace SMS.App.Common
{
    public static class Cryptography
    {
        /// <summary>
        /// Encrypt string using a key provided as parameter.
        /// </summary>
        /// <param name="stringToEncode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string EncodeStringWithKey(this string stringToEncode, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(key);
                aesAlg.GenerateIV();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(stringToEncode);
                        }
                    }

                    byte[] iv = aesAlg.IV;
                    byte[] encryptedData = msEncrypt.ToArray();

                    byte[] result = new byte[iv.Length + encryptedData.Length];
                    Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                    Buffer.BlockCopy(encryptedData, 0, result, iv.Length, encryptedData.Length);

                    return Convert.ToBase64String(result);
                }
            }
        }
        /// <summary>
        /// DEcrypt string using a key provided as parameter.
        /// </summary>
        /// <param name="stringToDecode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DecodeStringWithKey(this string stringToDecode, string key)
        {
            byte[] fullCipher = Convert.FromBase64String(stringToDecode);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(key);

                aesAlg.IV = fullCipher.Take(aesAlg.BlockSize / 8).ToArray();

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(fullCipher.Skip(aesAlg.BlockSize / 8).ToArray()))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

    }
}
