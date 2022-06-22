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
            ASCIIEncoding ByteConverter = new ASCIIEncoding();

            byte[] dataToEncrypt = ByteConverter.GetBytes(input);


            using RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

            byte[] encryptedData = RSAalg.Encrypt(dataToEncrypt, false);

            return Encoding.UTF8.GetString(encryptedData);
        }

        public static string DecriptPassword(this string input)
        {
            ASCIIEncoding ByteConverter = new ASCIIEncoding();

            byte[] dataEncrypted = ByteConverter.GetBytes(input);

            using RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

            var decryptedData = RSAalg.Decrypt(dataEncrypted, false);

            return Encoding.UTF8.GetString(decryptedData);
        }

        public static bool ComparePassword(this string input, string compare)
        {
            return DecriptPassword(input).Equals(compare);
        }
    }
}