using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Ionic.Zlib;

namespace Assets.SimpleZip
{
    public static class Zip
    {
        /// <summary>
        /// Compress plain text to byte array
        /// </summary>
        public static byte[] Compress(string text)
        {
            return ZlibStream.CompressString(text);
        }

        /// <summary>
        /// Compress plain text to compressed string
        /// </summary>
        public static string CompressToString(string text)
        {
            var data = Convert.ToBase64String(Compress(text));
            data = Encrypt(data);
            return data;
            //return Convert.ToString(Compress(text));
        }

        /// <summary>
        /// Decompress byte array to plain text
        /// </summary>
        public static string Decompress(byte[] bytes)
        {
            return Encoding.UTF8.GetString(ZlibStream.UncompressBuffer(bytes));
        }

        /// <summary>
        /// Decompress compressed string to plain text
        /// </summary>
        public static string Decompress(string data)
        {
            data = Decrypt(data);
            data = Decompress(Convert.FromBase64String(data));
            return data;
        }

        #region Encrypt 1

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "PFrI9W0x";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey,
                    new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }

                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }

            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "PFrI9W0x";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey,
                    new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }

                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return cipherText;
        }

        #endregion
    }
}