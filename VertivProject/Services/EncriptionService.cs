using System.Security.Cryptography;
using System.Text;
using VertivProject.Interfaces;

namespace VertivProject.Services
{
    public class EncriptionService : IEncriptionService
    {
        public string EncryptString(string plainText, string secretKey)
        {
            using (Aes aes = Aes.Create())
            {
                // Derive key and initialization vector (IV) from the secret key
                byte[] key = Encoding.UTF8.GetBytes(secretKey.PadRight(32, '0').Substring(0, 32));
                aes.Key = key;
                aes.GenerateIV(); // Generate a random IV for each encryption

                // Encrypt the plain text
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    // Write the IV at the beginning of the encrypted data
                    ms.Write(aes.IV, 0, aes.IV.Length);
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        // Decrypt a string using a secret key
        public string DecryptString(string cipherText, string secretKey)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] fullCipher = Convert.FromBase64String(cipherText);

                // Derive key and extract IV from the encrypted data
                byte[] key = Encoding.UTF8.GetBytes(secretKey.PadRight(32, '0').Substring(0, 32));
                byte[] iv = new byte[16];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);

                aes.Key = key;
                aes.IV = iv;

                // Decrypt the cipher text
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
