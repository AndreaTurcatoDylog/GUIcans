using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// This class manage the ASC Criptografy.
    /// To create the Key and the vector IV must create a new instance of the Aes class.  
    /// This generates a new key and initialization vector (IV).
    ///  Example -> Aes myAes = Aes.Create();
    /// </summary>
    public static class AESCryptography
    {
        #region Constants

        //public static readonly byte[] Key = new byte[] { 59, 76, 6, 196, 33, 144, 81, 91, 66, 66, 232, 155, 125, 60, 248, 174, 168, 121, 224, 145, 217, 195, 10, 63, 253, 19, 23, 222, 98, 38, 46, 249 };
        public static readonly byte[] IV = new byte[] { 146, 156, 206, 112, 204, 153, 217, 195, 72, 13, 86, 106, 11, 19, 145, 172 };

        #endregion

        #region Properties

        public static byte[] Information;
        public static bool _isDirty;

        #endregion

        #region Methods

        /// <summary>
        /// Create new Key and IV and save in file
        /// </summary>
        public static void CreateKeyAndIV()
        {
            Aes myAes = Aes.Create();
            string path = @"AESKey.txt";

            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {

                sw.WriteLine("Key");
                sw.WriteLine("");

                var key = myAes.Key.ToList();
                foreach (var k in key)
                {
                    sw.Write($", {k}");
                }

                sw.WriteLine("");
                sw.WriteLine($"IV");
                sw.WriteLine("");

                var iv = myAes.IV.ToList();
                foreach (var i in iv)
                {
                    sw.Write($", {i}");
                }
            }

        }

        /// <summary>
        /// Encript the string to byte
        /// </summary>
        public static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        /// <summary>
        /// Decript the byte to string
        /// </summary>
        public static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        /// <summary>
        /// Decrypt an encrypted string value
        /// </summary>
        public static string DecryptString(string value)
        {
            //if (!AESCryptography._isDirty)
            //{
            //    AESCryptography._isDirty = true;

            //    var length = AESCryptography.Information.Length;
            //    var first = AESCryptography.Information.Take(length / 2).ToArray();
            //    var second = AESCryptography.Information.Skip(length / 2).ToArray();

            //    var inf = second.Concat(first).ToArray();

            //    AESCryptography.Information = inf;
            //}

            //AESCryptography.Information = new byte[] { 59, 76, 6, 196, 33, 144, 81, 91, 66, 66, 232, 155, 125, 60, 248, 174, 168, 121, 224, 145, 217, 195, 10, 63, 253, 19, 23, 222, 98, 38, 46, 249 };

            if (!string.IsNullOrEmpty(value))
            {
                var bytes = Convert.FromBase64String(value);

                try
                {
                    //var stringValue = AESCryptography.DecryptStringFromBytes_Aes(bytes, AESCryptography.Key, AESCryptography.IV);
                    var stringValue = AESCryptography.DecryptStringFromBytes_Aes(bytes, AESCryptography.Information, AESCryptography.IV);
                    return stringValue;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }

            return string.Empty;
        }

        #endregion
    }
}
