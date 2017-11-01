using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Security.Cryptography;

namespace FrameWork
{
    public static class StringUtils
    {
        public static readonly Dictionary<DayOfWeek, string> WeekDayStr = new Dictionary<DayOfWeek, string>()
        {
            { DayOfWeek.Sunday, "星期天"},
            { DayOfWeek.Monday, "星期一"},
            { DayOfWeek.Tuesday, "星期二"},
            { DayOfWeek.Wednesday, "星期三"},
            { DayOfWeek.Thursday, "星期四"},
            { DayOfWeek.Friday, "星期五"},
            { DayOfWeek.Saturday, "星期六"},
        };

        // Format into "xx年xx月xx日 星期x"
        public static string FormatTimeYMDW(DateTime date)
        {
            return string.Format("{0}年{1}月{2}日 {3}", date.Year, date.Month, date.Day, WeekDayStr[date.DayOfWeek]);
        }

        public static StringBuilder AppendIndent(StringBuilder sb, int indentLevel)
        {
            for (int i = 0; i < indentLevel; i++)
            {
                sb.Append("\t");
            }

            return sb;
        }

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        private const string defaultPhrase = @"xYyXlEXVOZLTK";
        public static string Encrypt(string plainText, string passPhrase = defaultPhrase)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            Rfc2898DeriveBytes password = null;
            try
            {
                password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations);
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
            finally
            {
            }
        }

        public static string Decrypt(string cipherText, string passPhrase = defaultPhrase)
        {
            //UnityEngine.Debug.Log("----------------------DeCrypt : " + cipherText);
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            Rfc2898DeriveBytes password = null;
            try
            {
                password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations);
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
            finally
            { }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            RNGCryptoServiceProvider rngCsp = null;
            try
            {
                rngCsp = new RNGCryptoServiceProvider();
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            finally { }
            return randomBytes;
        }
        
    }
}