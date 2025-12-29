using System.Net.Http;

namespace ex02
{
    public class Downloader
    {
        private readonly HttpClient _httpClient;

        public Downloader()
        {
            _httpClient = new HttpClient();
        }

        public async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            try
            {
                return await _httpClient.GetByteArrayAsync(imageUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare descărcare {imageUrl}: {ex.Message}");
                return null;
            }
        }

        public async Task DownloadAndSaveAsync(string imageUrl, string fileName)
        {
            var data = await DownloadImageAsync(imageUrl);
            if (data != null)
            {
                await File.WriteAllBytesAsync(fileName, data);
                Console.WriteLine($"Salvată: {fileName}");
            }
            else
            {
                Console.WriteLine($"Eșec salvare: {fileName}");
            }
        }
    }
}