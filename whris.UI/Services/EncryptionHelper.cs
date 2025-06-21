using System.Text;

namespace whris.UI.Services
{
    public class EncryptionHelper
    {
        private static readonly byte _xorKey = 0x55; // A simple byte key

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return null;

            byte[] data = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedData = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                encryptedData[i] = (byte)(data[i] ^ _xorKey);
            }

            return Convert.ToBase64String(encryptedData);
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return null;

            byte[] encryptedData = Convert.FromBase64String(cipherText);
            byte[] decryptedData = new byte[encryptedData.Length];

            for (int i = 0; i < encryptedData.Length; i++)
            {
                decryptedData[i] = (byte)(encryptedData[i] ^ _xorKey);
            }

            return Encoding.UTF8.GetString(decryptedData);
        }
    }
}