namespace VertivProject.Interfaces
{
    public interface IEncriptionService
    {
        public string EncryptString(string plainText, string secretKey);

        public string DecryptString(string cipherText, string secretKey);
    }
}
