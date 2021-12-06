using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Linq;

namespace scabackend.Classes
{
    public static class HashClass
    {
        private const string publickey = "Fjx4xOvYjRJrnKLYyKjiy5QR4LYj3ZNkjkoRzEQkMSs=";
        private const string secretkey = "HeTOnAbrdJpcqRlkCaYhQQ==";

        public static string Encrypt(string value)
        {
            byte[] keyBytes = Convert.FromBase64String(publickey);

            byte[] ivBytes = Convert.FromBase64String(secretkey);

            byte[] encrypted = EncryptStringToBytes_Aes(value, keyBytes, ivBytes);

            string encryptedString = Convert.ToBase64String(encrypted);

            return encryptedString;

        }

        public static string Decrypt(string value)
        {
            byte[] keyBytes = Convert.FromBase64String(publickey);

            byte[] ivBytes = Convert.FromBase64String(secretkey);

            byte[] decryptedBytes = Convert.FromBase64String(value);

            string decryptString = DecryptStringFromBytes_Aes(decryptedBytes, keyBytes, ivBytes);

            return decryptString;
        }

        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
