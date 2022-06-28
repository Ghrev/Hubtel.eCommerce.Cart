using System;
using System.Security.Cryptography;
using System.Text;
using Hubtel.eCommerce.Cart.Api;

namespace Hubtel.eCommerce.Cart.Data.Crypto
{
    public static class Cypher
    {
        public static string EncryptMD5(string text)
        {
            string encryptedText;
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            var data = Encoding.UTF8.GetBytes(text);


            using (var md5 = new MD5CryptoServiceProvider())
            {
                var keys = md5.ComputeHash(Encoding.UTF8.GetBytes(AppConfig.ENCRYPTION_KEY));
                using (var tripDes = new TripleDESCryptoServiceProvider { Key = keys, Mode = CipherMode.ECB })
                {
                    var transform = tripDes.CreateEncryptor();
                    var results = transform.TransformFinalBlock(data, 0, data.Length);
                    encryptedText = Convert.ToBase64String(results, 0, results.Length);
                }
            }
            return encryptedText.Replace("+", "_").Replace("/", "-");

        }


        public static string DecryptMD5(string cipher)
        {
            if (string.IsNullOrWhiteSpace(cipher))
                return string.Empty;
            cipher = cipher.Replace(" ", "+").Replace("_", "+").Replace("-", "/");

            var data = Convert.FromBase64String(cipher);

            using (var md5 = new MD5CryptoServiceProvider())
            {
                var keys = md5.ComputeHash(Encoding.UTF8.GetBytes(AppConfig.ENCRYPTION_KEY));

                using (var tripDes = new TripleDESCryptoServiceProvider()
                {
                    Key = keys,
                    Mode = CipherMode.ECB,
                    //Padding = PaddingMode.PKCS7
                })
                {
                    var transform = tripDes.CreateDecryptor();
                    var results = transform.TransformFinalBlock(data, 0, data.Length);
                    return Encoding.UTF8.GetString(results);
                }
            }
        }

        public static string EncryptSHA256(string rawText)
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(rawText));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}

