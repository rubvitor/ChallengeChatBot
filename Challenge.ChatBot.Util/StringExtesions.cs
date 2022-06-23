using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Challenge.ChatBot.Util
{
    public static class StringExtesions
    {
        public static ArraySegment<byte> ToArraySegment<T>(this T input) where T : class
        {
            var stringData = JsonConvert.SerializeObject(input);
            var bytes = Encoding.UTF8.GetBytes(stringData);

            return new ArraySegment<byte>(bytes, 0, bytes.Length);
        }

        public static string ArraySegmentByteToString(this ArraySegment<byte> input)
        {
            return Encoding.Default.GetString(input);
        }

        public static string EncriptPassword(this string input)
        {
            var key = Encoding.UTF8.GetBytes(Security.Key);

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                            swEncrypt.Write(input);

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        public static string DecriptPassword(this string input)
        {
            var fullCipher = Convert.FromBase64String(input);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = Encoding.UTF8.GetBytes(Security.Key);

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }

        public static bool ComparePassword(this string input, string compare)
        {
            return DecriptPassword(input).Equals(compare);
        }
    }
}