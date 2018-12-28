﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace GeneralLayer
{
    class AES_256
    {
        public static byte[] encryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentNullException("plainText");

            }

            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("iv");
            }

            MemoryStream memoryStream = null;
            AesManaged aesAlg = null;

            try
            {
                // Create the encryption algorithm object with the specified key and IV.
                aesAlg = new AesManaged();
                aesAlg.Key = key;
                aesAlg.IV = iv;


                // Create an encryptor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                memoryStream = new MemoryStream();

                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                using (var streamWriter = new StreamWriter(cryptoStream))
                {
                    //Write all data to the stream.
                    streamWriter.Write(plainText);
                }
            }
            finally
            {
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }

            // Return the encrypted bytes from the memory stream.
            return memoryStream.ToArray();
        }

        public static string decryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }

            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");

            }

            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("iv");
            }

            AesManaged aesAlg = null;
            string plaintext = null;

            try
            {
                // Create a the encryption algorithm object with the specified key and IV.
                aesAlg = new AesManaged();
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (var memoryStream = new MemoryStream(cipherText))
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                using (var streamReader = new StreamReader(cryptoStream))
                {
                    // Read the decrypted bytes from the decrypting stream
                    // and place them in a string.
                    plaintext = streamReader.ReadToEnd();
                }
            }
            finally
            {
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }

            return plaintext;
        }

    }

}