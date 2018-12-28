using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace GeneralLayer
{
    //Encryption and decryption using AES 256
    public class Cryptography
    {
        private static string stringKey = "icwm53v3w1u903hyjiw5o6s3vnehie3u";

        private byte[] keyBytes = Encoding.UTF8.GetBytes(stringKey);
        private byte[] ivBytes = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        protected byte[] cipherData;

        public string encryptInfo(string inputText)
        {

            cipherData = AES_256.encryptStringToBytes(inputText, keyBytes, ivBytes);
            string userinfo64Text = Convert.ToBase64String(cipherData);
            return userinfo64Text;
        }

        public string decryptInfo(string outputText)
        {
            byte[] inputbytes = Convert.FromBase64String(outputText);
            string plainText = AES_256.decryptStringFromBytes(inputbytes, keyBytes, ivBytes);
            return plainText;
        }

        public string EncryptURLSafe(string strText)
        {
            return Encrypt_URLSafe(strText, "&%#@?,:*");
        }

        public string Decrypt_URLSafe(string strText, string sDecrKey)
        {
            strText = strText.Replace("%2B", "+");
            byte[] byKey = { };
            byte[] IV = { 0X12, 0X34, 0X56, 0X78, 0X90, 0XAB, 0XCD, 0XEF };
            byte[] inputByteArray = new byte[strText.Length + 1];

            try
            {
                byKey = System.Text.Encoding.UTF8.GetBytes(sDecrKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);

                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;

                return encoding.GetString(ms.ToArray());

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public string Encrypt_URLSafe(string strText, string strEncrKey)
        {
            string cipher = "";

            byte[] byKey = { };
            byte[] IV = { 0X12, 0X34, 0X56, 0X78, 0X90, 0XAB, 0XCD, 0XEF };

            try
            {
                byKey = System.Text.Encoding.UTF8.GetBytes(strEncrKey.Substring(0, 8));

                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                cipher = Convert.ToBase64String(ms.ToArray());
                cipher = cipher.Replace("+", "%2B");
                return cipher;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string DecryptURLSafe(string strText)
        {
            return Decrypt_URLSafe(strText, "&%#@?,:*");
        }
    }
}
