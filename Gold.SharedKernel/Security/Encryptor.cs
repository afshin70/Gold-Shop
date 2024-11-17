using System.Security.Cryptography;
using System.Text;

namespace Gold.SharedKernel.Security
{
    public  class Encryptor
    {
        /// <summary>
        /// رمزنگاری متن
        /// </summary>
        /// <param name="plainText">متن ورودی</param>
        /// <param name="SaltKey">متن زائد جهت رمزنگاری</param>
        /// <returns></returns>
        public static string Encrypt(string plainText, string SaltKey)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(GoldAssembly.GoldPasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(GoldAssembly.GoldVIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }
        /// <summary>
        /// رمزگشایی متن
        /// </summary>
        /// <param name="encryptedText">متن رمز شده</param>
        /// <param name="SaltKey">متن زائد رمز شده</param>
        /// <returns></returns>
        public static string Decrypt(string encryptedText, string SaltKey)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(GoldAssembly.GoldPasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(GoldAssembly.GoldVIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());

        }


    }
    public class GoldAssembly
    {
        public static string GoldEmailHost { get; } = "";
        public static int GoldEmailPort { get; } = 25;
        public static string GoldEmailAddress { get; } = "";
        public static string GoldEmailPassword { get; } = "";
        public static string GoldPasswordHash { get; } = "@84b28167-#792e-%49db-&949a-*5a6c321faa4e";
        public static string GoldVIKey { get; } = "@2c2B3f4e5d6e7g8";
    }
}
