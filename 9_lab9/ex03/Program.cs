using System.Security.Cryptography;
using System.Text;

namespace ex03
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string inputFile = "books.txt";
            string outputEncryptedFile = inputFile + "_encrypted.bin";
            string outputDecryptedFile = inputFile + "_decrypted.txt";

            byte[] inputBytes = File.ReadAllBytes(inputFile);
            string password = "thisisasimplepassword";
            byte[] iv = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

            byte[] encryptedBytes = EncryptSegment(inputBytes, password, iv);
            File.WriteAllBytes(outputEncryptedFile, encryptedBytes);

            byte[] decryptedBytes = DecryptSegment(encryptedBytes, password, iv);
            File.WriteAllBytes(outputDecryptedFile, decryptedBytes);
        }

        static byte[] EncryptSegment(byte[] input, string password, byte[] iv)
        {
            using Aes aes = Aes.Create();
            aes.Key = Rfc2898DeriveBytes.Pbkdf2(Encoding.Unicode.GetBytes(password), Array.Empty<byte>(), 1234, HashAlgorithmName.SHA256, 16);
            aes.IV = iv;
            using MemoryStream output = new();
            using CryptoStream cryptoStream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(input);
            cryptoStream.FlushFinalBlock();
            return output.ToArray();
        }

        static byte[] DecryptSegment(byte[] input, string password, byte[] iv)
        {
            using Aes aes = Aes.Create();
            aes.Key = Rfc2898DeriveBytes.Pbkdf2(Encoding.Unicode.GetBytes(password), Array.Empty<byte>(), 1234, HashAlgorithmName.SHA256, 16);
            aes.IV = iv;
            using MemoryStream inputStream = new(input);
            using CryptoStream cryptoStream = new(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using MemoryStream output = new();
            cryptoStream.CopyTo(output);
            return output.ToArray();
        }
    }
}