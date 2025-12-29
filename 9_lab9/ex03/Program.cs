using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace ex03
{
    internal class Program
    {
        static string password = "lalla";
        static void Main(string[] args)
        {
            string inputFile = "booksLarge.txt";

            int[] threadCounts = { 1, 2, 4, 8 };
            foreach (int threads in threadCounts)
            {
                Console.WriteLine($"\n=== Encrypting with {threads} threads");
                EncryptFileMultithreaded(inputFile, threads);
                //Console.WriteLine($"\n=== Decrypting with {threads} threads");
                //DecryptFileMultithreaded(inputFile, threads);
            }
        }

        static void DecryptFileMultithreaded(string inputFile, int threadCount)
        {
            string encryptedFile = inputFile + "_encrypted.bin";
            string outputDecryptedFile = inputFile + "_decrypted.txt";

            byte[] encryptedBytes = File.ReadAllBytes(encryptedFile);
            int fileLength = encryptedBytes.Length;
            int segmentSize = fileLength / threadCount;

            byte[][] decryptedSegments = new byte[threadCount][];

            Stopwatch sw = Stopwatch.StartNew();

            Parallel.For(0, threadCount, i =>
            {
                int start = i * segmentSize;
                int end = (i == threadCount - 1) ? fileLength : start + segmentSize;
                int length = end - start;

                byte[] encryptedSegment = new byte[length];
                Array.Copy(encryptedBytes, start, encryptedSegment, 0, length);

                byte[] iv = File.ReadAllBytes($"iv_{i}.bin");

                decryptedSegments[i] = DecryptSegment(encryptedSegment, password, iv);
            });

            sw.Stop();
            Console.WriteLine($"Decryption time: {sw.ElapsedMilliseconds} ms");

            using (FileStream fs = new FileStream(outputDecryptedFile, FileMode.Create))
            {
                foreach (var seg in decryptedSegments)
                    fs.Write(seg);
            }

            Console.WriteLine("Decryption finished.");
        }


        static bool CheckDecryption(string inputFile, string otherFile)
        {
            byte[] a = File.ReadAllBytes(inputFile);
            byte[] b = File.ReadAllBytes(otherFile);

            return a.SequenceEqual(b);
        }


        static void EncryptFileMultithreaded(string inputFile, int threadCount)
        {

            string outputEncryptedFile = inputFile + "_encrypted.bin";
            byte[] inputBytes = File.ReadAllBytes(inputFile);
            int fileLength = inputBytes.Length;
            int segmentSize = fileLength / threadCount;

            byte[][] encryptedSegments = new byte[threadCount][];
            byte[][] ivs = new byte[threadCount][];
            Stopwatch sw = Stopwatch.StartNew();

            Parallel.For(0, threadCount, i =>
            {
                int start = i * segmentSize;
                int end = (i == threadCount - 1) ? fileLength : start + segmentSize;
                int length = end - start;

                byte[] segment = new byte[length];
                Array.Copy(inputBytes, start ,segment, 0, length);

                using Aes aes = Aes.Create();
                aes.GenerateKey();
                aes.GenerateIV();

                ivs[i] = aes.IV;

                encryptedSegments[i] = EncryptSegment(segment, password, aes.IV);
            
            });

            sw.Stop();
            Console.WriteLine($"Time: {sw.ElapsedMilliseconds} ms");

            using (FileStream fs = new FileStream(outputEncryptedFile, FileMode.Create))
            {
                foreach (var seg in encryptedSegments)
                    fs.Write(seg);
            }

            string keysFile = $"{inputFile}_encrypted_{threadCount}_keys.txt";
            using StreamWriter writer = new StreamWriter(keysFile);

            writer.Write(threadCount);

            for(int i = 0; i < threadCount; i++)
            {
                int start = i * segmentSize;
                int end = (i == threadCount - 1) ? fileLength : start + segmentSize;

                string ivFile = $"iv_{i}.bin";

                File.WriteAllBytes(ivFile, ivs[i]);
            }
            
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