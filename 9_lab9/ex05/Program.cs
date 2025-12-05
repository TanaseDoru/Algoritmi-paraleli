using Newtonsoft.Json;
using System.Net;

namespace ex05
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (WebClient webClient = new WebClient())
            {
                string apiResponseJsonString = webClient.DownloadString("http://localhost:5000/api/image");

                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(apiResponseJsonString);

                // Access the properties of the deserialized object
                Console.WriteLine($"Status: {apiResponse.Status}");
                Console.WriteLine($"URL: {apiResponse.Url}");

                if (apiResponse.Status == "SUCCESS")
                {
                    // Download the image
                    byte[] imageData = webClient.DownloadData(apiResponse.Url);

                    // Extract the file name from the URL
                    string fileName = Path.GetFileName(new Uri(apiResponse.Url).LocalPath);

                    // Save the image
                    File.WriteAllBytes(fileName, imageData);

                    Console.WriteLine($"Downloaded: {apiResponse.Url} to {fileName}");
                }
            }
        }
    }
}